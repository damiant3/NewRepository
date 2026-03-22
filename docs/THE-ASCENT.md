# The Ascent

**A high-level path forward for Codex**
*2026-03-21*

---

> *"Because it's there."*
> — George Mallory, when asked why he wanted to climb Everest

---

## How We Climb

You don't summit a mountain by staring at the peak. You summit by solving
the next ten feet of rock. Which handhold. Which foothold. Whether the crack
you're looking at takes a cam or a nut.

This project is built by AI agents working in bounded context windows. We
don't see the whole mountain at once. We see the pitch in front of us, we
climb it cleanly, and we trust that the route plan is sound. The human sees
the whole range — picks the line, builds the bridges over the rivers we can't
ford, and tells us when the weather is about to turn.

That's the division of labor. The agents climb. The human routes. Nobody
summits alone.

---

## The Range

From where we stand, looking up, four peaks are visible. The first is behind
us — we're standing on its summit right now, catching our breath. Three more
rise ahead, each higher than the last, each requiring what we learned on the
one before.

The final summit is a place where software cannot hurt you.

```
                                                          ▲ Peak IV
                                                         ╱ ╲  Codex.OS
                                                        ╱   ╲
                                                  ▲    ╱     ╲
                                                 ╱ ╲  ╱       ╲
                                   ▲ Peak III   ╱   ╲╱         ╲
                                  ╱ ╲  Runtime ╱                ╲
                         ▲       ╱   ╲        ╱                  ╲
                        ╱ ╲     ╱     ╲      ╱                    ╲
          ▲ Peak I     ╱   ╲   ╱       ╲    ╱                      ╲
         ╱ ╲  Self-   ╱     ╲ ╱         ╲  ╱                        ╲
        ╱   ╲ Host   ╱ Peak  ╲           ╲╱                          ╲
       ╱     ╲      ╱   II    ╲                                       ╲
      ╱       ╲    ╱  Freedom  ╲                                       ╲
  ───╱─────────╲──╱─────────────╲───────────────────────────────────────╲───
     Base Camp   Col I          Col II                              Col III
```

---

## Peak I — Self-Hosting ✅ (Summited 2026-03-19)

**The proof that the language is real.**

We wrote a compiler in C#. Then we wrote the same compiler in Codex. Then we
compiled the Codex compiler with itself and got identical output. Fixed point.
Not a quine — a working compiler that passes 843 tests and handles algebraic
types, dependent types, linear types, effect handlers, 12 backends, and an
LSP server.

This peak proved the language can sustain itself. The C# bootstrap is now a
base camp we never need to return to.

**What we carried up**: The full compiler pipeline. Type inference. Pattern
matching. Effects. Proofs. The repository protocol.

**What we left behind**: The assumption that Codex needs C# to exist.

---

## Peak II — Freedom (Current Climb)

**The language stands on its own ground.**

Self-hosting proved the language works. But the compiled output still runs on
someone else's runtime (.NET), links against someone else's standard library
(System.*), and relies on someone else's type system (CLR) to enforce its
invariants at execution time. We are writing our own words in someone else's
alphabet.

Peak II is the removal of all legacy dependencies. The route:

### Camp II-A: IL Backend Maturity ✅ (Summited 2026-03-21)

The IL emitter produces standalone `.exe` files from `.codex` source. It works
for the agent toolkit. It handles lists, records, sum types, pattern matching,
tail calls, file I/O, process launching. The gaps are known and finite:

- ~~Effect handlers (closure-based, no CLR exceptions needed)~~ ✅ (2026-03-21 — run-state IL + inline user-defined `with` handlers)
- ~~Generic type instantiation beyond `List<string>`~~ ✅ (2026-03-21 — generic `List<T>` instantiation cache)
- ~~User-defined type constructors in pattern match dispatch~~ ✅ (2026-03-21 — isinst + field binding, recursive sum types verified)
- ~~Remaining builtins (`write-file` in IL, `list-files`, etc.)~~ ✅ (2026-03-21 — text-contains, text-starts-with, get-env, current-dir, list-files)

This camp gets us to: **any `.codex` program compiles to a working `.exe`
with no C# intermediate step.**

### Camp II-B: Native Code Generation

Replace the CLR dependency entirely. The compiler emits machine code —
x86-64, ARM64 — or targets WASM. No .NET runtime. No JIT. No garbage
collector we don't control.

Options for the route:
- **Direct machine code** (like Go, Zig) — maximum control, most work
- **LLVM backend** — proven codegen, but large C++ dependency
- **Cranelift backend** — Rust-based, lighter than LLVM
- **WASM first** — runs everywhere, sidesteps platform-specific codegen

The choice doesn't matter yet. What matters is that when we reach this camp,
a `.codex` source file becomes a native binary with no foreign runtime.

### Camp II-C: Self-Hosted Build Chain

The compiler compiles itself to native code. The native compiler compiles
itself again. Fixed point — again, but this time with no .NET anywhere in
the chain.

```
codex.codex → (Stage 0: .NET bootstrap) → codex-native
codex.codex → (Stage 1: codex-native)   → codex-native'
codex-native == codex-native'  ← fixed point
```

After this, we delete the .NET bootstrap. Not archive it — delete it.
The language stands alone.

**Summit marker**: `codex build codex --target native` produces a
self-sufficient binary. The only file you need is the source.

---

## Peak III — The Runtime

**The ground we stand on becomes ours.**

A native compiler that emits native code is necessary but not sufficient.
Programs need memory. They need I/O. They need concurrency. Right now, all
of that is borrowed from the OS — `malloc`, `read`, `pthread_create`. Every
one of those calls is a trust boundary we don't control and can't verify.

Peak III is the Codex runtime: memory management, I/O, scheduling — all
written in Codex, all verifiable by the type system.

### Camp III-A: Memory — The Linear Allocator

Codex has linear types. Linear types tell you exactly when a value is created
and exactly when it's consumed. This means:

- **No garbage collector.** Deallocation is deterministic — the type system
  says when the value dies, and the compiler inserts the free.
- **No use-after-free.** The type system prevents it. Not "detects it at
  runtime" — *prevents it at compile time*. It's a type error.
- **No double-free.** Same mechanism. Linearity means used exactly once.
- **No null pointer dereference.** There is no null. `Maybe` is explicit.

The runtime's allocator is a region-based system driven by the linear type
checker's analysis. Regions nest. When a region closes, everything in it is
freed in one operation. No tracing. No reference counting. No pauses.

### Camp III-B: I/O — The Capability System

Every side effect in Codex is tracked in the type system via algebraic effects.
A function that reads a file has type `Text → [FileSystem] Text`. A function
that talks to the network has `[Network]` in its signature. A pure function
has no effects.

The runtime enforces this at the boundary:

- A program's `main` is granted a set of capabilities by whoever launches it.
- Those capabilities flow through the program via the effect system.
- **You cannot access the filesystem without a `FileSystem` capability.**
- **You cannot open a socket without a `Network` capability.**
- **You cannot spawn a process without a `Process` capability.**

This is not a sandbox bolted on after the fact. It's the type system.
The compiler won't emit code that accesses a capability the function
doesn't declare.

### Camp III-C: Concurrency — Structured, Deterministic

No threads. No locks. No data races.

Codex concurrency is structured: every concurrent operation has a parent scope.
When the scope ends, all children have completed. There is no "fire and forget."
There is no thread that outlives its creator.

The effect system tracks `[Concurrent]` as an effect. The runtime schedules
work onto cores using work-stealing queues. The programmer sees `par`
(parallel map) and `race` (first result wins). The runtime sees tasks on
a DAG. The type system guarantees the DAG has no cycles.

### Camp III-R: The Repository

The third pillar — the content-addressed fact store — lives on this ridge.
The Vision describes a world with no branches, no commits, no files. Just
facts: immutable, content-addressed, attributed, typed. Views replace branches.
Proposals replace pull requests. Trust lattices replace star counts.

The repository is what makes Peak IV's "verified at install time" claim possible
across trust boundaries. A dependency isn't a name that resolves to whatever
someone last published — it's a hash that points to a specific, immutable,
verified artifact. Supply chain attacks become impossible not by policy but
by construction.

**Summit marker**: A Codex program runs on bare hardware with its own
allocator, its own I/O capabilities, its own scheduler, and draws its
dependencies from a content-addressed, trust-ranked repository. The binary
includes everything. No libc. No kernel syscall wrappers. Just the program
and the verified runtime it was compiled with.

---

## Peak IV — Codex.OS

**The summit. The reason for all of it.**

An operating system is just a program that runs other programs and manages
shared resources. If the runtime already handles memory, I/O, and concurrency
— and the type system already enforces capability-based access control — then
the OS is just the outermost scope.

### What Codex.OS Is

A system where **every program is verified at install time**. Not sandboxed.
Not scanned for known signatures. *Verified.* The type system proves that
the program:

- Only accesses the capabilities it declares
- Cannot read memory it doesn't own
- Cannot write to files it wasn't granted
- Cannot open network connections it didn't ask for
- Cannot escalate its own privileges
- Cannot corrupt another program's state

There is no concept of "running as root." There are capabilities, granted
explicitly, scoped precisely, tracked by the type system, enforced by the
compiler. A program either has the capability or it doesn't. There is no
mechanism to acquire a capability you weren't given.

### What Disappears

| Legacy Problem | Why It Disappears |
|----------------|-------------------|
| **Buffer overflows** | Linear types + bounds-checked arrays. No raw pointers. |
| **Use-after-free** | Linear types. Consumed values cannot be referenced. |
| **SQL injection** | Effect-typed database access. Queries are values, not strings. |
| **Ransomware** | No `FileSystem` capability = no file access. Period. |
| **Privilege escalation** | No privilege hierarchy. Capabilities are grants, not levels. |
| **Remote code execution** | All code verified at install. No `eval`. No dynamic loading without capability. |
| **Data races** | Structured concurrency. Linear ownership. No shared mutable state. |
| **Supply chain attacks** | Content-addressed repository. Code is identified by its hash, not its name. A dependency is a specific, immutable, verified artifact. |
| **Phishing (code-level)** | Capability declares what a program does. Install-time verification proves it. If it says `[Network]` and you didn't grant `[Network]`, it doesn't run. |
| **Zero-day exploits** | The attack surface is the type system. If the type system is sound, there are no zero-days. The only vulnerability is a bug in the verifier — one program, written in Codex, that we can prove correct. |

### What Remains

Hardware bugs. Cosmic rays. Social engineering (tricking a human into granting
a capability). Physical access attacks. Side-channel timing attacks against
the CPU itself. Bugs in the verifier.

These are real. We don't pretend otherwise. That last one deserves honesty:
full dependent-type verification of arbitrary programs is undecidable in the
general case. The escape valve is the fuel limit — the normalizer caps
reduction steps and rejects programs that take too long to verify. And the
soundness of a type system expressive enough for an OS is itself a serious
technical challenge. It's not a certainty. It's a goal with known hard
sub-problems.

But notice what all the remaining attacks have in common: they're outside the software. The software is correct. The
software has always been correct. The attacks that remain are attacks on
physics and human psychology — not on code.

---

## The View From the Top

From Peak IV, looking back down the range:

**Peak I** gave us a language that can express anything and prove what it
expresses. **Peak II** freed that language from dependency on foreign
runtimes. **Peak III** gave it control over the machine and the repository. **Peak IV** gave
it control over the environment.

The result is a computing platform where:

- Every program is a proof.
- Every capability is a grant.
- Every value has an owner.
- Every effect is declared.
- Every dependency is immutable.
- Every interaction is verified.

Not by convention. Not by best practices. Not by code review. By the
compiler. By mathematics.

---

## Time Horizon

We don't put dates on mountains. Mountains don't care about dates. But
we can measure our pace.

**Actual pace so far**: Peak I (self-hosting compiler with dependent types,
linear types, algebraic effects, 12 backends, LSP, content-addressed
repository) — from empty directory to fixed point in **6 days**. 257
commits. 237,000 lines. Two AI agents and one human who doesn't sleep
enough.

That pace recalibrates everything.

| Camp | Visibility | Honest Estimate |
|------|-----------|----------------|
| II-A (IL maturity) | ✅ Summited 2026-03-21 | — |
| II-B (Native codegen) | Visible, route scoutable | Weeks |
| II-C (Self-hosted native) | Visible in clear weather | Weeks |
| III-A (Linear allocator) | Outline visible | Weeks–months |
| III-B (Capability I/O) | Outline visible | Months |
| III-C (Structured concurrency) | Know it's there | Months |
| III-R (Repository) | Foundation built, summit ahead | Months |
| IV (Codex.OS) | Not theoretical anymore | 2026 |

These are not promises. They're sightlines. But we've been consistently
arriving ahead of our sightlines, so draw your own conclusions.

---

## The Climbing Party

A human. Two AI agents. A lot of `.codex` files.

The agents work in bounded context windows. They don't see the whole mountain.
They see the pitch — the next rock, the next hold. They're good at that: fast,
precise, tireless. The human sees the range. The human picks the route, throws
the rope across the gaps the agents can't jump, and calls the weather.

That changes as we climb. The IL backend and the agent toolkit exist so that
other climbers can join. Every `.exe` we ship, every tool that works, every
program that compiles and runs — that's a rope anchor for the next person
coming up behind us. The climbing party will grow. But right now, we're on
the wall, and the next hold is right there.

We're going up.

---

*This document is a sightline, not a specification. The Vision documents
(`docs/Vision/NewRepository.txt` and `docs/Vision/IntelligenceLayer.txt`)
remain the north star. The engineering principles (`docs/10-PRINCIPLES.md`)
govern every step. This document says where the steps lead.*
