# 07 — Transpilation & Code Generation

## Overview

Codex compiles to a semantic intermediate representation (IR) and then lowers to target languages. The targets are not equal — some can represent the full Codex type system, others can only represent a subset. The emission framework manages this gracefully.

This document defines the IR, the emission framework, and the design of each backend.

---

## The Codex IR

### Design Principles

1. **Typed**: every IR node carries its full type, including effects and linearity
2. **A-Normal Form**: all intermediate values are named (no nested expressions)
3. **Explicit closures**: lambda captures are made explicit
4. **Explicit effects**: effect operations are IR nodes, not implicit side effects
5. **Target-agnostic**: no target-specific constructs in the IR
6. **Optimizable**: the IR is the input to optimization passes

### IR Node Types

```
IRModule
├── name       : QualifiedName
├── definitions : IRDefinition[]
└── dependencies : QualifiedName[]

IRDefinition
├── name       : QualifiedName
├── type       : IRType
├── params     : IRParam[]
├── body       : IRExpr
├── attributes : IRAttribute[] (inline hints, export markers, etc.)
└── sourceSpan : SourceSpan

IRExpr =
  | IRLet        (name, type, value : IRExpr, body : IRExpr)
  | IRLetRec     (bindings : (name, type, IRExpr)[], body : IRExpr)
  | IRVar        (name, type)
  | IRLiteral    (value : Literal, type)
  | IRApply      (function : IRExpr, args : IRExpr[], type)
  | IRLambda     (params : IRParam[], captures : IRCapture[], body : IRExpr, type)
  | IRMatch      (scrutinee : IRExpr, branches : IRBranch[], type)
  | IRIf         (condition : IRExpr, then : IRExpr, else : IRExpr, type)
  | IRRecord     (fields : (name, IRExpr)[], type)
  | IRFieldAccess(record : IRExpr, field : name, type)
  | IRRecordWith (base : IRExpr, updates : (name, IRExpr)[], type)
  | IRConstruct  (constructor : QualifiedName, args : IRExpr[], type)
  | IREffectOp   (effect : EffectLabel, operation : name, args : IRExpr[], type)
  | IRHandle     (body : IRExpr, handlers : IRHandler[], type)
  | IRReturn     (value : IRExpr, type)
  | IRUnreachable(type)  -- provably unreachable code (after exhaustive match)

IRBranch
├── pattern : IRPattern
└── body    : IRExpr

IRPattern =
  | IRPatternVar       (name, type)
  | IRPatternLiteral   (value : Literal)
  | IRPatternConstructor(constructor : QualifiedName, subPatterns : IRPattern[])
  | IRPatternWildcard

IRType =
  | IRPrimitive   (PrimitiveKind)  -- Text, Number, Integer, Boolean, Nothing, Void
  | IRFunction    (params : IRType[], effects : EffectRow, result : IRType)
  | IRRecord      (fields : (name, IRType)[])
  | IRVariant     (constructors : (name, IRType[])[])
  | IRGeneric     (name, kind)
  | IRApplied     (constructor : IRType, args : IRType[])
  | IRLinear      (inner : IRType)
```

### Type Erasure Levels

Different backends need different levels of type information:

| Level | What's Kept | Used By |
|-------|-------------|---------|
| **Full** | All types, effects, linearity | Rust, Haskell backends |
| **Simple** | Types without dependent/linear | C#, TypeScript backends |
| **Erased** | No types at runtime | Python, LLVM backends |

The IR carries full type information. Each backend's emitter decides what to keep.

---

## Emission Framework

### The Emitter Interface

```csharp
public interface ICodeEmitter
{
    string TargetName { get; }
    string FileExtension { get; }
    TargetCapabilities Capabilities { get; }
    
    EmitResult Emit(IRModule module, EmitOptions options);
    
    // Can this emitter handle the given IR?
    // Returns diagnostics for unsupported features.
    ImmutableArray<Diagnostic> ValidateCapabilities(IRModule module);
}

[Flags]
public enum TargetCapabilities
{
    None                    = 0,
    DependentTypes          = 1 << 0,   // Types can depend on values
    LinearTypes             = 1 << 1,   // Linear resource tracking
    EffectTypes             = 1 << 2,   // Effect rows in types
    HigherKindedTypes       = 1 << 3,   // Type constructors as arguments
    TailCallOptimization    = 1 << 4,   // Guaranteed TCO
    ArbitraryPrecision      = 1 << 5,   // Big integers / rationals natively
    PatternMatching         = 1 << 6,   // Native pattern matching
    AlgebraicDataTypes      = 1 << 7,   // Native sum types
    Closures                = 1 << 8,   // First-class functions with capture
    GarbageCollection       = 1 << 9,   // Automatic memory management
    ManualMemory            = 1 << 10,  // Manual memory / ownership
}
```

### Capability Degradation Strategy

When a backend cannot represent a Codex feature, the emitter has three options (in order of preference):

1. **Encode**: represent the feature using the target's available features (e.g., sum types as class hierarchies in C#)
2. **Insert runtime check**: replace the compile-time guarantee with a runtime assertion (e.g., dependent type constraints become `Debug.Assert`)
3. **Reject**: refuse to emit with a clear explanation ("Cannot emit this module to Python because it uses linear types, which Python cannot enforce")

The user is always informed. Safety is never silently lost.

---

## Backend Designs

### C# Backend (Priority: First — Bootstrap Target)

**Capabilities**: Simple types, closures, GC, pattern matching (via switch expressions), no dependent/linear/effect types at the type level.

**Encoding strategy**:

| Codex Feature | C# Encoding |
|--------------|-------------|
| `Text` | `string` |
| `Number` | `decimal` (or `BigRational` for arbitrary precision) |
| `Integer` | `BigInteger` |
| `Boolean` | `bool` |
| `Nothing` | `Unit` (custom struct) |
| `List (a)` | `ImmutableList<T>` |
| `Maybe (a)` | Custom `Maybe<T>` discriminated union |
| `Result (a)` | Custom `Result<T>` discriminated union |
| Sum types | Abstract record + derived records (C# discriminated unions pattern) |
| Record types | C# `record` types |
| Pattern matching | C# switch expressions with pattern matching |
| Pure functions | Static methods |
| Effectful functions | Methods that take/return effect contexts (or just use IO directly) |
| Linear types | Runtime checks (`Debug.Assert` for use-once) |
| Dependent types | Runtime assertions for constraints |
| Higher-kinded types | Interface-based encoding |

**Output structure**:
```
output/
├── Codex.Runtime/          # Runtime support library
│   ├── Unit.cs
│   ├── Maybe.cs
│   ├── Result.cs
│   ├── CodexList.cs
│   └── Effects.cs
├── ModuleName/
│   ├── ModuleName.cs       # Generated module
│   └── ...
└── ModuleName.csproj       # Generated project file
```

### JavaScript/TypeScript Backend (Priority: Second)

**Capabilities**: Closures, GC, no static types (JS) or simple types (TS). No dependent, linear, or effect types.

| Codex Feature | JS/TS Encoding |
|--------------|----------------|
| Sum types | Tagged objects: `{ tag: "Success", value: ... }` |
| Records | Plain objects with TypeScript interfaces |
| Pattern matching | Switch on tag field |
| Effects | Ignored at type level; documented in JSDoc |
| Linear types | Not enforced; documented |
| Dependent types | Runtime assertions |
| `Number` | `bigint` for Integer, `number` for Number (lossy!) |

### Rust Backend (Priority: Third)

**Capabilities**: Full fidelity. Linear types map to ownership. Effects map to trait bounds. Pattern matching and algebraic types are native.

| Codex Feature | Rust Encoding |
|--------------|---------------|
| Sum types | `enum` |
| Records | `struct` |
| Linear types | Ownership (move semantics) — natural fit |
| Effects | Trait bounds on generic parameters |
| Pattern matching | Native `match` |
| `Number` | `num::BigRational` |
| `Integer` | `num::BigInt` |
| Dependent types | Runtime assertions (Rust cannot express dependent types) |

### Python Backend (Priority: Fourth)

**Capabilities**: Minimal. Dynamic typing. No compile-time guarantees beyond syntax.

| Codex Feature | Python Encoding |
|--------------|-----------------|
| Types | Type hints (PEP 484) — documentation only |
| Sum types | Dataclasses with a `tag` field |
| Records | Dataclasses |
| Pattern matching | `match` statement (Python 3.10+) |
| Everything else | Runtime assertions |

### WASM Backend (Priority: Fifth)

**Capabilities**: Low-level execution. Types erased at runtime. Good performance.

Strategy: Compile Codex IR → a C-like representation → compile with a WASM toolchain (Emscripten or wasm-ld). Alternatively, emit WASM text format directly.

### LLVM IR Backend (Priority: Sixth)

**Capabilities**: Full control over code generation. Types erased. Maximum performance.

Strategy: Emit LLVM IR text format. Use LLVM toolchain to produce native binaries. This is the path to native Codex executables with no runtime dependency.

---

## Runtime Library

Each backend has a **runtime library** that provides:

1. **Codex primitive types** (where the target doesn't have them natively)
2. **Effect system runtime** (effect handlers, effect stack)
3. **Pattern matching infrastructure** (where not native)
4. **Standard library bindings** (Codex stdlib functions implemented in the target language)

The runtime library is written by hand in each target language. It is small — most logic lives in the generated code.

---

## Self-Hosting Path

The ultimate goal is **self-hosting**: the Codex compiler is written in Codex and compiles itself.

```
Stage 0: C# compiler (this project) compiles Codex source
Stage 1: Stage 0 compiles the Codex compiler written in Codex → produces a new compiler
Stage 2: Stage 1 compiler compiles itself → produces a compiler that should be identical to Stage 1
```

When Stage 1 output = Stage 2 output, we have achieved self-hosting. The C# bootstrap becomes historical.

The self-hosting milestone is gated on:
- The language being expressive enough to write a compiler
- The C# backend being complete enough to compile the compiler
- The standard library being rich enough for compiler needs (string handling, file I/O, data structures)

---

## Open Questions

1. **C# runtime library scope** — how much do we implement by hand vs. generate? The more we generate, the less maintenance burden; but the runtime needs to exist before the generator does.

2. **Number representation** — `decimal` is convenient but limited to 28-29 significant digits. `BigRational` from a NuGet package gives arbitrary precision but has performance implications. Decision: start with `decimal`, add `BigRational` when needed.

3. **Effect encoding in C#** — effects can be encoded as:
   - Reader/Writer monad pattern (verbose, pure)
   - Interface injection (familiar to .NET developers)
   - Ambient context (global state, impure but simple)
   - We need to pick one for the bootstrap and stick with it.

4. **WASM strategy** — direct WASM emission vs. compiling through C/LLVM. Direct emission gives more control but is much more work. Going through LLVM gives us WASM + native for one backend investment.

5. **Source maps** — for the JS and WASM backends, we need source maps that map generated code back to Codex source. This requires carrying source spans through the entire pipeline.


Summary of JS & Rust emitter fixes post-bootstrap:
JavaScript (JavaScriptEmitter.cs)
Fix	Before	After
Match paren balance	(((_s) => ... — extra unmatched (	((_s) => ... — balanced
Ctor pattern binding	Nested IIFEs with reversed binding order	Single IIFE with const bindings
integer-to-text	Missing	String(x)
text-replace	Missing	.replaceAll(old, new)
Rust (RustEmitter.cs)
Fix	Before	After
fn main collision	Skipped main def, generated recursive fn main() calling main()	Emits codex_main(), Rust fn main() calls it
DependentFunctionType brace	{EmitType(dep.Body)}}> — extra }	{EmitType(dep.Body)}> — correct interpolation
integer-to-text	Missing	.to_string()
text-replace	Missing	.replace(&old, &new)
Both
Sample	JS	Rust
hello.codex	✅ 25n	✅ compiles
factorial.codex	✅ 3628800n	✅ compiles
fibonacci.codex	✅ 6765n	✅ compiles
greeting.codex	✅ Hello, World!	✅ compiles
shapes.codex	✅ 78.5	✅ compiles
person.codex	✅ Hello, Alice!	✅ compiles
string-ops.codex	✅ 10n	✅ compiles
effectful-hello.codex	✅ compiles	✅ compiles
