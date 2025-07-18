using ASTRALib;

namespace ConnectionRastr.Expansion
{
    internal static class ResponseCodeExtension
    {
        public static ResponseCode MapCode(RastrRetCode code)
            => code switch
            {
                RastrRetCode.AST_OK => ResponseCode.Success,
                RastrRetCode.AST_NB => ResponseCode.BalancingFailed,
                RastrRetCode.AST_REPT => ResponseCode.WeightingDone,
                RastrRetCode.AST_CONTROL => ResponseCode.NoNe,
                RastrRetCode.AST_NBAP => ResponseCode.NoNe,
                RastrRetCode.AST_UB => ResponseCode.NoNe,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(code),
                    $"$Значение {code} неподдерживается."
                )
            };
    }
}
