using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Codex.Types;

namespace Codex.Emit.IL;

sealed class LocalsBuilder
{
    readonly MetadataBuilder m_metadata;
    readonly List<(string Name, CodexType Type)> m_locals = new();

    public int Count => m_locals.Count;

    public LocalsBuilder(MetadataBuilder metadata)
    {
        m_metadata = metadata;
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
            EncodeLocalType(localEncoder.Type(), type);
        }
        return m_metadata.AddStandaloneSignature(m_metadata.GetOrAddBlob(sig));
    }

    static void EncodeLocalType(SignatureTypeEncoder encoder, CodexType type)
    {
        switch (type)
        {
            case IntegerType:
                encoder.Int64();
                break;
            case NumberType:
                encoder.Double();
                break;
            case TextType:
                encoder.String();
                break;
            case BooleanType:
                encoder.Boolean();
                break;
            default:
                encoder.Object();
                break;
        }
    }
}
