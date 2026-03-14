using System.Collections.Immutable;
using Codex.Core;

namespace Codex.Types;

// The internal type representation used by the type checker.
// TypeExpr (in Codex.Ast) is the surface syntax; CodexType is what it resolves to.

public abstract record CodexType;

// Primitive types
public sealed record IntegerType : CodexType
{
    public static readonly IntegerType s_instance = new();
    public override string ToString() => "Integer";
}

public sealed record NumberType : CodexType
{
    public static readonly NumberType s_instance = new();
    public override string ToString() => "Number";
}

public sealed record TextType : CodexType
{
    public static readonly TextType s_instance = new();
    public override string ToString() => "Text";
}

public sealed record BooleanType : CodexType
{
    public static readonly BooleanType s_instance = new();
    public override string ToString() => "Boolean";
}

public sealed record NothingType : CodexType
{
    public static readonly NothingType s_instance = new();
    public override string ToString() => "Nothing";
}

public sealed record VoidType : CodexType
{
    public static readonly VoidType s_instance = new();
    public override string ToString() => "Void";
}

// Function type: Parameter → Return
public sealed record FunctionType(CodexType Parameter, CodexType Return) : CodexType
{
    public override string ToString()
    {
        string paramStr = Parameter is FunctionType ? $"({Parameter})" : Parameter.ToString()!;
        return $"{paramStr} → {Return}";
    }
}

// Type constructor applied to arguments: List Integer, Result Text, etc.
public sealed record ConstructedType(Name Constructor, ImmutableArray<CodexType> Arguments) : CodexType
{
    public override string ToString()
    {
        if (Arguments.IsEmpty) return Constructor.Value;
        return $"{Constructor.Value} {string.Join(" ", Arguments)}";
    }
}

// A type variable, used during inference. Identity is by reference (Id).
public sealed record TypeVariable(int Id) : CodexType
{
    public override string ToString() => $"?t{Id}";
}

// A universally quantified (polymorphic) type: forall a. body
public sealed record ForAllType(int VariableId, CodexType Body) : CodexType
{
    public override string ToString() => $"∀t{VariableId}. {Body}";
}

// List type — so common it gets a shortcut
public sealed record ListType(CodexType Element) : CodexType
{
    public override string ToString() => $"List {Element}";
}

// Error sentinel — produced when type resolution fails, prevents cascading errors
public sealed record ErrorType : CodexType
{
    public static readonly ErrorType s_instance = new();
    public override string ToString() => "<error>";
}
