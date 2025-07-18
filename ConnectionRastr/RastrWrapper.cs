using ASTRALib;

using System.Runtime.InteropServices;

namespace ConnectionRastr
{
    /// <summary>
    /// Класс обертка над Rastr.
    /// </summary>
    public class RastrWrapper : IDisposable
    {
        private bool _isDisposed = false;

        /// <summary>
        /// Объект класса Rastr.
        /// </summary>
        private Rastr _rastr;
        private readonly Tables _dataTables;

        public RastrWrapper()
        {
            _rastr = new Rastr();
            _dataTables = new(_rastr.Tables, this);
        }

        public RastrWrapper(Rastr original)
        {
            _rastr = original;
            _dataTables = new(_rastr.Tables, this);
        }

        public Tables Tables
        { get => _dataTables; }

        public Rastr Original { get => _rastr; }

        #region WorkFileRastr

        /// <summary>
        /// Загрузка файла в рабочую область.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="stencilPath">Путь к шаблону.</param>
        /// <remarks>
        /// - При указании файла и шаблона данные из файла
        /// замут рабочую область в соответствии с шаблоном:
        /// <code>LoadFile(filePath, stencilPath)</code>
        /// - Если не указать файл, то шаблон загрузится,
        /// очистив рабочую область согласно шаблону:
        /// <code>LoadFile("", stencilPath)</code>
        /// - Если не указать шаблон, то файл загрузится,
        /// очистив всю рабочую область: <code>LoadFile(filePath)</code>
        /// </remarks>
        public void LoadFile(string filePath,
            string stencilPath = "")
        {
            OpenFile(LoadCode.Load, filePath, stencilPath);
        }

        /// <summary>
        /// Открытие файла в рабочей области.
        /// </summary>
        /// <param name="code">Код, определяющий тип загрузки.</param>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="stencilPath">Путь к шаблону.</param>
        /// <remarks>
        /// - При указании файла и шаблона данные из файла
        /// займут рабочую область в соответствии с шаблоном:
        /// <code>LoadFile(filePath, stencilPath)</code>
        /// - Если не указать файл, то шаблон загрузится,
        /// очистив рабочую область согласно шаблону:
        /// <code>LoadFile("", stencilPath)</code>
        /// - Если не указать шаблон, то файл загрузится,
        /// очистив всю рабочую область: <code>LoadFile(filePath)</code>
        /// </remarks>
        public void OpenFile(
            LoadCode code,
            string filePath,
            string stencilPath = "")
        {
            _rastr.Load(code.ToRgKod(), filePath, stencilPath);
        }

        /// <summary>
        /// Сохранение рабочей области в файл.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="stencilPath">Путь к шаблону.</param>
        /// <remarks><para>
        /// Если не задать шаблон то сохранится вся рабочая область.</para>
        /// </remarks>
        public void SaveFile(string filePath,
            string stencilPath = "")
        {
            _rastr.Save(filePath, stencilPath);
        }

        /// <summary>
        /// Очистка рабочей области по указанному шаблону.
        /// </summary>
        /// <param name="stencilPath">Путь к шаблону.</param>
        /// <remarks><para>
        /// Если структура таблицы в рабочей области не существует, она
        /// создается(пустая).</para><para>
        /// Если таблица уже существует, ее содержимое очищается и приводится
        /// в соответствие с шаблоном.</para><para>
        /// Если шаблон не задан, то очищается вся рабочая область.</para>
        /// </remarks>
        public void ClearWorkspace(string stencilPath = "")
        {
            _rastr.NewFile(stencilPath);
        }
        #endregion

        #region Dispose

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;


            // Освобождение неуправляемых ресурсов
            if (_rastr != null)
            {
                _ = Marshal.ReleaseComObject(_rastr);
                _rastr = null;
            }

            _isDisposed = true;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RegimeSolver"/> class.
        /// </summary>
        ~RastrWrapper()
        {
            Dispose(false);
        }
        #endregion
    }
}
