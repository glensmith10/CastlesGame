using System.Collections.Generic;

namespace CastlesGameControl.Environment
{
    public interface IGameGrid
    {
        IEnumerable<int> Rows { get; set; }
    }
}