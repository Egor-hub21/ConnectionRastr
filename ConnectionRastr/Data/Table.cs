using ASTRALib;
using System.Collections;

namespace ConnectionRastr
{
    public class Table
    {
        private readonly ITable _table;
        private readonly Columns _dataColumns;
        public readonly Tables Owner;

        internal Table(ITable table, Tables owner)
        {
            _table = table;
            _dataColumns = new(_table.Cols, this);
            Owner = owner;
        }

        public Columns Columns
        { get => _dataColumns; }

        public int Count => _table.Size;
        public string Name => _table.Name;


        #region ROWS
        public void Clear(string filter = "")
        {
            _table.SetSel(filter);
            try
            {
                _table.DelRowS();
            }
            finally
            {
                _table.SetSel("");
            }
        }

        public void AddRow() => _table.AddRow();
        public void DuplicateRow(int index) => _table.DupRow(index);
        public void InsertNewRow(int position) => _table.InsRow(position);
        public void DeleteRow(int index) => _table.DelRow(index);
        #endregion

        #region SEARCH
        /// <summary>
        /// Поиск строки по запросу.
        /// </summary>
        /// <param name="query">Запрос.</param>
        /// <returns>Индекс строки, -1 в случае отсутствия совпадений.</returns>
        /// <remarks>
        /// Вернет индекс первой строки удовлетворяющей условию.
        /// </remarks>
        public int SelectRow(string query)
        {
            _table.SetSel(query);
            int result = -1;
            try
            {
                result = _table.FindNextSel[-1];
            }
            finally
            {
                _table.SetSel("");
            }
            return result;
        }

        /// <summary>
        /// Поиск строк по запросу.
        /// </summary>
        /// <param name="query">Запрос.</param>
        /// <returns>Массив индексов строк удовлетворяющих условие.</returns>
        /// <remarks>
        /// В случае отсутствия совпадений вернет пустой массив.
        /// </remarks>
        public int[] SelectRows(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                int[] array = new int[_table.Count];
                for (int i = 0; i < _table.Count; i++)
                {
                    array[i] = i;
                }
                return array;
            }
            _table.SetSel(query);
            int[] result;
            try
            {
                int newSize = 0;
                int[] array = new int[_table.Count];
                for (int j = _table.get_FindNextSel(-1); j >= 0; j = _table.get_FindNextSel(j))
                {
                    array[newSize++] = j;
                }
                Array.Resize(ref array, newSize);
                Array.Sort(array);
                result = array;
            }
            finally
            {
                _table.SetSel("");
            }
            return result;
        }
        #endregion

        #region KEY
        public string[] GetNameKeyColumns(
        ) => _table.Key.Split(
             [','],
             StringSplitOptions.RemoveEmptyEntries
        );

        public string GetUniqueRecordQueryString(
            int index
        ) => _table.get_SelString(index);
        #endregion
    }
}
