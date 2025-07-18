using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionRastr.Expansion
{
    internal static class CalcSettingExtension
    {
        public static string ToStringCode(this CalcSetting calcSetting)
            => calcSetting switch
            {
                CalcSetting.Default => "",
                CalcSetting.Initialize => "i",
                CalcSetting.FlatStart => "p",
                CalcSetting.NoStartAlgorithm => "z",
                CalcSetting.NoDataControl => "c",
                CalcSetting.NoDataPrep => "r",
                _ => throw new ArgumentOutOfRangeException(
                    nameof(calcSetting),
                    $"$Значение {calcSetting} неподдерживается."
                )
            };
    }
}
