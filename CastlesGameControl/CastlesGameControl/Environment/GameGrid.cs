using System.Collections.Generic;

namespace CastlesGameControl.Environment
{
    public class GameGrid : IGameGrid
    {
        public IEnumerable<int> Rows { get; set; }
    }
}