using ASTRALib;

using static ConnectionRastr.Expansion.CalcSetting;

namespace ConnectionRastr.Expansion
{
    public static class WeightingExtension
    {
        private const string CONTROLLED_DESCRIPTION = "ots_val";
        private const string CONTROLLED_DESCRIPTION_NUMBER = "Num";
        private const string CONTROLLED_DESCRIPTION_NAME = "name";
        private const string CONTROLLED_DESCRIPTION_TYPE = "tip";
        private const string CONTROLLED_DESCRIPTION_TABLE = "tabl";
        private const string CONTROLLED_DESCRIPTION_QUERY = "vibork";
        private const string CONTROLLED_DESCRIPTION_FORMULA = "formula";
        private const string CONTROLLED_DESCRIPTION_SCALE = "mash";

        private const string CONTROLLED_VALUES = "ots_znach";
        private const string CONTROLLED_VALUES_NUMBER = "Num";
        private const string CONTROLLED_VALUES_NAME = "name";

        public static int CountControlledStates(
            this RastrWrapper provider
        ) => provider.Tables[CONTROLLED_VALUES].Count;

        public static void SelectStates(
            this RastrWrapper provider,
            int number
        )
        {
            Table description = provider.Tables[CONTROLLED_DESCRIPTION];
            Table values = provider.Tables[CONTROLLED_VALUES];

            var descriptions = new (
                int Number,
                string Name,
                string Table,
                string Query,
                string Column
            )[description.Count];
            for (int i = 0; i < description.Count; i++)
            {
                descriptions[i] = new(
                
                    (int)description.Columns[CONTROLLED_DESCRIPTION_NUMBER][i],
                    (string)description.Columns[CONTROLLED_DESCRIPTION_NAME][i],
                    (string)description.Columns[CONTROLLED_DESCRIPTION_TABLE][i],
                    (string)description.Columns[CONTROLLED_DESCRIPTION_QUERY][i],
                    (string)description.Columns[CONTROLLED_DESCRIPTION_FORMULA][i]
                );
            }

            foreach (var record in descriptions)
            {
                Table table = provider.Tables[record.Table];
                int index = table.SelectRow(record.Query);
                object value = values.Columns[$"O_{record.Number}"][number];
                table.Columns[record.Column][index] = value;
            }

            provider.RunRegimeCalculation();
        }

        public static void Weight(this RastrWrapper provider)
        {
            Rastr rastr = provider.Original;
            if (rastr.ut_Param[0] == 0)
            {
                rastr.ut_FormControl();
                rastr.ClearControl();
                RastrRetCode kod = rastr.step_ut("i");

                if (kod == 0)
                {
                    RastrRetCode kd;
                    do
                    {
                        kd = rastr.step_ut(""); // Пустая строка вместо "z" как в оригинале?

                        if ((kd == 0 && rastr.ut_Param[(ParamUt)1] == 0) 
                            || rastr.ut_Param[(ParamUt)2] == 1
                        )
                        {
                            rastr.AddControl(-1, "");
                        }
                    } while (kd == 0);
                }
            }
        }

        /// <summary>
        /// Инициирует расчет режима.
        /// </summary>
        /// <param name="settings">Параметры расчета.</param>
        /// <returns>Код расчета.</returns>
        public static ResponseCode RunRegimeCalculation(
            this RastrWrapper provider,
            CalcSetting settings = FlatStart
        ) => ResponseCodeExtension.MapCode(
            provider.Original.rgm(settings.ToStringCode())
        );
    }
}
