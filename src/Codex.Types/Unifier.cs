using Codex.Core;

namespace Codex.Types;

public sealed class Unifier
{
    private Map<int, CodexType> m_substitutions = Map<int, CodexType>.s_empty;
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
            m_substitutions = m_substitutions.Set(va.Id, b);
            return true;
        }

        if (b is TypeVariable vb)
        {
            if (OccursIn(vb.Id, a))
            {
                m_diagnostics.Error("CDX2010", $"Infinite type: {vb} occurs in {a}", span);
                return false;
            }
            m_substitutions = m_substitutions.Set(vb.Id, a);
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
        {
            if (sa.Constructors.Length != sb.Constructors.Length) return true;
            for (int i = 0; i < sa.Constructors.Length; i++)
            {
                SumConstructorType ca2 = sa.Constructors[i];
                SumConstructorType cb2 = sb.Constructors[i];
                int fieldCount = Math.Min(ca2.Fields.Length, cb2.Fields.Length);
                for (int j = 0; j < fieldCount; j++)
                    Unify(ca2.Fields[j], cb2.Fields[j], span);
            }
            return true;
        }

        if (a is RecordType ra && b is RecordType rb && ra.TypeName == rb.TypeName)
        {
            int fieldCount = Math.Min(ra.Fields.Length, rb.Fields.Length);
            for (int i = 0; i < fieldCount; i++)
                Unify(ra.Fields[i].Type, rb.Fields[i].Type, span);
            return true;
        }

        if (a is LinearType la2 && b is LinearType lb2)
            return Unify(la2.Inner, lb2.Inner, span);

        if (a is EffectfulType ea && b is EffectfulType eb)
        {
            return Unify(ea.Return, eb.Return, span);
        }

        if (a is EffectfulType ea2)
            return Unify(ea2.Return, b, span);

        if (b is EffectfulType eb2)
            return Unify(a, eb2.Return, span);

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
        while (type is TypeVariable tv && m_substitutions.TryGet(tv.Id, out CodexType? resolved))
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
            SumType s => s with
            {
                Constructors = [.. s.Constructors.Select(c => c with
                {
                    Fields = [.. c.Fields.Select(DeepResolve)]
                })]
            },
            RecordType r => r with
            {
                Fields = [.. r.Fields.Select(f => f with
                {
                    Type = DeepResolve(f.Type)
                })]
            },
            EffectfulType eft => eft with { Return = DeepResolve(eft.Return) },
            LinearType lin => new LinearType(DeepResolve(lin.Inner)),
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
            SumType s => s.Constructors.Any(c => c.Fields.Any(f => OccursIn(varId, f))),
            RecordType r => r.Fields.Any(f => OccursIn(varId, f.Type)),
            EffectfulType eft => OccursIn(varId, eft.Return),
            LinearType lin => OccursIn(varId, lin.Inner),
            _ => false
        };
    }

    private void ReportMismatch(CodexType expected, CodexType actual, SourceSpan span)
    {
        m_diagnostics.Error("CDX2001",
            $"Type mismatch: expected {expected}, but found {actual}", span);
    }
}
