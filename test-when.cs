using System;
using System.Collections.Generic;
using System.Linq;

public sealed record UnifyResult(bool success, long state);

public abstract record CodexType;

public sealed record IntegerTy : CodexType;
public sealed record TextTy : CodexType;
public sealed record ErrorTy : CodexType;

public static class Codex_test_when
{
    public static UnifyResult unify_structural(long st, CodexType a, CodexType b)
    {
        return ((Func<CodexType, UnifyResult>)((_scrutinee_) => (_scrutinee_ is IntegerTy _mIntegerTy_ ? (b is IntegerTy _mIntegerTy_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => new UnifyResult(false, st)))(b)) : (_scrutinee_ is TextTy _mTextTy_ ? (b is TextTy _mTextTy_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => new UnifyResult(false, st)))(b)) : ((Func<CodexType, UnifyResult>)((_) => new UnifyResult(false, st)))(_scrutinee_)))))(a);
    }

}
