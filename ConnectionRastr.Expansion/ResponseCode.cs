namespace ConnectionRastr.Expansion
{
    /// <summary>
    /// Код ответа расчета.
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// Режим балансируется.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Режим не балансируется,
        /// дальнейшее утяжеление невозмож
        /// </summary>
        BalancingFailed = 1,

        /// <summary>
        /// утяжеление закончено с заданной точностью
        /// или достигнуто предельное число итераций
        /// </summary>
        WeightingDone = 2,

        /// <summary>
        /// Неизвестный.
        /// </summary>
        NoNe = 3,
    }
}
