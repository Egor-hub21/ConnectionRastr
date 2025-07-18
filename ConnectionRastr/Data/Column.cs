using ASTRALib;
using System.Collections;
using System.Drawing;

namespace ConnectionRastr
{
    public class Column : IEnumerable
    {
        private readonly ICol _column;
        public readonly Columns Owner;

        internal Column(ICol column, Columns owner)
        {
            _column = column;
            Owner = owner;
        }

        public object this[int i]
        {
            get
            {
                CheckIndex(i);
                return GetValue(i);
            }
            set
            {
                CheckIndex(i);
                _column.Z[i] = value;
            }
        }

        public string Name => _column.Name;
        public int Index => _column.Index;
        public int Count => Owner.Owner.Count;
        public Type TypeOfItems => _column.type switch
        {
            PropTT.PR_INT => typeof(int),
            PropTT.PR_REAL => typeof(double),
            PropTT.PR_STRING => typeof(string),
            PropTT.PR_BOOL => typeof(bool),
            PropTT.PR_ENUM => typeof(Enum),
            PropTT.PR_ENPIC => typeof(object),
            PropTT.PR_COLOR => typeof(Color),
            PropTT.PR_SUPERENUM => typeof(Enum),
            PropTT.PR_TIME => typeof(DateTime),
            PropTT.PR_HEX => typeof(ulong),
            _ => typeof(object)
        };

        public string TypeNameOfItems => _column.type switch
        {
            PropTT.PR_INT => "integer",
            PropTT.PR_REAL => "real_number",
            PropTT.PR_STRING => "string",
            PropTT.PR_BOOL => "bool",
            PropTT.PR_ENUM => "enum",
            PropTT.PR_ENPIC => "picture",
            PropTT.PR_COLOR => "color",
            PropTT.PR_SUPERENUM => "ref_enum",
            PropTT.PR_TIME => "date_time",
            PropTT.PR_HEX => "hexadecimal",
            _ => "unknown_type"
        };

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DataColumnEnumerator(this);
        }

        private sealed class DataColumnEnumerator : IEnumerator
        {
            private readonly Column _dataColumn;
            private int _currentIndex = -1;

            public DataColumnEnumerator(Column dataColumn)
            {
                _dataColumn = dataColumn;
            }

            public object Current => _dataColumn[_currentIndex];

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                _currentIndex++;
                return _currentIndex < _dataColumn.Count;
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

        #region ServiceMethods
        private void CheckIndex(int index)
        {
            if (index >= Count || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        private object GetValue(int index)
        {
            object value = _column.Z[index];
            return _column.type switch
            {
                PropTT.PR_INT => (int)value,
                PropTT.PR_REAL => (double)value,
                PropTT.PR_STRING => (string)value,
                PropTT.PR_BOOL => (bool)value,
                // Если будет ошибка поменять на int
                PropTT.PR_ENUM => (Enum)value,
                PropTT.PR_ENPIC => value,
                PropTT.PR_COLOR => (Color)value,
                // Если будет ошибка поменять на int
                PropTT.PR_SUPERENUM => (Enum)value,
                PropTT.PR_TIME => (DateTime)value,
                // Проверить на корректность
                PropTT.PR_HEX => (ulong)value,
                _ => value
            };
        }
        #endregion
    }
}
