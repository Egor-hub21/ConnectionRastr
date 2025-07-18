using ASTRALib;
using System.Reflection;

namespace ConnectionRastr.Expansion
{
    public static class VirExtension
    {
        private const string NODE = "node";
        private const string NODE_NUMBER = "ny";
        private const string NODE_STATE = "sta";

        private const string VIR_NODE = "ut_vir_node";
        private const string VIR_NODE_NUMBER = "Num";
        private const string VIR_NODE_LOAD_FACTOR = "Kl";
        private const string VIR_NODE_GENERATION_FACTORS = "Kg";
        private const string VIR_NODE_TANGENTS = "ConstTg";

        private const string GENERATOR = "Generator";
        private const string GENERATOR_NUMBER = "Num";
        private const string GENERATOR_STATE = "sta";
        private const string GENERATOR_NODE = "Node";

        private const string VIR_GENERATOR = "ut_vir_gen";
        private const string VIR_GENERATOR_NUMBER = "Num";
        private const string VIR_GENERATOR_GENERATION_FACTOR = "Kg";

        /// <summary>
        /// Получение траектории утяжеления по коэффициентам ВИР.
        /// </summary>
        /// <param name="vir">Номер ВИР.</param>
        /// <param name="multiplier">Множитель.</param>
        public static void ApplyVirTrajectory(
            this RastrWrapper provider,
            int vir,
            double multiplier
        )
        {
            List<RegimeChangeVectorGenerator> entriesVir = new();

            string searchTerm = $"!Act&NumVir={vir}";

            provider.AddVirTrajectoryNode(searchTerm, entriesVir);
            provider.AddVirTrajectoryGen(searchTerm, entriesVir);

            DivideSecondItemsBySum(entriesVir);

            MultiplyFactor(entriesVir, multiplier);

            // Очистит таблицу "Приращения_Узлы"
            provider.Tables["ut_node"].Clear();

            provider.FillUtFromVir(entriesVir);
        }
        private static void AddVirTrajectoryNode(
            this RastrWrapper provider,
            string searchTerm,
            List<RegimeChangeVectorGenerator> entriesVir
        )
        {
            string numberNode = "ny";
            string stateNode = "sta";

            Table nodeVir = provider.Tables["ut_vir_node"];
            Table node = provider.Tables["node"];

            Column numbers = nodeVir.Columns["Num"];
            Column loadFactors = nodeVir.Columns["Kl"];
            Column generationFactors = nodeVir.Columns["Kg"];
            Column tangents = nodeVir.Columns["ConstTg"];

            
            int[] indexes = nodeVir.SelectRows(searchTerm);

            foreach (var index in indexes)
            {
                int number = (int)numbers[index];

                int indexNode = node.SelectRow($"{number}={numberNode}"
                    + $"&!{stateNode}");

                if (indexNode != -1)
                {
                    entriesVir.Add(new RegimeChangeVectorNode(
                        number,
                        (double)loadFactors[index],
                        (double)generationFactors[index],
                        (bool)tangents[index]));
                }
            }
        }

        private static void AddVirTrajectoryGen(
            this RastrWrapper provider,
            string searchTerm,
            List<RegimeChangeVectorGenerator> entriesVir
        )
        {
            string numberGenerators = "Num";
            string stateGenerators = "sta";

            string numberNodes = "ny";
            string stateNodes = "sta";

            Table generatorsVir = provider.Tables["ut_vir_gen"];
            Table generators = provider.Tables["Generator"];
            Table nodes = provider.Tables["node"];

            Column numbers = generatorsVir.Columns["Num"];
            Column generationFactors = generatorsVir.Columns["Kg"];

            Column generatorNode = generators.Columns["Node"];

            int[] indexes = generatorsVir.SelectRows(searchTerm);

            foreach (var index in indexes)
            {
                int number = (int)numbers[index];

                int indexGenerator = generators.SelectRow(
                    $"{number}={numberGenerators}"
                    + $"&!{stateGenerators}");

                if (indexGenerator != -1)
                {
                    int indexNode = nodes.SelectRow(
                        $"{(int)generatorNode[indexGenerator]}={numberNodes}"
                        + $"&!{stateNodes}");

                    if (indexNode != -1)
                    {
                        entriesVir.Add(new RegimeChangeVectorGenerator
                            (number, (double)generationFactors[index]));
                    }
                }
            }
        }

        /// <summary>
        /// Сбалансированный ВИР.
        /// </summary>
        /// <param name="entriesVir">Список с данными о вир.</param>
        private static void DivideSecondItemsBySum(
            List<RegimeChangeVectorGenerator> entriesVir
        )
        {
            double sumPositive = 0;
            double sumNegative = 0;

            foreach (var entryVir in entriesVir)
            {

                if (entryVir.GeneratorFactor > 0)
                {
                    sumPositive += entryVir.GeneratorFactor;
                }
                else if (entryVir.GeneratorFactor < 0)
                {
                    sumNegative -= entryVir.GeneratorFactor;
                }

                if (entryVir is RegimeChangeVectorNode nodeVir)
                {
                    if (nodeVir.LoadFactor < 0)
                    {
                        sumPositive -= nodeVir.LoadFactor;
                    }
                    else if (nodeVir.LoadFactor > 0)
                    {
                        sumNegative += nodeVir.LoadFactor;
                    }
                }
            }

            foreach (var entryVir in entriesVir)
            {

                if (entryVir.GeneratorFactor > 0)
                {
                    entryVir.GeneratorFactor /= sumPositive;
                }
                else if (entryVir.GeneratorFactor < 0)
                {
                    entryVir.GeneratorFactor /= sumNegative;
                }

                if (entryVir is RegimeChangeVectorNode nodeVir)
                {
                    if (nodeVir.LoadFactor < 0)
                    {
                        nodeVir.LoadFactor /= sumPositive;
                    }
                    else if (nodeVir.LoadFactor > 0)
                    {
                        nodeVir.LoadFactor /= sumNegative;
                    }
                }
            }
        }

        /// <summary>
        /// Увеличивает значения коэффициентов.
        /// </summary>
        /// <param name="entriesVir">Список записей вир.</param>
        /// <param name="multiplier">Множитель.</param>
        private static void MultiplyFactor(
            List<RegimeChangeVectorGenerator> entriesVir,
            double multiplier
        )
        {
            entriesVir.ForEach(entryVir =>
            {
                if (entryVir is RegimeChangeVectorNode newLoadVir)
                {
                    newLoadVir.GeneratorFactor *= multiplier;
                    newLoadVir.LoadFactor *= multiplier;
                }
                else
                {
                    entryVir.GeneratorFactor *= multiplier;
                }
            });

        }

        /// <summary>
        /// Заполняет таблицу Приращения_узлы.
        /// </summary>
        /// <param name="entriesVir">Список с данными о вир.</param>
        private static void FillUtFromVir(
            this RastrWrapper provider,
            List<RegimeChangeVectorGenerator> entriesVir
        )
        {
            Table weighting = provider.Tables["ut_node"];

            Column type = weighting.Columns["tip"];
            Column number = weighting.Columns["ny"];
            Column activePowerLoad = weighting.Columns["pn"];
            Column tangent = weighting.Columns["tg"];
            Column activePowerGeneration = weighting.Columns["pg"];

            foreach (var entryVir in entriesVir)
            {
                if (entryVir is RegimeChangeVectorNode newLoadVir)
                {

                    weighting.AddRow();
                    int index = weighting.Count - 1;

                    type[index] = 0;
                    number[index] = newLoadVir.Number;
                    activePowerLoad[index] = newLoadVir.LoadFactor;
                    tangent[index] = newLoadVir.ConstantTangent;
                    activePowerGeneration[index] = newLoadVir.GeneratorFactor;
                }
                else
                {

                    weighting.AddRow();
                    int index = weighting.Count - 1;

                    type[index] = 2;
                    number[index] = entryVir.Number;
                    activePowerGeneration[index] = entryVir.GeneratorFactor;
                }
            }
        }

        private static Rastr GetRastr(
            this RastrWrapper provider
        )
        {
            FieldInfo? fieldInfo = typeof(RastrWrapper)
                .GetField(
                    "_rastr",
                    BindingFlags.NonPublic
                    | BindingFlags.Instance
                );

            if (fieldInfo != null)
            {
                if (fieldInfo.GetValue(provider) is Rastr rastr)
                { return rastr; }
                throw new ArgumentNullException("Куда делся _rastr?");
            }
            else
            {
                throw new ArgumentNullException("Куда делся _rastr?");
            }
        }
        /// <summary>
        /// Класс хранит в себе основные данные как строки
        /// из таблицы МДП: Генераторы ВИР.
        /// </summary>
        /// <remarks>
        /// Инициализирует новый экземпляр класса <see cref="RegimeChangeVectorGenerator"/>.
        /// </remarks>
        /// <param name="number">Номер(Генератора).</param>
        /// <param name="generatorFactor">Коэффициент генератора.</param>
        private class RegimeChangeVectorGenerator(int number, double generatorFactor)
        {

            /// <summary>
            /// Принять, вернуть номер.
            /// </summary>
            public int Number { get; set; } = number;

            /// <summary>
            /// Принять, вернуть коэффициент генерации.
            /// </summary>
            public double GeneratorFactor { get; set; } = generatorFactor;

        }
        /// <summary>
        /// Класс хранит в себе основные данные как строки
        /// из таблицы МДП: Узлы ВИР.
        /// </summary>
        private class RegimeChangeVectorNode : RegimeChangeVectorGenerator
        {
            /// <summary>
            /// Инициализирует новый экземпляр класса <see cref="RegimeChangeVectorNode"/>.
            /// </summary>
            /// <param name="number">Номер узла.</param>
            /// <param name="generatorFactor">Коэффициент генератора.</param>
            /// <param name="loadFactor">Коэффициент нагрузки.</param>
            /// <param name="constantTangent">Параметр постоянства тангенса.</param>
            public RegimeChangeVectorNode(int number, double loadFactor, double generatorFactor,
                bool constantTangent) : base(number, generatorFactor)
            {
                Number = number;
                GeneratorFactor = generatorFactor;
                LoadFactor = loadFactor;
                ConstantTangent = constantTangent;
            }

            /// <summary>
            /// Принять, вернуть коэффициент нагрузки.
            /// </summary>
            public double LoadFactor { get; set; }

            /// <summary>
            /// Принять, вернуть параметр постоянства тангенса.
            /// </summary>
            public bool ConstantTangent { get; set; }
        }
    }
}
