using System.Collections.Immutable;
using Codex.Core;
using Codex.Ast;

namespace Codex.Types;

public sealed class LinearityChecker
{
    private readonly DiagnosticBag m_diagnostics;
    private readonly ImmutableDictionary<string, CodexType> m_typeMap;
    private Dictionary<string, int> m_usageCounts;
    private Map<string, CodexType> m_linearBindings;

    public LinearityChecker(DiagnosticBag diagnostics, ImmutableDictionary<string, CodexType> typeMap)
    {
        m_diagnostics = diagnostics;
        m_typeMap = typeMap;
        m_usageCounts = new();
        m_linearBindings = Map<string, CodexType>.s_empty;
    }

    public void CheckModule(Module module)
    {
        foreach (Definition def in module.Definitions)
        {
            CheckDefinition(def);
        }
    }

    private void CheckDefinition(Definition def)
    {
        Dictionary<string, int> savedCounts = m_usageCounts;
        Map<string, CodexType> savedLinear = m_linearBindings;
        m_usageCounts = new();
        m_linearBindings = Map<string, CodexType>.s_empty;

        CodexType defType = m_typeMap.TryGetValue(def.Name.Value, out CodexType? dt)
            ? dt : ErrorType.s_instance;
        CodexType currentType = defType;
        foreach (Parameter param in def.Parameters)
        {
            CodexType paramType;
            if (currentType is FunctionType ft)
            {
                paramType = ft.Parameter;
                currentType = ft.Return;
            }
            else
            {
                paramType = ErrorType.s_instance;
            }

            if (paramType is LinearType)
            {
                m_linearBindings = m_linearBindings.Set(param.Name.Value, paramType);
                m_usageCounts[param.Name.Value] = 0;
            }
        }

        CheckExpr(def.Body);
        ReportUnusedLinearBindings(def.Span);

        m_usageCounts = savedCounts;
        m_linearBindings = savedLinear;
    }

    private void CheckExpr(Expr expr)
    {
        switch (expr)
        {
            case NameExpr name:
                RecordUsage(name.Name.Value, name.Span);
                break;

            case LiteralExpr:
                break;

            case BinaryExpr bin:
                CheckExpr(bin.Left);
                CheckExpr(bin.Right);
                break;

            case UnaryExpr un:
                CheckExpr(un.Operand);
                break;

            case ApplyExpr app:
                CheckExpr(app.Function);
                CheckExpr(app.Argument);
                break;

            case IfExpr iff:
                CheckExpr(iff.Condition);
                Dictionary<string, int> savedThen = CloneCounts();
                CheckExpr(iff.Then);
                Dictionary<string, int> afterThen = CloneCounts();
                m_usageCounts = savedThen;
                CheckExpr(iff.Else);
                Dictionary<string, int> afterElse = CloneCounts();
                MergeBranchCounts(afterThen, afterElse, iff.Span);
                break;

            case LetExpr let:
                foreach (LetBinding binding in let.Bindings)
                {
                    CheckExpr(binding.Value);
                }
                CheckExpr(let.Body);
                break;

            case LambdaExpr lam:
                CheckExpr(lam.Body);
                break;

            case MatchExpr match:
                CheckExpr(match.Scrutinee);
                CheckMatchBranches(match);
                break;

            case ListExpr list:
                foreach (Expr element in list.Elements)
                    CheckExpr(element);
                break;

            case RecordExpr rec:
                foreach (RecordFieldExpr field in rec.Fields)
                    CheckExpr(field.Value);
                break;

            case FieldAccessExpr fa:
                CheckExpr(fa.Record);
                break;

            case DoExpr doExpr:
                foreach (DoStatement stmt in doExpr.Statements)
                {
                    switch (stmt)
                    {
                        case DoBindStatement bind:
                            CheckExpr(bind.Value);
                            break;
                        case DoExprStatement exprStmt:
                            CheckExpr(exprStmt.Expression);
                            break;
                    }
                }
                break;

            case ErrorExpr:
                break;
        }
    }

    private void CheckMatchBranches(MatchExpr match)
    {
        if (match.Branches.Count == 0) return;

        Dictionary<string, int> savedCounts = CloneCounts();
        Dictionary<string, int>? mergedCounts = null;

        foreach (MatchBranch branch in match.Branches)
        {
            RestoreCounts(savedCounts);
            CheckExpr(branch.Body);
            if (mergedCounts is null)
            {
                mergedCounts = CloneCounts();
            }
            else
            {
                MergeBranchCounts(mergedCounts, CloneCounts(), match.Span);
                mergedCounts = CloneCounts();
            }
        }

        if (mergedCounts is not null)
            RestoreCounts(mergedCounts);
    }

    private void MergeBranchCounts(
        Dictionary<string, int> branch1,
        Dictionary<string, int> branch2,
        SourceSpan span)
    {
        foreach (KeyValuePair<string, CodexType> kv in m_linearBindings)
        {
            string name = kv.Key;
            int count1 = branch1.GetValueOrDefault(name, 0);
            int count2 = branch2.GetValueOrDefault(name, 0);
            if (count1 != count2)
            {
                m_diagnostics.Error("CDX2042",
                    $"Linear variable '{name}' is used inconsistently across branches " +
                    $"({count1} time(s) in one branch, {count2} in another)",
                    span);
            }
        }
    }

    private void RecordUsage(string name, SourceSpan span)
    {
        if (m_linearBindings[name] is null) return;

        int current = m_usageCounts.GetValueOrDefault(name, 0);
        m_usageCounts[name] = current + 1;

        if (current + 1 > 1)
        {
            m_diagnostics.Error("CDX2041",
                $"Linear variable '{name}' is used more than once",
                span);
        }
    }

    private void ReportUnusedLinearBindings(SourceSpan span)
    {
        foreach (KeyValuePair<string, CodexType> kv in m_linearBindings)
        {
            int count = m_usageCounts.GetValueOrDefault(kv.Key, 0);
            if (count == 0)
            {
                m_diagnostics.Error("CDX2040",
                    $"Linear variable '{kv.Key}' is never used (linear resources must be consumed)",
                    span);
            }
        }
    }

    private Dictionary<string, int> CloneCounts() => new(m_usageCounts);

    private void RestoreCounts(Dictionary<string, int> counts)
    {
        m_usageCounts = new(counts);
    }
}
