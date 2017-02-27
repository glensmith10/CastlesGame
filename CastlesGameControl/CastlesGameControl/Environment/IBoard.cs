using System.Collections.Generic;
using CastlesGameControl.Game;

namespace CastlesGameControl.Environment
{
    public interface IBoard
    {
        ICell[][] Arena { get; set; }
        IEnumerable<ICell> Cells { get; }
        GameMode GameMode { get; set; }
        int Height { get; }
        IList<IInstruction> Instructions { get; set; }
        long MoveScore { get; set; }
        IPlayer Owner { get; set; }
        string Reference { get; set; }
        long Score { get; set; }
        int Width { get; }

        void BuildWalls();
        void Clear();
        bool ContainsCellsPiecesBy(IPlayer player);
        bool ContainsCellsPiecesNotBy(IPlayer player);
        bool ContainsNonOwnerPieces();
        bool ContainsNoOwnerPieces();
        bool ContainsNoPieces();
        bool ContainsNoPieces(IPlayer player);
        bool IsFull();
        void PopulateDefaultIds();
        void ResetOwnership();
    }
}