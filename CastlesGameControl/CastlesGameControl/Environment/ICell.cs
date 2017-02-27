using System.Drawing;
using System.Windows;

namespace CastlesGameControl.Environment
{
    public interface ICell
    {
        Color CellColor { get; }
        bool ContainsPlayablePiece { get; }
        Vector CurrentLocation { get; set; }
        int Health { get; set; }
        string Id { get; set; }
        Vector Location { get; set; }
        IPlayer Owner { get; set; }
        MergeStatus Status { get; set; }
        int Strength { get; set; }
        CellType Type { get; set; }
        int? Value { get; set; }

        void Clear();

        void Import(ICell importCell, bool includeLocation);
    }
}