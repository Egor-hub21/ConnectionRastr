namespace ConnectionRastr.Extension
{
    public enum CalcSetting
    {
        /// <summary>
        /// параметры по умолчанию.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Инициализация параметров.
        /// </summary>
        Initialize = 1,

        /// <summary>
        /// с плоского старта;
        /// </summary>
        FlatStart = 2,

        /// <summary>
        /// отключение стартового алгоритма;
        /// </summary>
        NoStartAlgorithm = 3,

        /// <summary>
        /// отключение контроля данных;
        /// </summary>
        NoDataControl = 4,

        /// <summary>
        /// отключение подготовки данных
        /// </summary>
        NoDataPrep = 5,
    }
}
