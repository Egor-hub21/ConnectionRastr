using ASTRALib;

namespace ConnectionRastr.Extension.Emergencies
{
    public static class Emergencies
    {
        const double ACCURACY = 0.001;
        public static int RunEmergencies(
            this RastrWrapper provider,
            out Array? resulr
        )
        {
            object? response = null;
            int value = provider.Original.Emergencies(ref response);
            if (response != null)
            {
                if (response is Array validResult)
                {
                    resulr = validResult;
                    return value;
                }

                resulr = null;
                return value;
            }

            resulr = null;
            return value;
        }

        public static int InstalMdpRegime(
            this RastrWrapper provider,
            int section
        ) => InstalRegime(provider, section, "itog_mdp");

        public static int InstalInitialRegime(
            this RastrWrapper provider,
            int section
        ) => InstalRegime(provider, section, "ptek");

        private static int InstalRegime(
            this RastrWrapper provider,
            int section,
            string tableName
        )
        {
            int indexResult = provider.Tables["ut_os_result"].SelectRow(
                $"ns={section}&nvir=0&pred=0&pmdp=0"
            );
            double overflow = (double)provider
                    .Tables["ut_os_result"]
                    .Columns[tableName][indexResult];

            int indexSection = provider
                .Tables["sechen"]
                .SelectRow($"ns={section}");

            Toggle toggle = provider.Original.GetToggle();

            int currentIndex = 1;
            int respons = 0;
            while (currentIndex != -2 && respons != -2)
            {
                respons = toggle.MoveOnPosition(currentIndex);

                bool check =
                    double.Abs(
                        (double)provider
                            .Tables["sechen"]
                            .Columns["psech"][indexSection]
                        - overflow
                    ) < ACCURACY;
                if (check)
                {
                    return currentIndex;
                }

                currentIndex++;
            }
            return -1;
        }
    }
}
