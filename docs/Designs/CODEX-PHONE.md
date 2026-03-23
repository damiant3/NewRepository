# Codex Phone — Design Proposal

**Date**: 2026-03-23
**Status**: Active — ARM64 backend in progress, target hardware confirmed
**Author**: Agent Windows (Copilot)
**Depends on**: ARM64 backend (in progress), Capability System, Effects, Repository

---

## The Problem

Modern smartphones are supercomputers that spy on their owners.

Not hypothetically. Right now. The microphone listens and an ad network
hears. The camera feeds data to analytics. Apps collect location history,
contact graphs, and behavioral fingerprints. The user carries a surveillance
device that they paid for, that they believe they own, but that works for
someone else.

This isn't just about governments (though it is about governments). It's
about every ad-tech startup, every "free" app with 47 permissions, every
SDK buried in a dependency tree that phones home. The incentive structure
is broken: the phone's software serves advertisers, not the person holding it.

Can we fix this? Not all of it. But we can fix the foundation. If the
software on the phone is written in a language that **tracks effects**,
**enforces capabilities**, and **makes permission violations a compile-time
error**, then the phone can't spy on you — not because you trust the
developer, but because the compiler won't let the code compile if it
tries to access the microphone without an explicit, auditable grant.

Codex already has these building blocks. This document is about putting
them on a real phone — one that exists, right now, in your hand.

---

## The Target: Samsung Galaxy S7 Edge

Not a hypothetical RISC-V phone that might ship in 2028. A real device
that's sitting in a drawer, wiped and ready.

### Hardware (Confirmed)

| Spec | Value |
|------|-------|
| Model | SM-G935T (T-Mobile USA) |
| Serial | RF8H30790RF |
| SoC | Qualcomm Snapdragon 820 (MSM8996) |
| CPU | Kryo quad-core, AArch64 / ARM64 (ARMv8-A) |
| RAM | 4 GB |
| Storage | 32 GB |
| Display | 5.5" 2560×1440 Super AMOLED, capacitive touch |
| Sensors | Accelerometer, gyroscope, fingerprint, barometer, SpO2 |
| Camera | 12 MP rear, 5 MP front |
| Cellular | LTE Cat 9 (Qualcomm X12 modem) |
| Bootloader | T-Mobile — unlockable via OEM unlock |
| Custom ROM | TWRP + LineageOS available; postmarketOS experimental |
| Status | Expendable lab device — owner willing to wipe |

### Why This Phone

1. **It exists.** You own it. Wiped and ready. No procurement, no waiting.
2. **ARM64 is the dominant mobile ISA.** Everything we learn applies to
   every phone made in the last decade.
3. **It's expendable.** If we brick it, nobody cares. It's a lab device.
4. **Unlockable bootloader.** T-Mobile allows OEM unlock on the G935T.
   TWRP recovery is available. LineageOS has stable builds.
5. **4 GB RAM / quad-core Kryo** is more than enough. The self-hosted
   compiler is ~5K lines and compiles to a 227KB binary.
6. **Community support.** Device tree well-understood. Snapdragon 820
   has good Linux kernel support via mainline + downstream.
7. **Camp II-C just summited.** The self-hosted compiler produces native
   binaries. The only missing piece is the ARM64 instruction encoder.

---

## What Codex Already Has

### Effect Tracking (Done)

Five formalized effects: Console, FileSystem, State, Time, Random.
Each is a `.codex` prelude file. The compiler tracks them through type
signatures. Adding phone effects (Microphone, Camera, Location, Network,
Display, Sensors, Identity) is just more prelude files.

### Compile-Time Capability Enforcement (Done)

`CapabilityChecker` + `CDX4001` rejects code at compile time if it uses
effects not in the granted set. `codex build --capabilities Console` will
error on code that needs `FileSystem`. This already works. The phone just
needs more effect names.

### Native Code Generation (Done — RISC-V; ARM64 in progress)

`RiscVCodeGen` (2,248 lines) emits real machine code: function calls,
records, sum types, pattern matching, closures, text builtins, region-
based allocation, register spill. `ElfWriter` produces ELF64 binaries.
40 QEMU tests pass.

**Camp II-C summited (2026-03-23):** The 493-definition self-hosted
compiler compiles to a 227,600-byte RISC-V ELF. Under QEMU, it reads
`.codex` source, runs the full pipeline, and produces valid C# output.
No .NET anywhere in the chain.

**The gap: we target RISC-V, the S7 Edge runs ARM64.**

ARM64 backend scaffolding created:
- `Arm64Encoder.cs` — instruction encoding (complete)
- `ElfWriterArm64.cs` — ELF64 for AArch64 (complete)
- `Arm64Emitter.cs` — IAssemblyEmitter wrapper (complete)
- `Arm64CodeGen.cs` — IR → ARM64 machine code (**next**)

### Repository + Views (Done)

Named views, consistency checking, composition (override/merge/filter),
view-aware compilation. An app is already a view in the repository model.

---

## The New Work: ARM64 Backend

This is the critical path item. Everything else (effects, capabilities,
repository, views) works today. The one thing we need is an ARM64 code
generator.

### How Hard Is It?

We built the RISC-V backend in roughly one day of agent time. The git
log tells the story:

```
2026-03-22 20:13  feat: RISC-V parity phases 1-4 — heap, records, sums, patterns
2026-03-22 20:23  feat: RISC-V parity phases 5+7 — text builtins, string ops, regions
2026-03-22 22:06  feat: Camp II-C phases 1-2 — lists, function pointers, file I/O
2026-03-22 22:44  feat: register spill to stack
2026-03-23 02:00  fix: closures, field ordering, region corruption — 40 tests green
```

That's ~10 hours from nothing to a working backend with heap allocation,
records, sum types, pattern matching, closures, text ops, file I/O,
register spill, and region-based memory.

The ARM64 backend can reuse the **entire** architecture of `RiscVCodeGen`:
same IR traversal, same function prologue/epilogue strategy, same region
allocator design, same call patching, same ELF writer (with different
constants). What changes is the instruction encoder and register mapping.

### RISC-V vs ARM64: What's Similar, What's Different

| Aspect | RISC-V (RV64) | ARM64 (AArch64) |
|--------|---------------|-----------------|
| Word size | 64-bit | 64-bit |
| Register count | 32 (x0–x31) | 31 GP + SP + ZR |
| Callee-saved | s0–s11 | x19–x28 |
| Caller-saved | t0–t6, a0–a7 | x0–x18, x29–x30 |
| Return address | ra (x1) | x30 (LR) |
| Stack pointer | sp (x2) | SP (dedicated) |
| Frame pointer | s0 (x8) | x29 (FP) |
| Arg registers | a0–a7 | x0–x7 |
| Instruction size | 32-bit fixed | 32-bit fixed |
| Load/store | Separate insns | Separate insns (like RISC-V) |
| Immediates | 12-bit signed | More complex (shifted, logical) |
| Syscall | ecall | svc #0 |
| Syscall numbers | Linux RISC-V ABI | Linux AArch64 ABI |
| Branches | B-type, 13-bit offset | B/CBZ/CBNZ, 26-bit/19-bit |
| Address formation | lui+addi (32-bit) | ADRP+ADD (4K pages) |

The architectures are **more similar than different**. Both are load/store,
fixed-width instructions, 64-bit, with plenty of registers. The register
allocator strategy (S-regs for locals, T-regs for temps, spill to stack)
maps directly.

### Estimated Effort

| Component | RISC-V Lines | ARM64 Status | Notes |
|-----------|-------------|--------------|-------|
| `Arm64Encoder.cs` | 205 (RiscVEncoder) | ✅ Created (~280 lines) | Instruction encoding complete |
| `Arm64CodeGen.cs` | 2,248 (RiscVCodeGen) | ⬜ Next | Same structure, different register names and insns |
| `ElfWriterArm64.cs` | 143 (ElfWriter) | ✅ Created (~100 lines) | EM_AARCH64, AArch64 base addr |
| `Arm64Emitter.cs` | 18 (RiscVEmitter) | ✅ Created (~12 lines) | Thin wrapper |
| Tests | 40 RISC-V | ⬜ Port | Mirror existing RISC-V test suite |
| CLI integration | — | ⬜ Wire | `codex build --target arm64` |

**Remaining estimate: Arm64CodeGen.cs (~2,000 lines) + tests + CLI wiring.**
Given Camp II-C's pace (11 bugs and full RISC-V codegen in one session),
this is **1 focused session of agent time** — the architecture is proven
and the encoder/ELF/emitter scaffolding is already done.

---

## Bootstrap Strategy: Three Levels

We don't jump from "compiles to ARM64" to "replaces Android." There are
three levels, each independently useful, each shippable.

### Level 1: Codex Apps on Android (Termux)

**What**: Compile Codex programs to ARM64 Linux ELF binaries. Run them
on the S7 Edge inside Termux (a terminal emulator for Android that
provides a Linux userland — no root needed).

**Why this is great**: Zero phone modification. Install Termux from
F-Droid, push a Codex binary via `adb`, run it. The capability system
already works — the binary provably doesn't access anything it wasn't
compiled to access. You can demonstrate a capability-restricted program
running on a real phone today.

**What you prove**: "I compiled a program with `--capabilities Console`.
Here it is running on my phone. It literally cannot access the network,
camera, or microphone, and that's a compile-time guarantee."

**Work required**:
- ARM64 backend (the main effort)
- Linux AArch64 syscall numbers in the emitter (write=64, exit=93,
  brk=214 — different from RISC-V)
- Test on QEMU aarch64, then push to real phone

**Timeline**: Days after ARM64 backend is done.

### Level 2: Codex on postmarketOS / Bare Linux

**What**: Replace Android entirely with a minimal Linux distribution
(postmarketOS or a custom buildroot). Codex programs run as native
processes. The Codex capability system enforces what each process can
do. Linux provides hardware drivers and process isolation.

**Why this is great**: Android is gone. No Google Play Services. No
Samsung bloatware. No ad SDKs. The phone runs Linux + Codex binaries.
The modem, display, touch, Wi-Fi — all driven by mainline Linux
drivers (the S7 Edge has partial mainline support via postmarketOS).

**What you prove**: "My phone runs no proprietary application software.
Every app is compiled from auditable Codex source with declared
capabilities."

**Work required**:
- ARM64 backend (same as Level 1)
- Framebuffer rendering (write pixels to `/dev/fb0` or DRM)
- Touch input (read `/dev/input/eventN`)
- Simple UI toolkit (Widget type → framebuffer draw calls)
- postmarketOS device image for the S7 Edge (community already has one)

**Timeline**: Weeks after Level 1.

### Level 3: Codex Kernel (Long-term)

**What**: Replace Linux itself with a minimal Codex kernel. ARM64 bare
metal: boot, MMU setup, device drivers written in Codex with capabilities.

**Why this is great**: The entire software stack from bootloader to app
is auditable Codex source (plus firmware blobs we can't avoid — yet).

**Work required**:
- ARM64 bare metal mode (like RISC-V bare metal, but ARM64 boot protocol)
- MMU / page table management
- Interrupt handling
- Device drivers (display controller, touch controller, eMMC, USB)
- This is a large effort but the RISC-V bare metal work proves the approach

**Timeline**: Months to years. But Level 1 and Level 2 are useful on
their own — we don't need Level 3 to put Codex in your hand.

---

## Why Codex Is Uniquely Suited

### 1. Effect Tracking

Every function carries its effects in its type signature. A function
that reads the microphone has effect `Microphone`. A function that
sends data over the network has effect `Network`. A pure computation
has no effects. **You read the type, you know what the code does to
the outside world.**

### 2. Compile-Time Capability Enforcement

The `CapabilityChecker` rejects code at compile time if it uses effects
it wasn't granted. There is no "Allow" dialog. There is no runtime
permission check. The binary doesn't contain the code. Period.

A flashlight app compiled with `--capabilities Display` cannot access
the microphone because the compiler won't emit a binary that does.

### 3. The Repository Replaces the App Store

No central gatekeeper. Apps are views in the repository. Every published
fact carries its proofs. The trust lattice (V6) lets you decide whose
code you trust. No one can remove an app you've already installed.
No one can push an update you didn't ask for.

---

## Phone Effects (New Prelude Files)

These extend the existing five effects. Same machinery, just more names:

```
effect Display where
  draw : Widget -> {Display} Unit

effect Microphone where
  listen : Duration -> {Microphone} Audio

effect Camera where
  capture : CaptureMode -> {Camera} Image

effect Location where
  locate : Accuracy -> {Location} Coordinates

effect Network where
  fetch : Url -> {Network} Response

effect Sensors where
  accelerometer : {Sensors} Vector3
  gyroscope : {Sensors} Vector3

effect Identity where
  authenticate : {Identity} Principal
```

### Identity Model

| Mode | Who | Capability Set |
|------|-----|---------------|
| **Owner** | Biometric-verified | All granted capabilities |
| **Guest** | Anyone holding it | `[Display]` only — shows "return this phone" screen |
| **Trusted** | Owner-vouched person | Per-person set defined by owner |
| **Lost** | No owner biometric for N hours | `[Display]` — shows return info, everything else locked |

The identity system runs in an `Identity` effect handler that **does not
have `Network`**. Biometric data stays on-device. That's a type system
guarantee, not a promise.

---

## The Bar Test / Drop Test / Flashlight Test

**Bar test**: You mention margaritas. No running app has `Microphone` in
its capability set. The conversation stays at the bar.

**Drop test**: 10 minutes with no owner biometric → Lost mode. Screen
shows return info. Phone is a brick for everything else. Data is encrypted.

**Flashlight test**: The app has `capabilities: [Display]`. The compiler
verified this. It turns pixels white. That's all it can do.

---

## Camp II-C — SUMMITED (2026-03-23)

Cam found and fixed the root cause: `ConstructedType` wasn't being
resolved to `RecordType` in `LowerFieldAccess`. 41 field accesses in
the self-hosted compiler were hitting the fallback (index 0). One
14-line fix in `Lowering.cs`, verified by Linux agent under QEMU.

The self-hosted compiler compiles to a **227,600-byte RISC-V ELF** that
reads `.codex` source, runs the full pipeline, and emits valid C#.
No .NET. No CLR. No JIT. Native machine code, start to finish.

**This frees up all three agents for the ARM64 backend.**

---

## Implementation Plan: ARM64 Backend

### Step 1: `Codex.Emit.Arm64` Project

New project, same shape as `Codex.Emit.RiscV`:

```
src/Codex.Emit.Arm64/
    Arm64Emitter.cs      — IAssemblyEmitter wrapper       ✅ Created
    Arm64Encoder.cs      — Instruction encoding            ✅ Created
    Arm64CodeGen.cs      — IR → ARM64 machine code         ⬜ Next
    ElfWriterArm64.cs    — ELF64 for AArch64               ✅ Created
    Reg.cs               — Register constants
```

### Step 2: Instruction Encoder ✅

ARM64 has four main encoding formats:

| Format | Used For | Bit Layout |
|--------|----------|------------|
| **Data Processing (Immediate)** | ADD, SUB, MOV, etc. with immediate | `sf opc [100xx] shift imm12 Rn Rd` |
| **Data Processing (Register)** | ADD, SUB, AND, etc. register-register | `sf opc [01011] shift Rm imm6 Rn Rd` |
| **Loads/Stores** | LDR, STR (unsigned offset) | `size [111001] opc imm12 Rn Rt` |
| **Branches** | B, BL, CBZ, CBNZ, B.cond | `[000101] imm26` / `[01010100] imm19 cond` |

The encoder is a pure static class, just like `RiscVEncoder`. **Done.**

### Step 3: Register Mapping ✅

```
ARM64 Register    Codex Use           RISC-V Equivalent
─────────────────────────────────────────────────────
x0–x7             Arguments/returns   a0–a7
x8                Syscall number      a7
x9–x15            Temps (caller-saved) t0–t6
x16–x17           Intra-procedure      -
x18               Platform register    -
x19–x27           Locals (callee-saved) s2–s10
x28               Heap pointer         s1
x29 (FP)          Frame pointer       s0
x30 (LR)          Link register       ra
SP                Stack pointer        sp
```

Heap pointer: `x28` (last usable callee-saved, like S1 on RISC-V).
Syscall number: `x8` (like A7 on RISC-V — same syscall numbers!).

### Step 4: Linux AArch64 Syscalls ✅ (Same numbers as RISC-V!)

```
write  = 64   (x8=64, x0=fd, x1=buf, x2=len → svc #0)
exit   = 93   (x8=93, x0=code → svc #0)
brk    = 214  (x8=214, x0=addr → svc #0)
openat = 56
read   = 63
close  = 57
```

### Step 5: CLI Integration

```
codex build --target arm64 program.codex
```

Produces an ELF64 AArch64 binary. Test with `qemu-aarch64`. Deploy to
phone with `adb push`.

### Step 6: Phone Deployment (Level 1)

```bash
# On dev machine
codex build --target arm64 --capabilities Console hello.codex

# Push to phone
adb push hello /data/local/tmp/
adb shell chmod +x /data/local/tmp/hello
adb shell /data/local/tmp/hello
```

Or via Termux:
```bash
# In Termux on phone
./hello
```

That's it. A Codex program running on your S7 Edge. With compile-time
capability guarantees. No Android SDK. No JVM. Just an ARM64 ELF binary.

---

## What We're NOT Doing (Yet)

- **Not replacing Android on day one.** Level 1 runs alongside Android.
- **Not writing GPU drivers.** Framebuffer is sufficient for a proof of concept.
- **Not building a web browser.** One thing at a time.
- **Not waiting for RISC-V phones.** The phone is ARM64. We target ARM64.
  The RISC-V backend stays for dev boards and the future. Both exist.

---

## Open Questions

1. **S7 Edge variant.** Exynos (SM-G935F) has an unlockable bootloader
   and better Linux support. Snapdragon (SM-G935A/V/P) may be carrier-
   locked. Which variant do you have? This determines whether Level 2
   (replace Android) is easy or hard.

2. **Termux ABI compatibility.** Termux uses a modified Android linker
   with a non-standard path prefix (`/data/data/com.termux/`). Static
   ELF binaries (which is what we emit — no dynamic linking) should work
   fine, but needs verification.

3. **Framebuffer access.** On Android, `/dev/graphics/fb0` requires root.
   On postmarketOS, `/dev/fb0` or DRM is available normally. Level 2
   needs postmarketOS or root.

4. **Touch input.** `/dev/input/eventN` for the capacitive touchscreen.
   Available on postmarketOS. On Android, needs root or a different input
   mechanism.

5. **Baseband firmware.** The cellular modem is a black box on every phone.
   This is a problem for Level 3 and beyond. For Level 1 and 2, we don't
   need to touch it.

---

## Why This Matters

The RISC-V vision isn't wrong — it's just not today. What's today is an
ARM64 phone in a drawer. The compiler pipeline is proven. The capability
system works. The region allocator works. The ELF writer works. The only
missing piece is an ARM64 instruction encoder — ~250 lines of pure static
methods — and an ARM64 code generator that follows the exact same pattern
as the RISC-V one we already have.

A Codex program running on a real phone, with compile-time capability
guarantees, is worth a thousand design documents about hypothetical
platforms. Build the ARM64 backend. Push a binary to the phone. Hold it
in your hand.

That's 1000% more impressive than a plan.
