using ASTRALib;

namespace ConnectionRastr
{
    public class Columns : IEnumerable<Column>
    {
        private readonly ICols _columns;
        public readonly Table Owner;

        internal Columns(ICols columns, Table owner)
        {
            _columns = columns;
            Owner = owner;
        }

        public int Count => _columns.Count;

        #region Индексаторы

        /// <summary>
        /// Индексатор по колонкам.
        /// </summary>
        /// <param name="index">Номер колонки.</param>
        /// <returns>Колонка.</returns>
        public Column this[int index]
        {
            get
            {
                if (index >= Count || index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return GetDataColumn(index);
            }
        }

        /// <summary>
        /// Индексатор по колонкам.
        /// </summary>
        /// <param name="name">Название колонки.</param>
        /// <returns>Колонка.</returns>
        public Column this[string name]
        {
            get => GetDataColumn(name);
        }
        #endregion

        #region ServicMethod

        /// <summary>
        /// Возвращает колонку.
        /// </summary>
        /// <param name="identifier">Идентификатор.</param>
        /// <returns>Колонка.</returns>
        private Column GetDataColumn<T>(T identifier)
        {
            return new Column(
                _columns.Item(identifier),
                this
            );
        }
        #endregion

        #region IEnumerable
        public IEnumerator<Column> GetEnumerator()
        {
            return new DataColumnEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class DataColumnEnumerator : IEnumerator<Column>
        {
            private readonly Columns _dataColumns;
            private int _currentIndex = -1;

            public DataColumnEnumerator(Columns dataColumns)
            {
                _dataColumns = dataColumns;
            }

            public Column Current => _dataColumns[_currentIndex];

            object System.Collections.IEnumerator.Current => Current;

            public bool MoveNext()
            {
                _currentIndex++;
                return _currentIndex < _dataColumns.Count;
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
