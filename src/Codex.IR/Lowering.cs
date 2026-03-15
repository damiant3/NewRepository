using System.Collections.Immutable;
using Codex.Core;
using Codex.Ast;
using Codex.Types;

namespace Codex.IR;

public sealed class Lowering(
    Map<string, CodexType> typeMap,
    Map<string, CtorInfo> ctorMap,
    Map<string, CodexType> typeDefMap,
    DiagnosticBag diagnostics)
{
    readonly Map<string, CodexType> m_typeMap = typeMap;
    readonly Map<string, CtorInfo> m_ctorMap = ctorMap;
    readonly Map<string, CodexType> m_typeDefMap = typeDefMap;
    readonly DiagnosticBag m_diagnostics = diagnostics;
    Map<string, CodexType> m_localEnv = Map<string, CodexType>.s_empty;

    static readonly Map<string, CodexType> s_builtinTypes = BuildBuiltinTypes();

    public IRModule Lower(Module module)
    {
        ImmutableArray<IRDefinition>.Builder defs = ImmutableArray.CreateBuilder<IRDefinition>();
        foreach (Definition def in module.Definitions)
        {
            defs.Add(LowerDefinition(def));
        }
        return new(module.Name, defs.ToImmutable(), m_typeDefMap);
    }

    IRDefinition LowerDefinition(Definition def)
    {
        CodexType fullType = m_typeMap[def.Name.Value] ?? ErrorType.s_instance;

        Map<string, CodexType> savedEnv = m_localEnv;

        ImmutableArray<IRParameter>.Builder parameters = ImmutableArray.CreateBuilder<IRParameter>();
        CodexType currentType = fullType;
        foreach (Parameter param in def.Parameters)
        {
            while (currentType is FunctionType skipFt && skipFt.Parameter is ProofType)
                currentType = skipFt.Return;

            CodexType paramType;
            if (currentType is FunctionType ft)
            {
                paramType = ft.Parameter;
                currentType = ft.Return;
            }
            else if (currentType is DependentFunctionType dep)
            {
                paramType = dep.ParamType;
                currentType = dep.Body;
            }
            else
            {
                paramType = ErrorType.s_instance;
            }
            parameters.Add(new(param.Name.Value, paramType));
            m_localEnv = m_localEnv.Set(param.Name.Value, paramType);
        }

        while (currentType is FunctionType skipFt2 && skipFt2.Parameter is ProofType)
            currentType = skipFt2.Return;

        IRExpr body = LowerExpr(def.Body, currentType);
        m_localEnv = savedEnv;
        return new(def.Name.Value, parameters.ToImmutable(), fullType, body);
    }

    IRExpr LowerExpr(Expr expr, CodexType expectedType)
    {
        switch (expr)
        {
            case LiteralExpr lit:
                return LowerLiteral(lit);

            case NameExpr name:
                return new IRName(name.Name.Value, LookupName(name.Name.Value, expectedType));

            case BinaryExpr bin:
                return LowerBinary(bin, expectedType);

            case UnaryExpr un:
                return new IRNegate(LowerExpr(un.Operand, IntegerType.s_instance));

            case IfExpr iff:
            {
                IRExpr thenExpr = LowerExpr(iff.Then, expectedType);
                IRExpr elseExpr = LowerExpr(iff.Else, expectedType is ErrorType ? thenExpr.Type : expectedType);
                CodexType resultType = expectedType is ErrorType ? thenExpr.Type : expectedType;
                return new IRIf(
                    LowerExpr(iff.Condition, BooleanType.s_instance),
                    thenExpr,
                    elseExpr,
                    resultType);
            }

            case LetExpr let:
                return LowerLet(let, expectedType);

            case ApplyExpr app:
                return LowerApply(app, expectedType);

            case LambdaExpr lam:
                return LowerLambda(lam, expectedType);

            case MatchExpr match:
                return LowerMatch(match, expectedType);

            case ListExpr list:
                return LowerList(list, expectedType);

            case DoExpr doExpr:
                return LowerDoExpr(doExpr, expectedType);

            case RecordExpr rec:
                return LowerRecord(rec, expectedType);

            case FieldAccessExpr fa:
                return LowerFieldAccess(fa, expectedType);

            case ErrorExpr err:
                return new IRError(err.Message, expectedType);

            default:
                return new IRError($"Unsupported expression: {expr.GetType().Name}", expectedType);
        }
    }

    static IRExpr LowerLiteral(LiteralExpr lit)
    {
        return lit.Kind switch
        {
            LiteralKind.Integer => new IRIntegerLit(Convert.ToInt64(lit.Value)),
            LiteralKind.Number => new IRNumberLit(Convert.ToDecimal(lit.Value)),
            LiteralKind.Text => new IRTextLit((string)lit.Value),
            LiteralKind.Boolean => new IRBoolLit((bool)lit.Value),
            _ => new IRError("Unknown literal kind", ErrorType.s_instance)
        };
    }

    IRExpr LowerBinary(BinaryExpr bin, CodexType expectedType)
    {
        IRExpr left = LowerExpr(bin.Left, expectedType);
        IRExpr right = LowerExpr(bin.Right, expectedType);

        IRBinaryOp op = bin.Op switch
        {
            BinaryOp.Add when IsNumeric(left.Type) => IsInteger(left.Type) ? IRBinaryOp.AddInt : IRBinaryOp.AddNum,
            BinaryOp.Sub when IsNumeric(left.Type) => IsInteger(left.Type) ? IRBinaryOp.SubInt : IRBinaryOp.SubNum,
            BinaryOp.Mul when IsNumeric(left.Type) => IsInteger(left.Type) ? IRBinaryOp.MulInt : IRBinaryOp.MulNum,
            BinaryOp.Div when IsNumeric(left.Type) => IsInteger(left.Type) ? IRBinaryOp.DivInt : IRBinaryOp.DivNum,
            BinaryOp.Pow => IRBinaryOp.PowInt,
            BinaryOp.Add => IRBinaryOp.AddInt,
            BinaryOp.Sub => IRBinaryOp.SubInt,
            BinaryOp.Mul => IRBinaryOp.MulInt,
            BinaryOp.Div => IRBinaryOp.DivInt,
            BinaryOp.Eq => IRBinaryOp.Eq,
            BinaryOp.NotEq => IRBinaryOp.NotEq,
            BinaryOp.Lt => IRBinaryOp.Lt,
            BinaryOp.Gt => IRBinaryOp.Gt,
            BinaryOp.LtEq => IRBinaryOp.LtEq,
            BinaryOp.GtEq => IRBinaryOp.GtEq,
            BinaryOp.DefEq => IRBinaryOp.Eq,
            BinaryOp.And => IRBinaryOp.And,
            BinaryOp.Or => IRBinaryOp.Or,
            BinaryOp.Append when IsText(left.Type) || IsText(expectedType) => IRBinaryOp.AppendText,
            BinaryOp.Append => IRBinaryOp.AppendList,
            BinaryOp.Cons => IRBinaryOp.ConsList,
            _ => IRBinaryOp.AddInt
        };

        CodexType resultType = bin.Op switch
        {
            BinaryOp.Eq or BinaryOp.NotEq or BinaryOp.Lt or BinaryOp.Gt
                or BinaryOp.LtEq or BinaryOp.GtEq or BinaryOp.DefEq => BooleanType.s_instance,
            BinaryOp.And or BinaryOp.Or => BooleanType.s_instance,
            BinaryOp.Append when IsText(left.Type) || IsText(expectedType) => TextType.s_instance,
            _ => left.Type
        };

        return new IRBinary(op, left, right, resultType);
    }

    IRExpr LowerLet(LetExpr let, CodexType expectedType)
    {
        Map<string, CodexType> savedEnv = m_localEnv;

        List<(string Name, IRExpr Value)> loweredBindings = [];
        foreach (LetBinding binding in let.Bindings)
        {
            IRExpr value = LowerExpr(binding.Value, ErrorType.s_instance);
            loweredBindings.Add((binding.Name.Value, value));
            m_localEnv = m_localEnv.Set(binding.Name.Value, value.Type);
        }

        IRExpr body = LowerExpr(let.Body, expectedType);

        for (int i = loweredBindings.Count - 1; i >= 0; i--)
        {
            (string name, IRExpr value) = loweredBindings[i];
            body = new IRLet(name, value.Type, value, body);
        }

        m_localEnv = savedEnv;
        return body;
    }

    IRExpr LowerApply(ApplyExpr app, CodexType expectedType)
    {
        IRExpr func = LowerExpr(app.Function, ErrorType.s_instance);
        CodexType argType;
        CodexType returnType;
        if (func.Type is FunctionType ft)
        {
            argType = ft.Parameter;
            returnType = ft.Return;
        }
        else if (func.Type is DependentFunctionType dep)
        {
            argType = dep.ParamType;
            returnType = dep.Body;
        }
        else
        {
            argType = ErrorType.s_instance;
            returnType = expectedType;
        }
        IRExpr arg = LowerExpr(app.Argument, argType);

        returnType = SubstituteTypeVarsFromArg(argType, arg.Type, returnType);

        return new IRApply(func, arg, returnType);
    }

    static CodexType SubstituteTypeVarsFromArg(
        CodexType paramType, CodexType argType, CodexType target)
    {
        if (paramType is TypeVariable tv)
            return SubstituteTypeVar(target, tv.Id, argType);

        if (paramType is ListType lp && argType is ListType la)
            return SubstituteTypeVarsFromArg(lp.Element, la.Element, target);

        if (paramType is FunctionType fp && argType is FunctionType fa)
        {
            target = SubstituteTypeVarsFromArg(fp.Parameter, fa.Parameter, target);
            return SubstituteTypeVarsFromArg(fp.Return, fa.Return, target);
        }

        return target;
    }

    static CodexType SubstituteTypeVar(CodexType type, int varId, CodexType replacement)
    {
        return type switch
        {
            TypeVariable tv when tv.Id == varId => replacement,
            FunctionType ft => new FunctionType(
                SubstituteTypeVar(ft.Parameter, varId, replacement),
                SubstituteTypeVar(ft.Return, varId, replacement)),
            ListType lt => new ListType(SubstituteTypeVar(lt.Element, varId, replacement)),
            _ => type
        };
    }

    IRExpr LowerLambda(LambdaExpr lam, CodexType expectedType)
    {
        Map<string, CodexType> savedEnv = m_localEnv;

        ImmutableArray<IRParameter>.Builder parameters = ImmutableArray.CreateBuilder<IRParameter>();
        CodexType currentType = expectedType;
        foreach (Parameter p in lam.Parameters)
        {
            CodexType paramType;
            if (currentType is FunctionType ft)
            {
                paramType = ft.Parameter;
                currentType = ft.Return;
            }
            else if (currentType is DependentFunctionType dep)
            {
                paramType = dep.ParamType;
                currentType = dep.Body;
            }
            else
            {
                paramType = ErrorType.s_instance;
            }
            parameters.Add(new(p.Name.Value, paramType));
            m_localEnv = m_localEnv.Set(p.Name.Value, paramType);
        }

        IRExpr body = LowerExpr(lam.Body, currentType);
        m_localEnv = savedEnv;
        return new IRLambda(parameters.ToImmutable(), body, expectedType);
    }

    IRExpr LowerMatch(MatchExpr match, CodexType expectedType)
    {
        IRExpr scrutinee = LowerExpr(match.Scrutinee, ErrorType.s_instance);
        ImmutableArray<IRMatchBranch>.Builder branches = ImmutableArray.CreateBuilder<IRMatchBranch>();

        foreach (MatchBranch branch in match.Branches)
        {
            Map<string, CodexType> savedEnv = m_localEnv;
            IRPattern pattern = LowerPattern(branch.Pattern, scrutinee.Type);
            IRExpr body = LowerExpr(branch.Body, expectedType);
            branches.Add(new(pattern, body));
            m_localEnv = savedEnv;
        }

        return new IRMatch(scrutinee, branches.ToImmutable(), expectedType);
    }

    IRPattern LowerPattern(Pattern pattern, CodexType scrutineeType)
    {
        switch (pattern)
        {
            case VarPattern v:
                m_localEnv = m_localEnv.Set(v.Name.Value, scrutineeType);
                return new IRVarPattern(v.Name.Value, scrutineeType);
            case CtorPattern ctor:
                return LowerCtorPattern(ctor, scrutineeType);
            case LiteralPattern lit:
                return new IRLiteralPattern(lit.Value, scrutineeType);
            case WildcardPattern:
                return new IRWildcardPattern();
            default:
                return new IRWildcardPattern();
        }
    }

    IRPattern LowerCtorPattern(CtorPattern ctor, CodexType scrutineeType)
    {
        SumType? sumType = scrutineeType as SumType;
        if (sumType is null && scrutineeType is ConstructedType ct)
        {
            CodexType? resolved = m_typeDefMap[ct.Constructor.Value];
            sumType = resolved as SumType;
        }
        SumConstructorType? sumCtor = sumType?.Constructors
            .FirstOrDefault(c => c.Name.Value == ctor.Constructor.Value);

        if (sumCtor is null)
        {
            CtorInfo? ctorInfo = m_ctorMap[ctor.Constructor.Value];
            if (ctorInfo is not null)
            {
                CodexType ctorType = ctorInfo.ConstructorType;
                List<CodexType> fields = [];
                while (ctorType is FunctionType ft)
                {
                    fields.Add(ft.Parameter);
                    ctorType = ft.Return;
                }
                sumCtor = new SumConstructorType(ctor.Constructor,
                    [.. fields]);
            }
        }

        ImmutableArray<IRPattern>.Builder subPatterns = ImmutableArray.CreateBuilder<IRPattern>();
        for (int i = 0; i < ctor.SubPatterns.Count; i++)
        {
            CodexType fieldType = sumCtor is not null && i < sumCtor.Fields.Length
                ? sumCtor.Fields[i]
                : ErrorType.s_instance;
            subPatterns.Add(LowerPattern(ctor.SubPatterns[i], fieldType));
        }

        return new IRCtorPattern(ctor.Constructor.Value, subPatterns.ToImmutable(), scrutineeType);
    }

    IRExpr LowerList(ListExpr list, CodexType expectedType)
    {
        CodexType elementType = expectedType is ListType lt
            ? lt.Element
            : (list.Elements.Count > 0 ? InferElementType(list) : ErrorType.s_instance);

        ImmutableArray<IRExpr>.Builder elements = ImmutableArray.CreateBuilder<IRExpr>();
        foreach (Expr elem in list.Elements)
        {
            elements.Add(LowerExpr(elem, elementType));
        }

        return new IRList(elements.ToImmutable(), elementType);
    }

    static CodexType InferElementType(ListExpr list)
    {
        if (list.Elements.Count == 0) return ErrorType.s_instance;
        return list.Elements[0] switch
        {
            LiteralExpr lit => lit.Kind switch
            {
                LiteralKind.Integer => IntegerType.s_instance,
                LiteralKind.Number => NumberType.s_instance,
                LiteralKind.Text => TextType.s_instance,
                LiteralKind.Boolean => BooleanType.s_instance,
                _ => ErrorType.s_instance
            },
            _ => ErrorType.s_instance
        };
    }

    CodexType LookupName(string name, CodexType fallback)
    {
        CodexType result = m_localEnv[name]
            ?? m_typeMap[name]
            ?? m_ctorMap[name]?.ConstructorType
            ?? s_builtinTypes[name]
            ?? fallback;
        while (result is ForAllType fa)
            result = fa.Body;
        return result;
    }

    IRExpr LowerDoExpr(DoExpr doExpr, CodexType expectedType)
    {
        Map<string, CodexType> savedEnv = m_localEnv;
        ImmutableArray<IRDoStatement>.Builder statements = ImmutableArray.CreateBuilder<IRDoStatement>();

        foreach (DoStatement stmt in doExpr.Statements)
        {
            switch (stmt)
            {
                case DoBindStatement bind:
                {
                    IRExpr value = LowerExpr(bind.Value, ErrorType.s_instance);
                    CodexType boundType = value.Type is EffectfulType eft ? eft.Return : value.Type;
                    statements.Add(new IRDoBind(bind.Name.Value, boundType, value));
                    m_localEnv = m_localEnv.Set(bind.Name.Value, boundType);
                    break;
                }
                case DoExprStatement exprStmt:
                {
                    IRExpr value = LowerExpr(exprStmt.Expression, ErrorType.s_instance);
                    statements.Add(new IRDoExec(value));
                    break;
                }
            }
        }

        m_localEnv = savedEnv;
        return new IRDo(statements.ToImmutable(), expectedType);
    }

    IRExpr LowerRecord(RecordExpr rec, CodexType expectedType)
    {
        ImmutableArray<(string FieldName, IRExpr Value)>.Builder fields =
            ImmutableArray.CreateBuilder<(string, IRExpr)>();
        CodexType recType = expectedType;
        RecordType? rt = recType as RecordType;
        if (rt is null && rec.TypeName is not null)
        {
            string typeName = rec.TypeName.Value.Value;
            CodexType? looked = m_typeMap[typeName] ?? m_typeDefMap[typeName];
            rt = looked as RecordType;
            if (rt is not null)
                recType = rt;
        }

        foreach (RecordFieldExpr field in rec.Fields)
        {
            CodexType fieldType = ErrorType.s_instance;
            if (rt is not null)
            {
                RecordFieldType? rft = rt.Fields
                    .FirstOrDefault(f => f.FieldName.Value == field.FieldName.Value);
                if (rft is not null) fieldType = rft.Type;
            }
            fields.Add((field.FieldName.Value, LowerExpr(field.Value, fieldType)));
        }

        string emittedName = rec.TypeName is not null
            ? rec.TypeName.Value.Value
            : rt?.TypeName.Value ?? "Unknown";
        return new IRRecord(emittedName, fields.ToImmutable(), recType);
    }

    IRExpr LowerFieldAccess(FieldAccessExpr fa, CodexType expectedType)
    {
        IRExpr record = LowerExpr(fa.Record, ErrorType.s_instance);
        CodexType fieldType = expectedType;
        if (record.Type is RecordType rt)
        {
            RecordFieldType? rft = rt.Fields
                .FirstOrDefault(f => f.FieldName.Value == fa.FieldName.Value);
            if (rft is not null) fieldType = rft.Type;
        }
        return new IRFieldAccess(record, fa.FieldName.Value, fieldType);
    }

    static Map<string, CodexType> BuildBuiltinTypes()
    {
        Map<string, CodexType> map = Map<string, CodexType>.s_empty;
        map = map.Set("show", new ForAllType(0,
            new FunctionType(new TypeVariable(0), TextType.s_instance)));
        map = map.Set("negate", new FunctionType(IntegerType.s_instance, IntegerType.s_instance));

        EffectfulType consoleText = new(
            [new EffectType(new Name("Console"))],
            TextType.s_instance);
        map = map.Set("read-line", consoleText);

        EffectfulType consoleNothing = new(
            [new EffectType(new Name("Console"))],
            NothingType.s_instance);
        map = map.Set("print-line", new FunctionType(TextType.s_instance, consoleNothing));

        LinearType fileHandle = new(new ConstructedType(new Name("FileHandle"), []));

        EffectfulType fsFileHandle = new(
            [new EffectType(new Name("FileSystem"))],
            fileHandle);
        map = map.Set("open-file", new FunctionType(TextType.s_instance, fsFileHandle));

        EffectfulType fsTextAndHandle = new(
            [new EffectType(new Name("FileSystem"))],
            new ConstructedType(new Name("Pair"), [TextType.s_instance, fileHandle]));
        map = map.Set("read-all", new FunctionType(fileHandle, fsTextAndHandle));

        EffectfulType fsNothing = new(
            [new EffectType(new Name("FileSystem"))],
            NothingType.s_instance);
        map = map.Set("close-file", new FunctionType(fileHandle, fsNothing));

        map = map.Set("char-at", new FunctionType(TextType.s_instance,
            new FunctionType(IntegerType.s_instance, TextType.s_instance)));
        map = map.Set("text-length", new FunctionType(TextType.s_instance, IntegerType.s_instance));
        map = map.Set("substring", new FunctionType(TextType.s_instance,
            new FunctionType(IntegerType.s_instance,
                new FunctionType(IntegerType.s_instance, TextType.s_instance))));
        map = map.Set("is-letter", new FunctionType(TextType.s_instance, BooleanType.s_instance));
        map = map.Set("is-digit", new FunctionType(TextType.s_instance, BooleanType.s_instance));
        map = map.Set("is-whitespace", new FunctionType(TextType.s_instance, BooleanType.s_instance));
        map = map.Set("text-to-integer", new FunctionType(TextType.s_instance, IntegerType.s_instance));
        map = map.Set("integer-to-text", new FunctionType(IntegerType.s_instance, TextType.s_instance));
        map = map.Set("text-replace", new FunctionType(TextType.s_instance,
            new FunctionType(TextType.s_instance,
                new FunctionType(TextType.s_instance, TextType.s_instance))));
        map = map.Set("char-code", new FunctionType(TextType.s_instance, IntegerType.s_instance));
        map = map.Set("code-to-char", new FunctionType(IntegerType.s_instance, TextType.s_instance));

        map = map.Set("list-length", new ForAllType(0,
            new FunctionType(new ListType(new TypeVariable(0)), IntegerType.s_instance)));
        map = map.Set("list-at", new ForAllType(0,
            new FunctionType(new ListType(new TypeVariable(0)),
                new FunctionType(IntegerType.s_instance, new TypeVariable(0)))));
        return map;
    }

    static bool IsInteger(CodexType type) => type is IntegerType;

    static bool IsNumeric(CodexType type) => type is IntegerType or NumberType;

    static bool IsText(CodexType type) => type is TextType;
}
