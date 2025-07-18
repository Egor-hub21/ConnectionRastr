using ASTRALib;

namespace ConnectionRastr
{
    public class Tables : IEnumerable<Table>
    {
        private readonly ITables _tables;
        public readonly RastrWrapper Owner;
        internal Tables(ITables tables, RastrWrapper owner)
        {
            _tables = tables;
            Owner = owner;
        }

        public int Count => _tables.Count;

        #region Индексаторы

        /// <summary>
        /// Индексатор по таблицам.
        /// </summary>
        /// <param name="index">Номер таблицы.</param>
        /// <returns>Таблица.</returns>
        public Table this[int index]
        {
            get
            {
                if (index >= Count || index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return GetDataTable(index);
            }
        }

        /// <summary>
        /// Индексатор по таблицам.
        /// </summary>
        /// <param name="name">Название таблицы.</param>
        /// <returns>Таблица.</returns>
        public Table this[string name]
        {
            get => GetDataTable(name);
        }
        #endregion

        #region ServicMethod

        /// <summary>
        /// Возвращает таблицу.
        /// </summary>
        /// <param name="identifier">Идентификатор.</param>
        /// <returns>Таблица.</returns>
        private Table GetDataTable<T>(T identifier)
        {
            return new Table(
                _tables.Item(identifier),
                this
            );
        }
        #endregion

        #region IEnumerable
        public IEnumerator<Table> GetEnumerator()
        {
            return new DataTableEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class DataTableEnumerator : IEnumerator<Table>
        {
            private readonly Tables _dataTables;
            private int _currentIndex = -1;

            public DataTableEnumerator(Tables dataTables)
            {
                _dataTables = dataTables;
            }

            public Table Current => _dataTables[_currentIndex];

            object System.Collections.IEnumerator.Current => Current;

            public bool MoveNext()
            {
                _currentIndex++;
                return _currentIndex < _dataTables.Count;
            }

            public void Reset()
            {
                _currentIndex = -1;
            }

            public void Dispose()
            {
                // Здесь можно освободить ресурсы, если это необходимо
            }
        }
        #endregion
    }
}
