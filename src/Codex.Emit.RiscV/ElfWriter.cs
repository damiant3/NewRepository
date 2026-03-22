namespace Codex.Emit.RiscV;

/// Writes a minimal ELF64 executable for Linux RISC-V.
/// No section headers, no symbol table, no relocations.
/// Just headers + loadable segments.
sealed class ElfWriter
{
    // ELF constants
    const byte ELFCLASS64 = 2;
    const byte ELFDATA2LSB = 1;   // little-endian
    const ushort EM_RISCV = 243;
    const ushort ET_EXEC = 2;
    const uint PT_LOAD = 1;
    const uint PF_X = 1;
    const uint PF_W = 2;
    const uint PF_R = 4;

    // Layout constants
    const ulong BaseAddress = 0x10000;    // standard Linux user-space base
    const int ElfHeaderSize = 64;
    const int ProgramHeaderSize = 56;

    public static byte[] WriteExecutable(byte[] textSection, byte[] rodataSection, ulong entryOffset)
    {
        // Layout:
        // [ELF header: 64 bytes]
        // [Phdr 0 (text): 56 bytes]   — code, r-x
        // [Phdr 1 (rodata): 56 bytes] — data, r--
        // [.text bytes]
        // [.rodata bytes]
        //
        // We pack text right after headers. Rodata follows text.

        int headersTotalSize = ElfHeaderSize + ProgramHeaderSize * 2;

        // Align text to 16 bytes after headers
        int textFileOffset = Align(headersTotalSize, 16);
        ulong textVaddr = BaseAddress + (ulong)textFileOffset;
        ulong entryPoint = textVaddr + entryOffset;

        int rodataFileOffset = Align(textFileOffset + textSection.Length, 16);
        ulong rodataVaddr = BaseAddress + (ulong)rodataFileOffset;

        int totalSize = rodataFileOffset + rodataSection.Length;

        MemoryStream ms = new(totalSize);
        BinaryWriter w = new(ms);

        // ── ELF Header (64 bytes) ──────────────────────────────
        // e_ident[0..3]: magic
        w.Write((byte)0x7F); w.Write((byte)'E'); w.Write((byte)'L'); w.Write((byte)'F');
        // e_ident[4]: class
        w.Write(ELFCLASS64);
        // e_ident[5]: data encoding
        w.Write(ELFDATA2LSB);
        // e_ident[6]: version
        w.Write((byte)1);
        // e_ident[7]: OS/ABI (Linux)
        w.Write((byte)0);
        // e_ident[8..15]: padding
        w.Write(new byte[8]);

        // e_type: executable
        w.Write(ET_EXEC);
        // e_machine: RISC-V
        w.Write(EM_RISCV);
        // e_version
        w.Write((uint)1);
        // e_entry: entry point virtual address
        w.Write(entryPoint);
        // e_phoff: program header offset
        w.Write((ulong)ElfHeaderSize);
        // e_shoff: section header offset (0 = none)
        w.Write((ulong)0);
        // e_flags: RISC-V flags (RVC + double-float ABI = 0x0005 for RV64GC, 0x0004 for double-float)
        w.Write((uint)0x0004);
        // e_ehsize
        w.Write((ushort)ElfHeaderSize);
        // e_phentsize
        w.Write((ushort)ProgramHeaderSize);
        // e_phnum
        w.Write((ushort)2);
        // e_shentsize
        w.Write((ushort)0);
        // e_shnum
        w.Write((ushort)0);
        // e_shstrndx
        w.Write((ushort)0);

        // ── Program Header 0: .text (r-x) ─────────────────────
        w.Write(PT_LOAD);                     // p_type
        w.Write(PF_R | PF_X);                 // p_flags
        w.Write((ulong)textFileOffset);        // p_offset
        w.Write(textVaddr);                    // p_vaddr
        w.Write(textVaddr);                    // p_paddr
        w.Write((ulong)textSection.Length);     // p_filesz
        w.Write((ulong)textSection.Length);     // p_memsz
        w.Write((ulong)16);                    // p_align

        // ── Program Header 1: .rodata (r--) ───────────────────
        w.Write(PT_LOAD);                      // p_type
        w.Write(PF_R);                         // p_flags
        w.Write((ulong)rodataFileOffset);       // p_offset
        w.Write(rodataVaddr);                   // p_vaddr
        w.Write(rodataVaddr);                   // p_paddr
        w.Write((ulong)rodataSection.Length);    // p_filesz
        w.Write((ulong)rodataSection.Length);    // p_memsz
        w.Write((ulong)16);                     // p_align

        // ── Pad to text offset ─────────────────────────────────
        while (ms.Position < textFileOffset)
            w.Write((byte)0);

        // ── .text ──────────────────────────────────────────────
        w.Write(textSection);

        // ── Pad to rodata offset ───────────────────────────────
        while (ms.Position < rodataFileOffset)
            w.Write((byte)0);

        // ── .rodata ────────────────────────────────────────────
        w.Write(rodataSection);

        return ms.ToArray();
    }

    /// Returns the virtual address where .rodata starts.
    /// Used by the code generator to compute string literal addresses.
    public static ulong ComputeRodataVaddr(int textSize)
    {
        int headersTotalSize = ElfHeaderSize + ProgramHeaderSize * 2;
        int textFileOffset = Align(headersTotalSize, 16);
        int rodataFileOffset = Align(textFileOffset + textSize, 16);
        return BaseAddress + (ulong)rodataFileOffset;
    }

    static int Align(int value, int alignment)
    {
        int remainder = value % alignment;
        return remainder == 0 ? value : value + (alignment - remainder);
    }
}
