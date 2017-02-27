using System.Drawing;
using System.Windows;

namespace CastlesGameControl.Environment
{
    public enum CellType
    {
        Playable,
        Wall
    }

    public enum MergeStatus
    {
        None,
        Moved,
        Merged,
        Attacked
    }

    public class Cell : ICell
    {
        public Cell(Vector location, int? value, IPlayer owner)
        {
            Location = location;
            Value = value;
            Type = CellType.Playable;
            Status = MergeStatus.None;
            Owner = owner;
        }

        public string Id { get; set; }
        public Vector CurrentLocation { get; set; }
        public Vector Location { get; set; }
        public int? Value { get; set; }
        public MergeStatus Status { get; set; }
        public IPlayer Owner { get; set; }
        public int Health { get; set; }
        public CellType Type { get; set; }

        public int Strength { get; set; }

        public Color CellColor => Owner?.PlayerColor ?? Color.White;

        public void Clear()
        {
            if (Type != CellType.Playable) return;

            Id = string.Empty;
            Value = null;
        }

        public bool ContainsPlayablePiece => Type == CellType.Playable && (Value ?? 0) != 0;

        public void Import(ICell importCell, bool includeLocation)
        {
            Health = importCell.Health;
            Value = importCell.Value;
            Strength = importCell.Strength;
            Status = importCell.Status;
            Owner = importCell.Owner;

            if (!includeLocation) return;
            Location = importCell.Location;
        }
    }
}