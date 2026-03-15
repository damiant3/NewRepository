using System.Text;
using Codex.Core;
using Codex.IR;
using Codex.Types;

namespace Codex.Emit.CSharp;

public sealed class CSharpEmitter : ICodeEmitter
{
    Set<string> m_constructorNames = Set<string>.s_empty;
    ValueMap<string, int> m_definitionArity = ValueMap<string, int>.s_empty;

    public string TargetName => "C#";
    public string FileExtension => ".cs";

    public string Emit(IRModule module)
    {
        m_constructorNames = CollectConstructorNames(module);
        m_definitionArity = ValueMap<string, int>.s_empty;
        foreach (IRDefinition d in module.Definitions)
            m_definitionArity = m_definitionArity.Set(d.Name, d.Parameters.Length);

        StringBuilder sb = new();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine();

        string className = "Codex_" + SanitizeIdentifier(module.Name.Leaf.Value);

        IRDefinition? mainDef = module.Definitions
            .FirstOrDefault(d => d.Name == "main");

        if (mainDef is not null && mainDef.Parameters.Length == 0)
        {
            if (IsEffectfulDefinition(mainDef))
            {
                sb.AppendLine($"{className}.main();");
            }
            else
            {
                sb.AppendLine($"Console.WriteLine({className}.main());");
            }
            sb.AppendLine();
        }

        EmitTypeDefinitions(sb, module);

        sb.AppendLine($"public static class {className}");
        sb.AppendLine("{");

        foreach (IRDefinition def in module.Definitions)
        {
            EmitDefinition(sb, def);
            sb.AppendLine();
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    static void EmitTypeDefinitions(StringBuilder sb, IRModule module)
    {
        foreach (KeyValuePair<string, CodexType> kv in module.TypeDefinitions)
        {
            switch (kv.Value)
            {
                case SumType sum:
                    EmitSumType(sb, sum);
                    break;
                case RecordType rec:
                    EmitRecordType(sb, rec);
                    break;
            }
        }
    }

    static void EmitSumType(StringBuilder sb, SumType sum)
    {
        string baseName = SanitizeIdentifier(sum.TypeName.Value);
        sb.AppendLine($"public abstract record {baseName};");
        sb.AppendLine();

        foreach (SumConstructorType ctor in sum.Constructors)
        {
            string ctorName = SanitizeIdentifier(ctor.Name.Value);
            if (ctor.Fields.IsEmpty)
            {
                sb.AppendLine($"public sealed record {ctorName} : {baseName};");
            }
            else
            {
                sb.Append($"public sealed record {ctorName}(");
                for (int i = 0; i < ctor.Fields.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append($"{EmitType(ctor.Fields[i])} Field{i}");
                }
                sb.AppendLine($") : {baseName};");
            }
        }
        sb.AppendLine();
    }

    static void EmitRecordType(StringBuilder sb, RecordType rec)
    {
        string name = SanitizeIdentifier(rec.TypeName.Value);
        sb.Append($"public sealed record {name}(");
        for (int i = 0; i < rec.Fields.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            RecordFieldType field = rec.Fields[i];
            sb.Append($"{EmitType(field.Type)} {SanitizeIdentifier(field.FieldName.Value)}");
        }
        sb.AppendLine(");");
        sb.AppendLine();
    }

    static Set<string> CollectConstructorNames(IRModule module)
    {
        Set<string> names = Set<string>.s_empty;
        foreach (KeyValuePair<string, CodexType> kv in module.TypeDefinitions)
        {
            if (kv.Value is SumType sum)
            {
                foreach (SumConstructorType ctor in sum.Constructors)
                {
                    names = names.Add(ctor.Name.Value);
                }
            }
        }
        return names;
    }

    void EmitDefinition(StringBuilder sb, IRDefinition def)
    {
        string returnType = EmitType(GetReturnType(def));
        string name = SanitizeIdentifier(def.Name);

        sb.Append($"    public static {returnType} {name}(");

        for (int i = 0; i < def.Parameters.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            IRParameter param = def.Parameters[i];
            sb.Append($"{EmitType(param.Type)} {SanitizeIdentifier(param.Name)}");
        }

        sb.AppendLine(")");
        sb.AppendLine("    {");

        if (IsEffectfulDefinition(def))
        {
            sb.Append("        ");
            EmitExpr(sb, def.Body, 2);
            sb.AppendLine(";");
            sb.AppendLine("        return null;");
        }
        else
        {
            sb.Append("        return ");
            EmitExpr(sb, def.Body, 2);
            sb.AppendLine(";");
        }

        sb.AppendLine("    }");
    }

    void EmitExpr(StringBuilder sb, IRExpr expr, int indent)
    {
        switch (expr)
        {
            case IRIntegerLit lit:
                sb.Append($"{lit.Value}L");
                break;

            case IRNumberLit lit:
                sb.Append($"{lit.Value}m");
                break;

            case IRTextLit lit:
                sb.Append($"\"{EscapeString(lit.Value)}\"");
                break;

            case IRBoolLit lit:
                sb.Append(lit.Value ? "true" : "false");
                break;

            case IRName name:
                if (name.Name == "read-line")
                    sb.Append("Console.ReadLine()");
                else if (name.Name == "show")
                    sb.Append("new Func<object, string>(x => Convert.ToString(x))");
                else if (name.Name == "negate")
                    sb.Append("new Func<long, long>(x => -x)");
                else if (name.Name.Length > 0 && char.IsUpper(name.Name[0])
                    && name.Type is not FunctionType)
                    sb.Append($"new {SanitizeIdentifier(name.Name)}()");
                else
                    sb.Append(SanitizeIdentifier(name.Name));
                break;

            case IRBinary bin:
                EmitBinary(sb, bin, indent);
                break;

            case IRNegate neg:
                sb.Append("(-");
                EmitExpr(sb, neg.Operand, indent);
                sb.Append(')');
                break;

            case IRIf iff:
                sb.Append('(');
                EmitExpr(sb, iff.Condition, indent);
                sb.Append(" ? ");
                EmitExpr(sb, iff.Then, indent);
                sb.Append(" : ");
                EmitExpr(sb, iff.Else, indent);
                sb.Append(')');
                break;

            case IRLet let:
                EmitLet(sb, let, indent);
                break;

            case IRApply app:
                if (app.Function is IRName fn && fn.Name == "show")
                {
                    sb.Append("Convert.ToString(");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(')');
                }
                else if (app.Function is IRName fn2 && fn2.Name == "negate")
                {
                    sb.Append("(-");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(')');
                }
                else if (app.Function is IRName fn3 && fn3.Name == "print-line")
                {
                    sb.Append("Console.WriteLine(");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(')');
                }
                else if (app.Function is IRName fn4 && fn4.Name == "open-file")
                {
                    sb.Append("File.OpenRead(");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(')');
                }
                else if (app.Function is IRName fn5 && fn5.Name == "read-all")
                {
                    sb.Append("new System.IO.StreamReader(");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(").ReadToEnd()");
                }
                else if (app.Function is IRName fn6 && fn6.Name == "close-file")
                {
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(".Dispose()");
                }
                else if (app.Function is IRName fn7 && fn7.Name == "text-length")
                {
                    sb.Append("((long)");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(".Length)");
                }
                else if (app.Function is IRName fn8 && fn8.Name == "is-letter")
                {
                    sb.Append("(");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(".Length > 0 && char.IsLetter(");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append("[0]))");
                }
                else if (app.Function is IRName fn9 && fn9.Name == "is-digit")
                {
                    sb.Append("(");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(".Length > 0 && char.IsDigit(");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append("[0]))");
                }
                else if (app.Function is IRName fn10 && fn10.Name == "is-whitespace")
                {
                    sb.Append("(");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(".Length > 0 && char.IsWhiteSpace(");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append("[0]))");
                }
                else if (app.Function is IRName fn11 && fn11.Name == "text-to-integer")
                {
                    sb.Append("long.Parse(");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(')');
                }
                else if (app.Function is IRName fn11b && fn11b.Name == "integer-to-text")
                {
                    sb.Append('(');
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(").ToString()");
                }
                else if (app.Function is IRName fn12 && fn12.Name == "char-code")
                {
                    sb.Append("((long)");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append("[0])");
                }
                else if (app.Function is IRName fn13 && fn13.Name == "code-to-char")
                {
                    sb.Append("((char)");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(").ToString()");
                }
                else if (app.Function is IRName fn14 && fn14.Name == "list-length")
                {
                    sb.Append("((long)");
                    EmitExpr(sb, app.Argument, indent);
                    sb.Append(".Count)");
                }
                else if (TryEmitMultiArgBuiltin(sb, app, indent))
                {
                }
                else
                {
                    string? ctorName = FindConstructorName(app);
                    if (ctorName is not null)
                    {
                        List<IRExpr> args = [];
                        CollectApplyArgs(app, args);
                        sb.Append($"new {SanitizeIdentifier(ctorName)}(");
                        for (int i = 0; i < args.Count; i++)
                        {
                            if (i > 0) sb.Append(", ");
                            EmitExpr(sb, args[i], indent);
                        }
                        sb.Append(')');
                    }
                    else
                    {
                        string? defName = FindDefinitionName(app);
                        if (defName is not null
                            && m_definitionArity.TryGet(defName, out int arity)
                            && arity > 1)
                        {
                            List<IRExpr> args = [];
                            CollectApplyArgs(app, args);
                            if (args.Count == arity)
                            {
                                sb.Append(SanitizeIdentifier(defName));
                                sb.Append('(');
                                for (int i = 0; i < args.Count; i++)
                                {
                                    if (i > 0) sb.Append(", ");
                                    EmitExpr(sb, args[i], indent);
                                }
                                sb.Append(')');
                            }
                            else
                            {
                                EmitExpr(sb, app.Function, indent);
                                sb.Append('(');
                                EmitExpr(sb, app.Argument, indent);
                                sb.Append(')');
                            }
                        }
                        else
                        {
                            EmitExpr(sb, app.Function, indent);
                            sb.Append('(');
                            EmitExpr(sb, app.Argument, indent);
                            sb.Append(')');
                        }
                    }
                }
                break;

            case IRLambda lam:
                EmitLambda(sb, lam, indent);
                break;

            case IRList list:
                EmitList(sb, list, indent);
                break;

            case IRMatch match:
                EmitMatch(sb, match, indent);
                break;

            case IRDo doExpr:
                EmitDoExpr(sb, doExpr, indent);
                break;

            case IRRecord rec:
                sb.Append($"new {SanitizeIdentifier(rec.TypeName)}(");
                for (int i = 0; i < rec.Fields.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    EmitExpr(sb, rec.Fields[i].Value, indent);
                }
                sb.Append(')');
                break;

            case IRFieldAccess fa:
                EmitExpr(sb, fa.Record, indent);
                sb.Append('.');
                sb.Append(SanitizeIdentifier(fa.FieldName));
                break;

            case IRError err:
                sb.Append($"throw new InvalidOperationException(\"{EscapeString(err.Message)}\")");
                break;

            default:
                sb.Append("default");
                break;
        }
    }

    static string? FindConstructorName(IRApply app)
    {
        IRExpr current = app.Function;
        while (current is IRApply inner)
        {
            current = inner.Function;
        }
        if (current is IRName name && name.Name.Length > 0 && char.IsUpper(name.Name[0]))
        {
            return name.Name;
        }
        return null;
    }

    static string? FindDefinitionName(IRApply app)
    {
        IRExpr current = app.Function;
        while (current is IRApply inner)
        {
            current = inner.Function;
        }
        if (current is IRName name && name.Name.Length > 0 && char.IsLower(name.Name[0]))
        {
            return name.Name;
        }
        return null;
    }

    static void CollectApplyArgs(IRApply app, List<IRExpr> args)
    {
        if (app.Function is IRApply inner)
        {
            CollectApplyArgs(inner, args);
        }
        args.Add(app.Argument);
    }

    static readonly Set<string> s_multiArgBuiltins = Set<string>.Of("char-at", "substring", "list-at", "text-replace");

    static string? FindBuiltinRoot(IRApply app)
    {
        IRExpr current = app.Function;
        while (current is IRApply inner)
            current = inner.Function;
        if (current is IRName name && s_multiArgBuiltins.Contains(name.Name))
            return name.Name;
        return null;
    }

    bool TryEmitMultiArgBuiltin(StringBuilder sb, IRApply app, int indent)
    {
        string? name = FindBuiltinRoot(app);
        if (name is null) return false;

        List<IRExpr> args = [];
        CollectApplyArgs(app, args);

        switch (name)
        {
            case "char-at" when args.Count == 2:
                EmitExpr(sb, args[0], indent);
                sb.Append("[(int)");
                EmitExpr(sb, args[1], indent);
                sb.Append("].ToString()");
                return true;

            case "substring" when args.Count == 3:
                EmitExpr(sb, args[0], indent);
                sb.Append(".Substring((int)");
                EmitExpr(sb, args[1], indent);
                sb.Append(", (int)");
                EmitExpr(sb, args[2], indent);
                sb.Append(')');
                return true;

            case "list-at" when args.Count == 2:
                EmitExpr(sb, args[0], indent);
                sb.Append("[(int)");
                EmitExpr(sb, args[1], indent);
                sb.Append(']');
                return true;

            case "text-replace" when args.Count == 3:
                EmitExpr(sb, args[0], indent);
                sb.Append(".Replace(");
                EmitExpr(sb, args[1], indent);
                sb.Append(", ");
                EmitExpr(sb, args[2], indent);
                sb.Append(')');
                return true;

            default:
                return false;
        }
    }

    void EmitBinary(StringBuilder sb, IRBinary bin, int indent)
    {
        switch (bin.Op)
        {
            case IRBinaryOp.AppendText:
                sb.Append("string.Concat(");
                EmitExpr(sb, bin.Left, indent);
                sb.Append(", ");
                EmitExpr(sb, bin.Right, indent);
                sb.Append(')');
                break;

            case IRBinaryOp.AppendList:
                sb.Append("Enumerable.Concat(");
                EmitExpr(sb, bin.Left, indent);
                sb.Append(", ");
                EmitExpr(sb, bin.Right, indent);
                sb.Append(").ToList()");
                break;

            case IRBinaryOp.ConsList:
                sb.Append("new List<");
                sb.Append(EmitType(bin.Left.Type));
                sb.Append("> { ");
                EmitExpr(sb, bin.Left, indent);
                sb.Append(" }.Concat(");
                EmitExpr(sb, bin.Right, indent);
                sb.Append(").ToList()");
                break;

            case IRBinaryOp.PowInt:
                sb.Append("(long)Math.Pow((double)");
                EmitExpr(sb, bin.Left, indent);
                sb.Append(", (double)");
                EmitExpr(sb, bin.Right, indent);
                sb.Append(')');
                break;

            default:
                string op = bin.Op switch
                {
                    IRBinaryOp.AddInt or IRBinaryOp.AddNum => "+",
                    IRBinaryOp.SubInt or IRBinaryOp.SubNum => "-",
                    IRBinaryOp.MulInt or IRBinaryOp.MulNum => "*",
                    IRBinaryOp.DivInt or IRBinaryOp.DivNum => "/",
                    IRBinaryOp.Eq => "==",
                    IRBinaryOp.NotEq => "!=",
                    IRBinaryOp.Lt => "<",
                    IRBinaryOp.Gt => ">",
                    IRBinaryOp.LtEq => "<=",
                    IRBinaryOp.GtEq => ">=",
                    IRBinaryOp.And => "&&",
                    IRBinaryOp.Or => "||",
                    _ => "+"
                };
                sb.Append('(');
                EmitExpr(sb, bin.Left, indent);
                sb.Append($" {op} ");
                EmitExpr(sb, bin.Right, indent);
                sb.Append(')');
                break;
        }
    }

    void EmitLet(StringBuilder sb, IRLet let, int indent)
    {
        string funcType = $"Func<{EmitType(let.NameType)}, {EmitType(let.Body.Type)}>";
        sb.Append("((" + funcType + ")((");
        sb.Append(SanitizeIdentifier(let.Name));
        sb.Append(") => ");
        EmitExpr(sb, let.Body, indent);
        sb.Append("))(");
        EmitExpr(sb, let.Value, indent);
        sb.Append(')');
    }

    void EmitLambda(StringBuilder sb, IRLambda lam, int indent)
    {
        sb.Append('(');
        for (int i = 0; i < lam.Parameters.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append($"{EmitType(lam.Parameters[i].Type)} {SanitizeIdentifier(lam.Parameters[i].Name)}");
        }
        sb.Append(") => ");
        EmitExpr(sb, lam.Body, indent);
    }

    void EmitList(StringBuilder sb, IRList list, int indent)
    {
        sb.Append($"new List<{EmitType(list.ElementType)}>()");
        if (list.Elements.Length > 0)
        {
            sb.Append(" { ");
            for (int i = 0; i < list.Elements.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                EmitExpr(sb, list.Elements[i], indent);
            }
            sb.Append(" }");
        }
    }

    void EmitMatch(StringBuilder sb, IRMatch match, int indent)
    {
        bool hasMultipleCtorBranches = match.Branches
            .Count(b => b.Pattern is IRCtorPattern or IRLiteralPattern) > 1;

        string scrutineeRef;
        if (hasMultipleCtorBranches)
        {
            string scrutineeType = EmitType(match.Scrutinee.Type);
            sb.Append($"((Func<{scrutineeType}, {EmitType(match.Type)}>)((_scrutinee_) => ");
            scrutineeRef = "_scrutinee_";
        }
        else
        {
            scrutineeRef = "";
        }

        bool first = true;
        int openParens = 0;
        foreach (IRMatchBranch branch in match.Branches)
        {
            if (!first) sb.Append(" : ");
            first = false;

            switch (branch.Pattern)
            {
                case IRLiteralPattern litPat:
                    sb.Append('(');
                    openParens++;
                    if (hasMultipleCtorBranches)
                        sb.Append(scrutineeRef);
                    else
                        EmitExpr(sb, match.Scrutinee, indent);
                    sb.Append(litPat.Value switch
                    {
                        bool b => $".Equals({(b ? "true" : "false")})",
                        long l => $" == {l}L",
                        string s => $" == \"{EscapeString(s)}\"",
                        _ => $".Equals({litPat.Value})"
                    });
                    sb.Append(" ? ");
                    EmitExpr(sb, branch.Body, indent);
                    break;

                case IRCtorPattern ctorPat:
                    string ctorId = SanitizeIdentifier(ctorPat.Name);
                    string binding = $"_m{ctorId}_";
                    sb.Append('(');
                    openParens++;
                    if (hasMultipleCtorBranches)
                        sb.Append(scrutineeRef);
                    else
                        EmitExpr(sb, match.Scrutinee, indent);
                    sb.Append($" is {ctorId} {binding} ? ");
                    EmitCtorPatternBody(sb, ctorPat, binding, branch.Body, indent);
                    break;

                case IRVarPattern varPat:
                    string varFuncType = $"Func<{EmitType(varPat.Type)}, {EmitType(branch.Body.Type)}>";
                    sb.Append("((" + varFuncType + ")((");
                    sb.Append(SanitizeIdentifier(varPat.Name));
                    sb.Append(") => ");
                    EmitExpr(sb, branch.Body, indent);
                    sb.Append("))(");
                    if (hasMultipleCtorBranches)
                        sb.Append(scrutineeRef);
                    else
                        EmitExpr(sb, match.Scrutinee, indent);
                    sb.Append(')');
                    for (int i = 0; i < openParens; i++) sb.Append(')');
                    if (hasMultipleCtorBranches)
                    {
                        sb.Append("))(");
                        EmitExpr(sb, match.Scrutinee, indent);
                        sb.Append(')');
                    }
                    return;

                case IRWildcardPattern:
                    EmitExpr(sb, branch.Body, indent);
                    for (int i = 0; i < openParens; i++) sb.Append(')');
                    if (hasMultipleCtorBranches)
                    {
                        sb.Append("))(");
                        EmitExpr(sb, match.Scrutinee, indent);
                        sb.Append(')');
                    }
                    return;
            }
        }

        sb.Append($" : throw new InvalidOperationException(\"Non-exhaustive match\")");
        for (int i = 0; i < openParens; i++) sb.Append(')');
        if (hasMultipleCtorBranches)
        {
            sb.Append("))(");
            EmitExpr(sb, match.Scrutinee, indent);
            sb.Append(')');
        }
    }

    void EmitCtorPatternBody(
        StringBuilder sb, IRCtorPattern ctorPat, string bindingName,
        IRExpr body, int indent)
    {
        List<(string Name, string Access, CodexType Type)> varBindings = [];
        List<(IRCtorPattern SubCtor, string Access)> nestedCtors = [];
        CollectPatternBindings(ctorPat, bindingName, varBindings, nestedCtors);

        if (nestedCtors.Count > 0)
        {
            EmitNestedCtorChecks(sb, nestedCtors, 0, varBindings, body, indent);
            return;
        }

        EmitVarBindingsAndBody(sb, varBindings, body, indent);
    }

    static void CollectPatternBindings(
        IRCtorPattern ctorPat, string bindingName,
        List<(string Name, string Access, CodexType Type)> varBindings,
        List<(IRCtorPattern SubCtor, string Access)> nestedCtors)
    {
        for (int i = 0; i < ctorPat.SubPatterns.Length; i++)
        {
            switch (ctorPat.SubPatterns[i])
            {
                case IRVarPattern vp:
                    varBindings.Add((vp.Name, $"{bindingName}.Field{i}", vp.Type));
                    break;
                case IRCtorPattern nested:
                    nestedCtors.Add((nested, $"{bindingName}.Field{i}"));
                    CollectPatternBindings(nested, $"_m{SanitizeIdentifier(nested.Name)}_{nestedCtors.Count}_", varBindings, nestedCtors);
                    break;
            }
        }
    }

    void EmitNestedCtorChecks(
        StringBuilder sb,
        List<(IRCtorPattern SubCtor, string Access)> nestedCtors,
        int idx,
        List<(string Name, string Access, CodexType Type)> varBindings,
        IRExpr body, int indent)
    {
        if (idx >= nestedCtors.Count)
        {
            EmitVarBindingsAndBody(sb, varBindings, body, indent);
            return;
        }

        (IRCtorPattern subCtor, string access) = nestedCtors[idx];
        string subCtorId = SanitizeIdentifier(subCtor.Name);
        string subBinding = $"_m{subCtorId}_{idx}_";
        sb.Append($"({access} is {subCtorId} {subBinding} ? ");

        List<(string, string, CodexType)> patchedBindings = [];
        foreach ((string name, string acc, CodexType type) in varBindings)
        {
            string patchedAccess = acc;
            string oldPrefix = $"_m{subCtorId}_{nestedCtors.Count}_";
            if (acc.StartsWith(oldPrefix))
            {
                patchedAccess = subBinding + acc[oldPrefix.Length..];
            }
            patchedBindings.Add((name, patchedAccess, type));
        }

        EmitNestedCtorChecks(sb, nestedCtors, idx + 1, patchedBindings, body, indent);
        sb.Append($" : throw new InvalidOperationException(\"Pattern match failed\"))");
    }

    void EmitVarBindingsAndBody(
        StringBuilder sb,
        List<(string Name, string Access, CodexType Type)> bindings,
        IRExpr body, int indent)
    {
        if (bindings.Count == 0)
        {
            EmitExpr(sb, body, indent);
            return;
        }

        for (int i = bindings.Count - 1; i >= 0; i--)
        {
            (string name, string _, CodexType type) = bindings[i];
            string funcType = $"Func<{EmitType(type)}, {EmitType(body.Type)}>";
            sb.Append("((" + funcType + ")((");
            sb.Append(SanitizeIdentifier(name));
            sb.Append(") => ");
        }

        EmitExpr(sb, body, indent);

        for (int i = 0; i < bindings.Count; i++)
        {
            (string _, string access, CodexType type) = bindings[i];
            string castStr = type is not (ErrorType or TypeVariable or NothingType or VoidType)
                ? $"({EmitType(type)})"
                : "";
            sb.Append("))(");
            sb.Append(castStr);
            sb.Append(access);
            sb.Append(')');
        }
    }

    void EmitDoExpr(StringBuilder sb, IRDo doExpr, int indent)
    {
        sb.AppendLine("((Func<object>)(() => {");
        string pad = new(' ', (indent + 2) * 4);
        foreach (IRDoStatement stmt in doExpr.Statements)
        {
            switch (stmt)
            {
                case IRDoBind bind:
                    sb.Append(pad);
                    sb.Append($"var {SanitizeIdentifier(bind.Name)} = ");
                    EmitExpr(sb, bind.Value, indent + 2);
                    sb.AppendLine(";");
                    break;
                case IRDoExec exec:
                    sb.Append(pad);
                    EmitExpr(sb, exec.Expression, indent + 2);
                    sb.AppendLine(";");
                    break;
            }
        }
        sb.Append(pad);
        sb.Append("return null;");
        sb.AppendLine();
        sb.Append(new string(' ', (indent + 1) * 4));
        sb.Append("}))()");
    }

    static string EmitType(CodexType type)
    {
        return type switch
        {
            IntegerType => "long",
            NumberType => "decimal",
            TextType => "string",
            BooleanType => "bool",
            NothingType => "object",
            VoidType => "void",
            EffectfulType eft => EmitType(eft.Return),
            LinearType lin => EmitType(lin.Inner),
            FunctionType ft => $"Func<{EmitType(ft.Parameter)}, {EmitType(ft.Return)}>",
            DependentFunctionType dep => $"Func<{EmitType(dep.ParamType)}, {EmitType(dep.Body)}>",
            ListType lt => $"List<{EmitType(lt.Element)}>",
            SumType st => SanitizeIdentifier(st.TypeName.Value),
            RecordType rt => SanitizeIdentifier(rt.TypeName.Value),
            ConstructedType ct => SanitizeIdentifier(ct.Constructor.Value),
            TypeLevelValue => "long",
            TypeLevelVar => "long",
            TypeLevelBinary => "long",
            ProofType => "object",
            TypeVariable => "object",
            ErrorType => "object",
            _ => "object"
        };
    }

    static CodexType GetReturnType(IRDefinition def)
    {
        CodexType type = def.Type;
        for (int i = 0; i < def.Parameters.Length; i++)
        {
            while (type is FunctionType pft && pft.Parameter is ProofType)
                type = pft.Return;
            if (type is FunctionType ft)
                type = ft.Return;
            else if (type is DependentFunctionType dep)
                type = dep.Body;
            else
                break;
        }
        while (type is FunctionType pft2 && pft2.Parameter is ProofType)
            type = pft2.Return;
        if (type is EffectfulType eft)
            type = eft.Return;
        return type;
    }

    static bool IsEffectfulDefinition(IRDefinition def)
    {
        CodexType type = def.Type;
        for (int i = 0; i < def.Parameters.Length; i++)
        {
            while (type is FunctionType pft && pft.Parameter is ProofType)
                type = pft.Return;
            if (type is FunctionType ft)
                type = ft.Return;
            else if (type is DependentFunctionType dep)
                type = dep.Body;
            else
                break;
        }
        while (type is FunctionType pft2 && pft2.Parameter is ProofType)
            type = pft2.Return;
        return type is EffectfulType;
    }

    static string SanitizeIdentifier(string name)
    {
        string sanitized = name.Replace('-', '_');

        return sanitized switch
        {
            "class" or "static" or "void" or "return" or "if" or "else" or "for"
            or "while" or "do" or "switch" or "case" or "break" or "continue"
            or "new" or "this" or "base" or "null" or "true" or "false" or "int"
            or "long" or "string" or "bool" or "double" or "decimal" or "object"
            or "in" or "is" or "as" or "typeof" or "default" or "throw" or "try"
            or "catch" or "finally" or "using" or "namespace" or "public" or "private"
            or "protected" or "internal" or "abstract" or "sealed" or "override"
            or "virtual" or "event" or "delegate" or "out" or "ref" or "params"
                => $"@{sanitized}",
            _ => sanitized
        };
    }

    static string EscapeString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}
