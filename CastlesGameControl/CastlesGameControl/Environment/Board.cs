using log4net;
using System.Collections.Generic;
using System.Linq;
using CastlesGameControl.Game;
using System.Globalization;
using System.Windows;

namespace CastlesGameControl.Environment
{
    public class Board : IBoard
    {
        private IPlayer _owner;
        private readonly ILog _log;

        public ICell[][] Arena { get; set; }

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public IEnumerable<ICell> Cells
        {
            get
            {
                return Arena.SelectMany(row => row);
            }
        }

        public bool ContainsCellsPiecesBy(IPlayer player)
        {
            return Cells.Any(x => x.ContainsPlayablePiece && x.Owner == player);
        }

        public bool ContainsCellsPiecesNotBy(IPlayer player)
        {
            return Cells.Any(x => x.ContainsPlayablePiece && x.Owner != player);
        }

        public bool ContainsNonOwnerPieces()
        {
            return ContainsCellsPiecesNotBy(Owner);
        }

        public IList<IInstruction> Instructions { get; set; }

        public Board(ILog log) : this(6, 6, log)
        {
        }

        public Board(int width, int height, ILog log) : this(width, height, null, log)
        {
        }

        public Board(int width, int height, Player owner, ILog log)
        {
            Width = width;
            Height = height;
            Owner = owner;
            _log = log;
            Initialize();
        }

        protected void Initialize()
        {
            Instructions = new List<IInstruction>();
            GenerateBoard();
            GameMode = GameMode.Build;
        }

        private void GenerateBoard()
        {
            Arena = new ICell[Height][];

            for (var row = 0; row < Height; row++)
            {
                Arena[row] = new ICell[Width];

                for (var column = 0; column < Width; column++)
                {
                    Arena[row][column] = new Cell(new Vector(column, row), null, Owner);
                }
            }

            BuildWalls();
        }

        public void Clear()
        {
            foreach (var row in Arena)
            {
                foreach (var cell in row)
                {
                    if (cell.Type == CellType.Playable)
                    {
                        if (cell.ContainsPlayablePiece)
                        {
                            Instructions.Add(new Instruction(InstructionType.Delete, new GridPosition { X = (int)cell.Location.X, Y = (int)cell.Location.Y }));
                        }
                        cell.Clear();
                    }
                }
            }
        }

        public long Score { get; set; }
        public long MoveScore { get; set; }
        public string Reference { get; set; }

        public IPlayer Owner
        {
            get { return _owner; }
            set
            {
                if (value == null || value.Equals(_owner)) return;

                _owner = value;
                ResetOwnership();
            }
        }

        public void ResetOwnership()
        {
            foreach (var row in Arena)
            {
                foreach (var cell in row)
                {
                    cell.Owner = Owner;
                }
            }
        }

        public bool IsFull()
        {
            return !Arena.Any(row => row.Any(cell => !cell.ContainsPlayablePiece && cell.Type != CellType.Wall));
        }

        public void PopulateDefaultIds()
        {
            foreach (var row in Arena)
            {
                foreach (var cell in row)
                {
                    int cellId = (Width * (int)cell.Location.X) + ((int)cell.Location.Y + 1);
                    cell.Id = cellId.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        public void BuildWalls()
        {
            foreach (var row in Arena)
            {
                foreach (var cell in row)
                {
                    //Outside border
                    if ((int)cell.Location.Y == 0
                        || (int)cell.Location.Y == Height - 1
                        || (int)cell.Location.X == 0
                        || (int)cell.Location.X == Width - 1)
                    {
                        cell.Type = CellType.Wall;
                    }
                }
            }
        }

        public GameMode GameMode { get; set; }

        public bool ContainsNoOwnerPieces()
        {
            return !ContainsNoPieces() && ContainsNoPieces(Owner);
        }

        public bool ContainsNoPieces()
        {
            return Arena.All(row => !row.Any(cell => cell.ContainsPlayablePiece));
        }

        public bool ContainsNoPieces(IPlayer player)
        {
            return !Cells.Any(cell => cell.ContainsPlayablePiece && cell.Owner.Equals(player));
        }
    }
}