using ASTRALib;


namespace ConnectionRastr
{
    internal static class LoadCodeExtension
    {
        public static RG_KOD ToRgKod(this LoadCode code)
            => code switch
            {
                LoadCode.Add => RG_KOD.RG_ADD,
                LoadCode.Load => RG_KOD.RG_REPL,
                LoadCode.Update => RG_KOD.RG_KEY,
                LoadCode.Join => RG_KOD.RG_KEYADD,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(code),
                    $"$Значение {code} неподдерживается."
                )
            };
    }
}
