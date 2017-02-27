using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CastlesGameControl.Environment
{
    public class LimboCellCollection : IEnumerable<LimboCell>
    {
        private IEnumerable<LimboCell> _limboCells;

        public LimboCellCollection()
        {
            Initialize();
        }

        private void Initialize()
        {
            _limboCells = new List<LimboCell>();
        }

        public void Add(LimboCell cell)
        {
            ((List<LimboCell>)_limboCells).Add(cell);
        }

        public void Remove(LimboCell cell)
        {
            ((List<LimboCell>)_limboCells).Remove(cell);
        }

        public void ClearRemovedCells()
        {
            _limboCells = _limboCells.Where(x => (((ICell)x).Value ?? 0) != 0).ToList();
        }

        public IEnumerator<LimboCell> GetEnumerator()
        {
            return _limboCells.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}