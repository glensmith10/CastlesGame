using CastlesGameControl.Game;
using System.Windows;

namespace CastlesGameControl.Environment
{
    public class LimboCell : Cell
    {
        private MoveDirection _attackDirection;
        private IBoard _target;

        public LimboCell(ICell cell) : base(cell.Location, cell.Value, cell.Owner)
        {
        }

        public LimboCell(Vector location, int? value, IPlayer owner) : base(location, value, owner)
        {
        }

        public IBoard Source { get; set; }

        public IBoard Target
        {
            get { return _target; }
            set
            {
                if (_target == value) return;

                _target = value;
                RecalculateLocation();
            }
        }

        public MoveDirection AttackDirection
        {
            get { return _attackDirection; }
            set
            {
                if (_attackDirection == value) return;

                _attackDirection = value;
                RecalculateLocation();
            }
        }

        public int Position { get; set; }

        public bool AddedToBoard { get; set; }

        private void RecalculateLocation()
        {
            if (_target == null) return;

            if (AttackDirection == MoveDirection.Left)
            {
                Location = new Vector(_target.Width, (int)Location.Y);
                Position = (int)Location.Y;
            }

            if (AttackDirection == MoveDirection.Right)
            {
                Location = new Vector(-1, (int)Location.Y);
                Position = (int)Location.Y;
            }

            if (AttackDirection == MoveDirection.Up)
            {
                Location = new Vector((int)Location.X, _target.Height);
                Position = (int)Location.X;
            }

            if (AttackDirection == MoveDirection.Down)
            {
                Location = new Vector((int)Location.X, -1);
                Position = (int)Location.X;
            }
        }
    }
}