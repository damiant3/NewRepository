using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Codex.Types;

namespace Codex.Emit.IL;

sealed class LocalsBuilder
{
    readonly MetadataBuilder m_metadata;
    readonly Action<SignatureTypeEncoder, CodexType> m_encodeType;
    readonly List<(string Name, CodexType Type)> m_locals = [];

    public int Count => m_locals.Count;

    public LocalsBuilder(MetadataBuilder metadata, Action<SignatureTypeEncoder, CodexType> encodeType)
    {
        m_metadata = metadata;
        m_encodeType = encodeType;
    }

    public int AddLocal(string name, CodexType type)
    {
        int index = m_locals.Count;
        m_locals.Add((name, type));
        return index;
    }

    public bool TryGetLocal(string name, out int index)
    {
        for (int i = m_locals.Count - 1; i >= 0; i--)
        {
            if (m_locals[i].Name == name)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }

    public StandaloneSignatureHandle BuildSignature()
    {
        BlobBuilder sig = new();
        BlobEncoder encoder = new(sig);
        LocalVariablesEncoder localsEncoder = encoder.LocalVariableSignature(m_locals.Count);
        foreach ((string _, CodexType type) in m_locals)
        {
            LocalVariableTypeEncoder localEncoder = localsEncoder.AddVariable();
            m_encodeType(localEncoder.Type(), type);
        }
        return m_metadata.AddStandaloneSignature(m_metadata.GetOrAddBlob(sig));
    }
}
