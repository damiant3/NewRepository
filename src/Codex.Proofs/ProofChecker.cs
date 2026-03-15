using System.Collections.Immutable;
using Codex.Core;
using Codex.Ast;
using Codex.Types;

namespace Codex.Proofs;

public sealed class ProofChecker(DiagnosticBag diagnostics)
{
    readonly DiagnosticBag m_diagnostics = diagnostics;
    Map<string, EqualityType> m_claimMap = Map<string, EqualityType>.s_empty;
    Map<string, CodexType> m_typeMap = Map<string, CodexType>.s_empty;

    public void CheckModule(Module module, Map<string, CodexType> typeMap)
    {
        m_typeMap = typeMap;
        RegisterClaims(module.Claims);
        foreach (ProofDef proof in module.Proofs)
        {
            CheckProof(proof);
        }
    }

    public Map<string, EqualityType> ClaimMap => m_claimMap;

    void RegisterClaims(IReadOnlyList<ClaimDef> claims)
    {
        foreach (ClaimDef claim in claims)
        {
            CodexType left = ResolveClaimSide(claim.Left);
            CodexType right = ResolveClaimSide(claim.Right);
            EqualityType eqType = new(left, right);
            m_claimMap = m_claimMap.Set(claim.Name.Value, eqType);
        }
    }

    CodexType ResolveClaimSide(TypeExpr expr)
    {
        return expr switch
        {
            NamedTypeExpr named => ResolveNameInClaim(named.Name),
            AppliedTypeExpr app => ResolveAppliedInClaim(app),
            IntegerLiteralTypeExpr intLit => new TypeLevelValue(intLit.Value),
            BinaryTypeExpr bin => ResolveBinaryInClaim(bin),
            FunctionTypeExpr func => new FunctionType(
                ResolveClaimSide(func.Parameter), ResolveClaimSide(func.Return)),
            _ => ErrorType.s_instance
        };
    }

    CodexType ResolveNameInClaim(Name name)
    {
        return name.Value switch
        {
            "Integer" => IntegerType.s_instance,
            "Text" => TextType.s_instance,
            "Boolean" => BooleanType.s_instance,
            "Number" => NumberType.s_instance,
            "Nothing" => NothingType.s_instance,
            _ => new TypeLevelVar(name.Value)
        };
    }

    CodexType ResolveAppliedInClaim(AppliedTypeExpr app)
    {
        if (app.Constructor is NamedTypeExpr named)
        {
            ImmutableArray<CodexType> args =
                [.. app.Arguments.Select(ResolveClaimSide)];
            return new ConstructedType(named.Name, args);
        }
        return ErrorType.s_instance;
    }

    CodexType ResolveBinaryInClaim(BinaryTypeExpr bin)
    {
        CodexType left = ResolveClaimSide(bin.Left);
        CodexType right = ResolveClaimSide(bin.Right);
        TypeLevelOp op = bin.Op switch
        {
            BinaryOp.Add => TypeLevelOp.Add,
            BinaryOp.Sub => TypeLevelOp.Sub,
            BinaryOp.Mul => TypeLevelOp.Mul,
            _ => TypeLevelOp.Add
        };
        return NormalizeTypeLevelExpr(new TypeLevelBinary(op, left, right));
    }

    void CheckProof(ProofDef proof)
    {
        EqualityType? claim = m_claimMap[proof.Name.Value];
        if (claim is null)
        {
            m_diagnostics.Error("CDX4001",
                $"No claim named '{proof.Name.Value}' found",
                proof.Span);
            return;
        }

        Map<string, CodexType> proofEnv = Map<string, CodexType>.s_empty;
        foreach (Parameter param in proof.Parameters)
        {
            proofEnv = proofEnv.Set(param.Name.Value, new TypeLevelVar(param.Name.Value));
        }

        EqualityType? result = CheckProofExpr(proof.Body, claim, proofEnv);
        if (result is null)
        {
            m_diagnostics.Error("CDX4002",
                $"Proof '{proof.Name.Value}' failed to verify",
                proof.Span);
        }
    }

    EqualityType? CheckProofExpr(
        ProofExpr expr, EqualityType goal, Map<string, CodexType> env)
    {
        switch (expr)
        {
            case ReflProofExpr:
                if (TypesEqual(goal.Left, goal.Right, env))
                    return goal;
                m_diagnostics.Error("CDX4010",
                    $"Refl requires both sides to be equal, but got {goal.Left} and {goal.Right}",
                    expr.Span);
                return null;

            case SymProofExpr sym:
            {
                EqualityType flipped = new(goal.Right, goal.Left);
                EqualityType? inner = CheckProofExpr(sym.Inner, flipped, env);
                return inner is not null ? goal : null;
            }

            case TransProofExpr trans:
            {
                TypeLevelVar mid = new("_mid_" + expr.Span.Start.Offset);
                EqualityType leftGoal = new(goal.Left, mid);
                EqualityType rightGoal = new(mid, goal.Right);

                EqualityType? leftResult = CheckProofExpr(trans.Left, leftGoal, env);
                if (leftResult is null) return null;

                CodexType midResolved = leftResult.Right;
                EqualityType rightGoalResolved = new(midResolved, goal.Right);
                EqualityType? rightResult = CheckProofExpr(trans.Right, rightGoalResolved, env);
                return rightResult is not null ? goal : null;
            }

            case CongProofExpr cong:
            {
                EqualityType? innerResult = InferProofExpr(cong.Inner, env);
                if (innerResult is null) return null;

                CodexType newLeft = ApplyFunction(cong.FunctionName.Value, innerResult.Left);
                CodexType newRight = ApplyFunction(cong.FunctionName.Value, innerResult.Right);

                if (TypesEqual(newLeft, goal.Left, env) && TypesEqual(newRight, goal.Right, env))
                    return goal;

                m_diagnostics.Error("CDX4011",
                    $"Cong {cong.FunctionName.Value}: expected {goal}, but got {newLeft} ≡ {newRight}",
                    expr.Span);
                return null;
            }

            case InductionProofExpr ind:
                return CheckInduction(ind, goal, env);

            case NameProofExpr nameRef:
            {
                EqualityType? lemma = m_claimMap[nameRef.Name.Value];
                if (lemma is null)
                {
                    m_diagnostics.Error("CDX4012",
                        $"Unknown lemma: '{nameRef.Name.Value}'",
                        expr.Span);
                    return null;
                }
                if (TypesEqual(lemma.Left, goal.Left, env)
                    && TypesEqual(lemma.Right, goal.Right, env))
                    return goal;

                m_diagnostics.Error("CDX4013",
                    $"Lemma '{nameRef.Name.Value}' proves {lemma}, but goal is {goal}",
                    expr.Span);
                return null;
            }

            case ApplyProofExpr apply:
                return CheckProofApply(apply, goal, env);

            default:
                return null;
        }
    }

    EqualityType? InferProofExpr(ProofExpr expr, Map<string, CodexType> env)
    {
        switch (expr)
        {
            case ReflProofExpr:
                return null;

            case NameProofExpr nameRef:
                return m_claimMap[nameRef.Name.Value];

            case SymProofExpr sym:
            {
                EqualityType? inner = InferProofExpr(sym.Inner, env);
                if (inner is null) return null;
                return new EqualityType(inner.Right, inner.Left);
            }

            default:
                return null;
        }
    }

    EqualityType? CheckInduction(
        InductionProofExpr ind, EqualityType goal, Map<string, CodexType> env)
    {
        if (ind.Cases.Count == 0)
        {
            m_diagnostics.Error("CDX4020",
                "Induction proof has no cases", ind.Span);
            return null;
        }

        foreach (ProofCase proofCase in ind.Cases)
        {
            Map<string, CodexType> caseEnv = env;
            CodexType caseValue = PatternToType(proofCase.Pattern, ind.Variable.Value, caseEnv);

            EqualityType caseGoal = new(
                SubstituteVar(goal.Left, ind.Variable.Value, caseValue),
                SubstituteVar(goal.Right, ind.Variable.Value, caseValue));

            caseGoal = NormalizeEquality(caseGoal);

            Map<string, CodexType> innerEnv = caseEnv;
            if (proofCase.Pattern is CtorPattern ctor)
            {
                foreach (Pattern sub in ctor.SubPatterns)
                {
                    if (sub is VarPattern v)
                    {
                        innerEnv = innerEnv.Set(v.Name.Value, new TypeLevelVar(v.Name.Value));
                    }
                }
            }

            EqualityType? result = CheckProofExpr(proofCase.Body, caseGoal, innerEnv);
            if (result is null)
            {
                m_diagnostics.Error("CDX4021",
                    $"Induction case failed for pattern at {proofCase.Pattern.Span}",
                    proofCase.Span);
                return null;
            }
        }

        return goal;
    }

    EqualityType? CheckProofApply(
        ApplyProofExpr apply, EqualityType goal, Map<string, CodexType> env)
    {
        EqualityType? lemma = m_claimMap[apply.LemmaName.Value];
        if (lemma is null)
        {
            m_diagnostics.Error("CDX4012",
                $"Unknown lemma: '{apply.LemmaName.Value}'",
                apply.Span);
            return null;
        }

        if (TypesEqual(lemma.Left, goal.Left, env)
            && TypesEqual(lemma.Right, goal.Right, env))
            return goal;

        m_diagnostics.Error("CDX4013",
            $"Lemma '{apply.LemmaName.Value}' does not match goal {goal}",
            apply.Span);
        return null;
    }

    static CodexType PatternToType(Pattern pattern, string inductionVar, Map<string, CodexType> env)
    {
        return pattern switch
        {
            CtorPattern ctor => new TypeLevelVar(ctor.Constructor.Value),
            VarPattern v => new TypeLevelVar(v.Name.Value),
            LiteralPattern lit when lit.Kind == LiteralKind.Integer =>
                new TypeLevelValue(Convert.ToInt64(lit.Value)),
            WildcardPattern => new TypeLevelVar(inductionVar),
            _ => new TypeLevelVar(inductionVar)
        };
    }

    static CodexType SubstituteVar(CodexType type, string varName, CodexType replacement)
    {
        return type switch
        {
            TypeLevelVar tv when tv.Name == varName => replacement,
            ConstructedType c => c with
            {
                Arguments = [.. c.Arguments.Select(
                    a => SubstituteVar(a, varName, replacement))]
            },
            FunctionType f => new FunctionType(
                SubstituteVar(f.Parameter, varName, replacement),
                SubstituteVar(f.Return, varName, replacement)),
            ListType l => new ListType(SubstituteVar(l.Element, varName, replacement)),
            TypeLevelBinary bin => NormalizeTypeLevelExpr(new TypeLevelBinary(
                bin.Op,
                SubstituteVar(bin.Left, varName, replacement),
                SubstituteVar(bin.Right, varName, replacement))),
            EqualityType eq => new EqualityType(
                SubstituteVar(eq.Left, varName, replacement),
                SubstituteVar(eq.Right, varName, replacement)),
            _ => type
        };
    }

    static CodexType ApplyFunction(string funcName, CodexType arg)
    {
        return new ConstructedType(new Name(funcName), [arg]);
    }

    static bool TypesEqual(CodexType a, CodexType b, Map<string, CodexType> env)
    {
        CodexType normA = NormalizeTypeLevelExpr(a);
        CodexType normB = NormalizeTypeLevelExpr(b);

        return (normA, normB) switch
        {
            (TypeLevelValue va, TypeLevelValue vb) => va.Value == vb.Value,
            (TypeLevelVar va, TypeLevelVar vb) => va.Name == vb.Name,
            (IntegerType, IntegerType) => true,
            (TextType, TextType) => true,
            (BooleanType, BooleanType) => true,
            (NumberType, NumberType) => true,
            (NothingType, NothingType) => true,
            (VoidType, VoidType) => true,
            (ListType la, ListType lb) => TypesEqual(la.Element, lb.Element, env),
            (FunctionType fa, FunctionType fb) =>
                TypesEqual(fa.Parameter, fb.Parameter, env)
                && TypesEqual(fa.Return, fb.Return, env),
            (ConstructedType ca, ConstructedType cb) =>
                ca.Constructor.Value == cb.Constructor.Value
                && ca.Arguments.Length == cb.Arguments.Length
                && ca.Arguments.Zip(cb.Arguments).All(
                    pair => TypesEqual(pair.First, pair.Second, env)),
            (EqualityType ea, EqualityType eb) =>
                TypesEqual(ea.Left, eb.Left, env)
                && TypesEqual(ea.Right, eb.Right, env),
            _ => normA.Equals(normB)
        };
    }

    static EqualityType NormalizeEquality(EqualityType eq)
    {
        return new EqualityType(
            NormalizeTypeLevelExpr(eq.Left),
            NormalizeTypeLevelExpr(eq.Right));
    }

    static CodexType NormalizeTypeLevelExpr(CodexType type)
    {
        if (type is TypeLevelBinary bin)
        {
            CodexType left = NormalizeTypeLevelExpr(bin.Left);
            CodexType right = NormalizeTypeLevelExpr(bin.Right);

            if (left is TypeLevelValue lv && right is TypeLevelValue rv)
            {
                long result = bin.Op switch
                {
                    TypeLevelOp.Add => lv.Value + rv.Value,
                    TypeLevelOp.Sub => lv.Value - rv.Value,
                    TypeLevelOp.Mul => lv.Value * rv.Value,
                    _ => 0
                };
                return new TypeLevelValue(result);
            }

            return new TypeLevelBinary(bin.Op, left, right);
        }

        if (type is ConstructedType ct)
        {
            ImmutableArray<CodexType> normArgs =
                [.. ct.Arguments.Select(NormalizeTypeLevelExpr)];
            return ct with { Arguments = normArgs };
        }

        return type;
    }
}
