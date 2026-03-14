using System.Text;
using Codex.Emit;
using Codex.IR;
using Codex.Types;

namespace Codex.Emit.CSharp;

public sealed class CSharpEmitter : ICodeEmitter
{
    public string TargetName => "C#";
    public string FileExtension => ".cs";

    public string Emit(IRModule module)
    {
        StringBuilder sb = new();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine();

        EmitTypeDefinitions(sb, module);

        string className = "Codex_" + SanitizeIdentifier(module.Name.Leaf.Value);

        IRDefinition? mainDef = module.Definitions
            .FirstOrDefault(d => d.Name == "main");

        if (mainDef is not null && mainDef.Parameters.Length == 0)
        {
            sb.AppendLine($"Console.WriteLine({className}.main());");
            sb.AppendLine();
        }

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

    private static void EmitTypeDefinitions(StringBuilder sb, IRModule module)
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

    private static void EmitSumType(StringBuilder sb, SumType sum)
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

    private static void EmitRecordType(StringBuilder sb, RecordType rec)
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

    private static void EmitDefinition(StringBuilder sb, IRDefinition def)
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
        sb.Append("        return ");
        EmitExpr(sb, def.Body, 2);
        sb.AppendLine(";");
        sb.AppendLine("    }");
    }

    private static void EmitExpr(StringBuilder sb, IRExpr expr, int indent)
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
                EmitExpr(sb, app.Function, indent);
                sb.Append('(');
                EmitExpr(sb, app.Argument, indent);
                sb.Append(')');
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

            case IRError err:
                sb.Append($"throw new InvalidOperationException(\"{EscapeString(err.Message)}\")");
                break;

            default:
                sb.Append("default");
                break;
        }
    }

    private static void EmitBinary(StringBuilder sb, IRBinary bin, int indent)
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

    private static void EmitLet(StringBuilder sb, IRLet let, int indent)
    {
        string funcType = $"Func<{EmitType(let.NameType)}, {EmitType(let.Body.Type)}>";
        sb.Append($"(({funcType})((");
        sb.Append(SanitizeIdentifier(let.Name));
        sb.Append(") => ");
        EmitExpr(sb, let.Body, indent);
        sb.Append("))(");
        EmitExpr(sb, let.Value, indent);
        sb.Append(')');
    }

    private static void EmitLambda(StringBuilder sb, IRLambda lam, int indent)
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

    private static void EmitList(StringBuilder sb, IRList list, int indent)
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

    private static void EmitMatch(StringBuilder sb, IRMatch match, int indent)
    {
        bool first = true;
        foreach (IRMatchBranch branch in match.Branches)
        {
            if (!first) sb.Append(" : ");
            first = false;

            switch (branch.Pattern)
            {
                case IRLiteralPattern litPat:
                    sb.Append('(');
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
                    EmitExpr(sb, match.Scrutinee, indent);
                    sb.Append($" is {ctorId} {binding} ? ");
                    EmitCtorPatternBody(sb, ctorPat, binding, branch.Body, indent);
                    break;

                case IRVarPattern varPat:
                    string varFuncType = $"Func<{EmitType(varPat.Type)}, {EmitType(branch.Body.Type)}>";
                    sb.Append($"(({varFuncType})((");
                    sb.Append(SanitizeIdentifier(varPat.Name));
                    sb.Append(") => ");
                    EmitExpr(sb, branch.Body, indent);
                    sb.Append("))(");
                    EmitExpr(sb, match.Scrutinee, indent);
                    sb.Append(')');
                    return;

                case IRWildcardPattern:
                    EmitExpr(sb, branch.Body, indent);
                    return;
            }
        }

        sb.Append($" : throw new InvalidOperationException(\"Non-exhaustive match\")");
    }

    private static void EmitCtorPatternBody(
        StringBuilder sb, IRCtorPattern ctorPat, string bindingName,
        IRExpr body, int indent)
    {
        List<(string Name, string Access, CodexType Type)> bindings = new();
        for (int i = 0; i < ctorPat.SubPatterns.Length; i++)
        {
            if (ctorPat.SubPatterns[i] is IRVarPattern vp)
            {
                bindings.Add((vp.Name, $"{bindingName}.Field{i}", vp.Type));
            }
        }

        if (bindings.Count == 0)
        {
            EmitExpr(sb, body, indent);
            return;
        }

        IRExpr current = body;
        for (int i = bindings.Count - 1; i >= 0; i--)
        {
            (string name, string access, CodexType type) = bindings[i];
            string funcType = $"Func<{EmitType(type)}, {EmitType(current.Type)}>";
            sb.Append($"(({funcType}((");
            sb.Append(SanitizeIdentifier(name));
            sb.Append(") => ");
        }

        EmitExpr(sb, body, indent);

        for (int i = 0; i < bindings.Count; i++)
        {
            sb.Append($"))({bindings[i].Access})");
        }
    }

    private static string EmitType(CodexType type)
    {
        return type switch
        {
            IntegerType => "long",
            NumberType => "decimal",
            TextType => "string",
            BooleanType => "bool",
            NothingType => "object",
            VoidType => "void",
            FunctionType ft => $"Func<{EmitType(ft.Parameter)}, {EmitType(ft.Return)}>",
            ListType lt => $"List<{EmitType(lt.Element)}>",
            SumType st => SanitizeIdentifier(st.TypeName.Value),
            RecordType rt => SanitizeIdentifier(rt.TypeName.Value),
            TypeVariable => "object",
            ErrorType => "object",
            _ => "object"
        };
    }

    private static CodexType GetReturnType(IRDefinition def)
    {
        CodexType type = def.Type;
        for (int i = 0; i < def.Parameters.Length; i++)
        {
            if (type is FunctionType ft)
                type = ft.Return;
            else
                break;
        }
        return type;
    }

    private static string SanitizeIdentifier(string name)
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

    private static string EscapeString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}
