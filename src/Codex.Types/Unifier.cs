using Codex.Core;

namespace Codex.Types;

public sealed class Unifier
{
    private readonly Dictionary<int, CodexType> m_substitutions = new();
    private readonly DiagnosticBag m_diagnostics;
    private int m_nextId;

    public Unifier(DiagnosticBag diagnostics)
    {
        m_diagnostics = diagnostics;
    }

    public TypeVariable FreshVar() => new(m_nextId++);

    public bool Unify(CodexType a, CodexType b, SourceSpan span)
    {
        a = Resolve(a);
        b = Resolve(b);

        if (a.Equals(b)) return true;

        if (a is TypeVariable va)
        {
            if (OccursIn(va.Id, b))
            {
                m_diagnostics.Error("CDX2010", $"Infinite type: {va} occurs in {b}", span);
                return false;
            }
            m_substitutions[va.Id] = b;
            return true;
        }

        if (b is TypeVariable vb)
        {
            if (OccursIn(vb.Id, a))
            {
                m_diagnostics.Error("CDX2010", $"Infinite type: {vb} occurs in {a}", span);
                return false;
            }
            m_substitutions[vb.Id] = a;
            return true;
        }

        if (a is FunctionType fa && b is FunctionType fb)
        {
            return Unify(fa.Parameter, fb.Parameter, span)
                && Unify(fa.Return, fb.Return, span);
        }

        if (a is ListType la && b is ListType lb)
            return Unify(la.Element, lb.Element, span);

        if (a is SumType sa && b is SumType sb && sa.TypeName == sb.TypeName)
            return true;

        if (a is RecordType ra && b is RecordType rb && ra.TypeName == rb.TypeName)
            return true;

        if (a is ConstructedType ca && b is ConstructedType cb)
        {
            if (ca.Constructor != cb.Constructor || ca.Arguments.Length != cb.Arguments.Length)
            {
                ReportMismatch(a, b, span);
                return false;
            }
            for (int i = 0; i < ca.Arguments.Length; i++)
            {
                if (!Unify(ca.Arguments[i], cb.Arguments[i], span))
                    return false;
            }
            return true;
        }

        if (a is ErrorType || b is ErrorType) return true;

        ReportMismatch(a, b, span);
        return false;
    }

    public CodexType Resolve(CodexType type)
    {
        while (type is TypeVariable tv && m_substitutions.TryGetValue(tv.Id, out CodexType? resolved))
        {
            type = resolved;
        }
        return type;
    }

    public CodexType DeepResolve(CodexType type)
    {
        type = Resolve(type);
        return type switch
        {
            FunctionType f => new FunctionType(DeepResolve(f.Parameter), DeepResolve(f.Return)),
            ListType l => new ListType(DeepResolve(l.Element)),
            ConstructedType c => c with
            {
                Arguments = [.. c.Arguments.Select(DeepResolve)]
            },
            ForAllType fa => fa with { Body = DeepResolve(fa.Body) },
            _ => type
        };
    }

    private bool OccursIn(int varId, CodexType type)
    {
        type = Resolve(type);
        return type switch
        {
            TypeVariable tv => tv.Id == varId,
            FunctionType f => OccursIn(varId, f.Parameter) || OccursIn(varId, f.Return),
            ListType l => OccursIn(varId, l.Element),
            ConstructedType c => c.Arguments.Any(a => OccursIn(varId, a)),
            ForAllType fa => OccursIn(varId, fa.Body),
            _ => false
        };
    }

    private void ReportMismatch(CodexType expected, CodexType actual, SourceSpan span)
    {
        m_diagnostics.Error("CDX2001",
            $"Type mismatch: expected {expected}, but found {actual}", span);
    }
}
