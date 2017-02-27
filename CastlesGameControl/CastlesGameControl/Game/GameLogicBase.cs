using CastlesGameControl.Environment;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CastlesGameControl.Game
{
    public enum MoveStatus
    {
        Valid,
        Invalid,
        OutOfTurn,
        Victory,
        Draw,
        GameEnded
    }

    public enum MoveDirection
    {
        None,
        Up,
        Down,
        Left,
        Right,
        AttackUp,
        AttackDown,
        AttackLeft,
        AttackRight,
        Special
    }

    public enum GameMode
    {
        Build,
        Attack
    }

    public struct AttackDetails
    {
        public MoveDirection Direction { get; set; }
        public IBoard SourceBoard { get; set; }
        public IBoard TargetBoard { get; set; }
        public IPlayer Owner { get; set; }
    }

    public abstract class GameLogicBase
    {
        private readonly IEnumerable<IBoard> _boards;
        protected ILog Log;

        protected GameLogicBase(ILog log) : this(new List<IBoard> { new Board(log), new Board(log) }, log)
        { }

        protected GameLogicBase(IEnumerable<IBoard> boards, ILog log)
        {
            Log = log;
            _boards = boards;
            GameScore = 0;
            ClearLimboCells();
        }

        public int GameScore { get; set; }

        public int MaxStorage { get; set; }

        public IPlayer CurrentPlayer { get; set; }

        public virtual IEnumerable<IBoard> StartGame()
        {
            GameScore = 0;
            ClearLimboCells();

            foreach (var board in _boards)
            {
                board.Clear();
                board.PopulateDefaultIds();
                board.ResetOwnership();
                board.Score = 0;
            }

            return _boards;
        }

        public virtual IEnumerable<IBoard> Boards => _boards;

        public virtual MoveStatus Move(MoveDirection direction, IBoard board, IPlayer player)
        {
            Log.DebugFormat("Move {0} on board {1} by player {2}", direction.ToString(), board.Owner.Name, player.Name);

            CurrentPlayer = player;

            var validMove = MoveStatus.Invalid;

            switch (direction)
            {
                case MoveDirection.Left:
                    validMove = MoveLeft(board, player);
                    break;

                case MoveDirection.Right:
                    validMove = MoveRight(board, player);
                    break;

                case MoveDirection.Up:
                    validMove = MoveUp(board, player);
                    break;

                case MoveDirection.Down:
                    validMove = MoveDown(board, player);
                    break;
            }

            Log.DebugFormat("//moving pieces on board result is {0}", validMove);

            GameLimboCells.ClearRemovedCells();

            if (!board.ContainsNoOwnerPieces()) return validMove;

            Log.DebugFormat("{0} - No Owner Pieces", board.Owner.Name);
            Log.DebugFormat("//Board {0} contains no owner pieces", board.Owner.Name);

            return board == _boards.FirstOrDefault() ? MoveStatus.GameEnded : MoveStatus.Victory;
        }

        public IEnumerable<LimboCell> FetchAttackingPieces(IBoard board, IPlayer player, MoveDirection move)
        {
            var attackingPieces = GameLimboCells.Where(x => x.ContainsPlayablePiece
                                                            && x.AttackDirection == move
                                                            && x.Source.Owner.Equals(player)
                                                            && x.Target.Owner.Equals(board.Owner));
            return attackingPieces;
        }

        private MoveStatus MoveUp(IBoard board, IPlayer player)
        {
            var validMove = MoveStatus.Invalid;
            var width = board.Width;
            var height = board.Height;
            var attackingPieces = FetchAttackingPieces(board, player, MoveDirection.Up).ToList();

            Log.DebugFormat("Found {0} attacking pieces", attackingPieces.Count());
            CreateNewTileInstructions(attackingPieces);

            Log.Debug("Moving Up");
            for (var col = 0; col < width; col++)
            {
                Log.DebugFormat("Starting Column {0}", col);
                for (var currRow = 0; currRow < height - 1; currRow++)
                {
                    Log.DebugFormat("Setting Current Row {0}", currRow);

                    var currentCell = board.Arena[currRow][col];
                    currentCell.Status = MergeStatus.None;

                    Log.DebugFormat("Cell Merge Status set to None");

                    Log.DebugFormat("Cell is owned by: {0}", currentCell.Owner.Name);
                    Log.DebugFormat("Cell value is: {0}", currentCell.Value);
                    Log.DebugFormat("Cell health is: {0}", currentCell.Health);

                    if (currentCell.Type == CellType.Wall) continue;

                    for (var testRow = currRow + 1; testRow <= height; testRow++)
                    {
                        Log.DebugFormat("Testing Cell against row {0}", testRow);

                        if (currentCell.Status == MergeStatus.Attacked) break;

                        var testCell = testRow < height
                            ? board.Arena[testRow][col]
                            : attackingPieces.FirstOrDefault(x => (int)x.Location.X == col);

                        if (testCell == null) break;

                        testCell.Status = MergeStatus.None;

                        //if (testCell.Type == CellType.Wall) HistoryLogger.Print(includeInLog, "Test Cell is Wall!");
                        if (testCell.Type == CellType.Wall)
                        {
                            if (testRow > 1 && testRow < height - 1)
                            {
                                break;
                            }
                            continue;
                        }

                        var thisIsValid = false;

                        Log.DebugFormat("Cell is owned by: {0}", testCell.Owner.Name);
                        Log.DebugFormat("Cell value is: {0}", testCell.Value);
                        Log.DebugFormat("Cell health is: {0}", testCell.Health);
                        Log.DebugFormat("Cell strength is: {0}", testCell.Strength);

                        if (!testCell.Owner.Equals(player) && testCell.ContainsPlayablePiece)
                        {
                            break;
                        }

                        if (testCell.Owner.Equals(currentCell.Owner) || !currentCell.ContainsPlayablePiece)
                        {
                            if (testCell.ContainsPlayablePiece && currentCell.ContainsPlayablePiece &&
                                !currentCell.Owner.Equals(player))
                            {
                                break;
                            }
                            Log.Debug("MERGING");
                            thisIsValid = MergeCells(testCell, currentCell, board);
                        }
                        else
                        {
                            if (currentCell.Value != null)
                            {
                                Log.Debug("ATTACKING");
                                thisIsValid = AttackCells(testCell, currentCell, board);
                            }
                        }

                        if (thisIsValid)
                        {
                            Log.Debug("Valid move!");
                            validMove = MoveStatus.Valid;
                        }

                        if (testCell.ContainsPlayablePiece && !thisIsValid) break;

                        if (testCell.Status == MergeStatus.Merged || testCell.Status == MergeStatus.Moved)
                        {
                            SetOwnershipOfCell(currentCell, board, player);
                            Log.Debug("This is to change between cells");
                            SetOwnershipBetweenCells(testCell, currentCell, board, player);
                        }

                        if (testCell.Status != MergeStatus.Merged) continue;

                        currentCell.Status = MergeStatus.Merged;
                        break;
                    }
                }
            }
            return validMove;
        }

        private MoveStatus MoveDown(IBoard board, IPlayer player)
        {
            var validMove = MoveStatus.Invalid;
            var width = board.Width;
            var height = board.Height;
            var attackingPieces = FetchAttackingPieces(board, player, MoveDirection.Down).ToList();

            Log.DebugFormat("Found {0} attacking pieces", attackingPieces.Count());
            CreateNewTileInstructions(attackingPieces);

            Log.DebugFormat("Moving Down");
            for (var col = 0; col < width; col++)
            {
                Log.DebugFormat("Starting Column {0}", col);

                for (var currRow = height - 1; currRow > 0; currRow--)
                {
                    Log.DebugFormat("Setting Current Row {0}", currRow);

                    var currentCell = board.Arena[currRow][col];
                    currentCell.Status = MergeStatus.None;
                    Log.DebugFormat("Cell Merge Status set to None");

                    Log.DebugFormat("Cell is owned by: {0}", currentCell.Owner.Name);
                    Log.DebugFormat("Cell value is: {0}", currentCell.Value);
                    Log.DebugFormat("Cell health is: {0}", currentCell.Health);

                    if (currentCell.Type == CellType.Wall) continue;

                    for (var testRow = currRow - 1; testRow >= -1; testRow--)
                    {
                        Log.DebugFormat("Testing Cell against column {0}", testRow);

                        if (currentCell.Status == MergeStatus.Attacked) break;

                        var testCell = testRow > -1
                            ? board.Arena[testRow][col]
                            : attackingPieces.FirstOrDefault(x => (int)x.Location.X == col);

                        if (testCell == null) break;

                        testCell.Status = MergeStatus.None;

                        if (testCell.Type == CellType.Wall)
                        {
                            if (testRow > 1 && testRow < height - 1)
                            {
                                break;
                            }
                            continue;
                        }

                        var thisIsValid = false;

                        Log.DebugFormat("Cell is owned by: {0}", testCell.Owner.Name);
                        Log.DebugFormat("Cell value is: {0}", testCell.Value);
                        Log.DebugFormat("Cell health is: {0}", testCell.Health);
                        Log.DebugFormat("Cell strength is: {0}", testCell.Strength);

                        if (!testCell.Owner.Equals(player) && testCell.ContainsPlayablePiece)
                        {
                            break;
                        }

                        if (testCell.Owner.Equals(currentCell.Owner) || !currentCell.ContainsPlayablePiece)
                        {
                            if (testCell.ContainsPlayablePiece && currentCell.ContainsPlayablePiece &&
                                !currentCell.Owner.Equals(player))
                            {
                                break;
                            }
                            Log.DebugFormat("MERGING");
                            thisIsValid = MergeCells(testCell, currentCell, board);
                        }
                        else
                        {
                            if (currentCell.Value != null)
                            {
                                Log.DebugFormat("ATTACKING");
                                thisIsValid = AttackCells(testCell, currentCell, board);
                            }
                        }

                        if (thisIsValid)
                        {
                            Log.DebugFormat("Valid move!");
                            validMove = MoveStatus.Valid;
                        }

                        if (testCell.ContainsPlayablePiece && !thisIsValid) break;

                        if (testCell.Status == MergeStatus.Merged || testCell.Status == MergeStatus.Moved)
                        {
                            SetOwnershipOfCell(currentCell, board, player);

                            Log.DebugFormat("This is to change between cells");
                            SetOwnershipBetweenCells(testCell, currentCell, board, player);
                        }

                        if (testCell.Status != MergeStatus.Merged) continue;

                        currentCell.Status = MergeStatus.Merged;
                        break;
                    }
                }
            }
            return validMove;
        }

        private MoveStatus MoveRight(IBoard board, IPlayer player)
        {
            var validMove = MoveStatus.Invalid;
            var width = board.Width;
            var height = board.Height;
            var attackingPieces = FetchAttackingPieces(board, player, MoveDirection.Right).ToList();

            CreateNewTileInstructions(attackingPieces);

            for (var row = 0; row < height; row++)
            {
                for (var currCol = width - 1; currCol > 0; currCol--)
                {
                    var currentCell = board.Arena[row][currCol];
                    currentCell.Status = MergeStatus.None;

                    if (currentCell.Type == CellType.Wall) continue;

                    for (var testCol = currCol - 1; testCol >= -1; testCol--)
                    {
                        if (currentCell.Status == MergeStatus.Attacked) break;

                        var testCell = testCol > -1
                            ? board.Arena[row][testCol]
                            : attackingPieces.FirstOrDefault(x => (int)x.Location.Y == row);

                        if (testCell == null) break;

                        testCell.Status = MergeStatus.None;

                        if (testCell.Type == CellType.Wall)
                        {
                            if (testCol > 1 && testCol < width - 1)
                            {
                                break;
                            }
                            continue;
                        }

                        var thisIsValid = false;

                        if (!testCell.Owner.Equals(player) && testCell.ContainsPlayablePiece)
                        {
                            break;
                        }

                        if (testCell.Owner.Equals(currentCell.Owner) || !currentCell.ContainsPlayablePiece)
                        {
                            if (testCell.ContainsPlayablePiece && currentCell.ContainsPlayablePiece &&
                                !currentCell.Owner.Equals(player))
                            {
                                break;
                            }
                            Log.DebugFormat("MERGING {0},{1} with {2},{3}", testCell.Location.X, testCell.Location.Y, currentCell.Location.X, currentCell.Location.Y);
                            thisIsValid = MergeCells(testCell, currentCell, board);
                        }
                        else
                        {
                            if (currentCell.Value != null)
                            {
                                Log.DebugFormat("ATTACKING  {0},{1} with {2},{3}", testCell.Location.X, testCell.Location.Y, currentCell.Location.X, currentCell.Location.Y);
                                thisIsValid = AttackCells(testCell, currentCell, board);
                            }
                        }

                        if (thisIsValid)
                        {
                            Log.DebugFormat("Valid move!");
                            validMove = MoveStatus.Valid;
                        }

                        if (testCell.ContainsPlayablePiece && !thisIsValid) break;

                        if (testCell.Status == MergeStatus.Merged || testCell.Status == MergeStatus.Moved)
                        {
                            SetOwnershipOfCell(currentCell, board, player);

                            SetOwnershipBetweenCells(testCell, currentCell, board, player);
                        }

                        if (testCell.Status != MergeStatus.Merged) continue;

                        currentCell.Status = MergeStatus.Merged;
                        break;
                    }
                }
            }
            return validMove;
        }

        private MoveStatus MoveLeft(IBoard board, IPlayer player)
        {
            var validMove = MoveStatus.Invalid;
            var width = board.Width;
            var height = board.Height;
            var attackingPieces = FetchAttackingPieces(board, player, MoveDirection.Left).ToList();

            Log.DebugFormat("Found {0} attacking pieces", attackingPieces.Count());

            CreateNewTileInstructions(attackingPieces);

            Log.DebugFormat("Moving Left");
            for (var row = 0; row < height; row++)
            {
                Log.DebugFormat("Starting Row {0}", row);
                for (var currCol = 0; currCol < width - 1; currCol++)
                {
                    Log.DebugFormat("Setting Current Column {0}", currCol);

                    var currentCell = board.Arena[row][currCol];
                    currentCell.Status = MergeStatus.None;
                    Log.DebugFormat("Cell Merge Status set to None");

                    Log.DebugFormat("Cell is owned by: {0}", currentCell.Owner.Name);
                    Log.DebugFormat("Cell value is: {0}", currentCell.Value);
                    Log.DebugFormat("Cell health is: {0}", currentCell.Health);

                    if (currentCell.Type == CellType.Wall) continue;

                    for (var testCol = currCol + 1; testCol <= width; testCol++)
                    {
                        Log.DebugFormat("Testing Cell against column {0}", testCol);

                        if (currentCell.Status == MergeStatus.Attacked) break;

                        var testCell = testCol < width
                            ? board.Arena[row][testCol]
                            : attackingPieces.FirstOrDefault(x => (int)x.Location.Y == row);

                        if (testCell == null) break;

                        testCell.Status = MergeStatus.None;

                        if (testCell.Type == CellType.Wall)
                        {
                            if (testCol > 1 && testCol < width - 1)
                            {
                                break;
                            }
                            continue;
                        }

                        var thisIsValid = false;

                        Log.DebugFormat("Cell is owned by: {0}", testCell.Owner.Name);
                        Log.DebugFormat("Cell value is: {0}", testCell.Value);
                        Log.DebugFormat("Cell health is: {0}", testCell.Health);
                        Log.DebugFormat("Cell strength is: {0}", testCell.Strength);

                        if (!testCell.Owner.Equals(player) && testCell.ContainsPlayablePiece)
                        {
                            break;
                        }

                        if (testCell.Owner.Equals(currentCell.Owner) || !currentCell.ContainsPlayablePiece)
                        {
                            if (testCell.ContainsPlayablePiece && currentCell.ContainsPlayablePiece &&
                                !currentCell.Owner.Equals(player))
                            {
                                break;
                            }
                            Log.DebugFormat("MERGING");
                            thisIsValid = MergeCells(testCell, currentCell, board);
                        }
                        else
                        {
                            if (currentCell.Value != null)
                            {
                                Log.DebugFormat("ATTACKING");
                                thisIsValid = AttackCells(testCell, currentCell, board);
                            }
                        }

                        if (thisIsValid)
                        {
                            Log.DebugFormat("Valid move!");
                            validMove = MoveStatus.Valid;
                        }

                        if (testCell.ContainsPlayablePiece && !thisIsValid) break;

                        if (testCell.Status == MergeStatus.Merged || testCell.Status == MergeStatus.Moved)
                        {
                            SetOwnershipOfCell(currentCell, board, player);

                            Log.DebugFormat("This is to change between cells");
                            SetOwnershipBetweenCells(testCell, currentCell, board, player);
                        }

                        if (testCell.Status != MergeStatus.Merged) continue;

                        currentCell.Status = MergeStatus.Merged;
                        break;
                    }
                }
            }
            return validMove;
        }

        private static void CreateNewTileInstructions(IEnumerable<LimboCell> attackingPieces)
        {
            foreach (var attackingPiece in attackingPieces.Where(x => !x.AddedToBoard))
            {
                var instruction = new Instruction(InstructionType.New, new GridPosition
                {
                    X = (int)attackingPiece.Location.X,
                    Y = (int)attackingPiece.Location.Y
                })
                { Value = attackingPiece.Value, Owner = attackingPiece.Owner };

                attackingPiece.Target.Instructions.Add(instruction);
                attackingPiece.AddedToBoard = true;
            }
        }

        private void SetOwnershipOfCell(ICell targetCell, IBoard board, IPlayer newOwner)
        {
            var instruction = new Instruction(InstructionType.ChangeOwnership, new GridPosition
            {
                X = (int)targetCell.Location.X,
                Y = (int)targetCell.Location.Y
            })
            { Owner = newOwner };

            board.Instructions.Add(instruction);
            targetCell.Owner = newOwner;
        }

        private void SetOwnershipBetweenCells(ICell sourceCell, ICell targetCell, IBoard board, IPlayer newOwner)
        {
            var minX = Math.Min((int)sourceCell.Location.X, (int)targetCell.Location.X);
            var maxX = Math.Max((int)sourceCell.Location.X, (int)targetCell.Location.X);
            var minY = Math.Min((int)sourceCell.Location.Y, (int)targetCell.Location.Y);
            var maxY = Math.Max((int)sourceCell.Location.Y, (int)targetCell.Location.Y);

            Log.DebugFormat("Ownership for Cells ({0},{1}) - ({2},{3}) to be set to {4}", minX, minY, maxX, maxY, newOwner.Name);

            foreach (var cell in board.Cells.Where(x =>
                (int)x.Location.X >= minX &&
                (int)x.Location.Y >= minY &&
                (int)x.Location.X <= maxX &&
                (int)x.Location.Y <= maxY))
            {
                if (cell.Owner == newOwner && cell.Type != CellType.Wall) continue;

                Log.DebugFormat("new owner ({0},{1}) = {2}", cell.CurrentLocation.X, cell.CurrentLocation.Y, cell.Owner.Name);

                var instruction = new Instruction(InstructionType.ChangeOwnership, new GridPosition
                {
                    X = (int)cell.Location.X,
                    Y = (int)cell.Location.Y
                })
                { Owner = newOwner };

                board.Instructions.Add(instruction);
                cell.Owner = newOwner;
            }
        }

        public virtual bool AttackCells(ICell sourceCell, ICell targetCell, IBoard board)
        {
            if (!sourceCell.ContainsPlayablePiece) return false;

            if (targetCell.Type == CellType.Wall) return false;

            if (targetCell.Value != null)
            {
                var targetWeightedHealth = targetCell.Health * targetCell.Value;
                Log.DebugFormat("Health of target: {0}", targetWeightedHealth);

                var sourceWeightedStrength = sourceCell.Strength * sourceCell.Value;
                Log.DebugFormat("Strength of source: {0}", sourceWeightedStrength);

                var newTargetHealth = targetWeightedHealth - sourceWeightedStrength;

                if (newTargetHealth > 0)
                {
                    targetCell.Health = (newTargetHealth / targetCell.Value).Value;

                    targetCell.Status = MergeStatus.Attacked;
                    return true;
                }
                targetCell.Value = sourceCell.Value;
                targetCell.Owner = sourceCell.Owner;

                sourceCell.Status = MergeStatus.Merged;
                sourceCell.Value = null;

                return true;
            }

            targetCell.Value = (targetCell.Value ?? 0) + sourceCell.Value;
            sourceCell.Value = null;

            return true;
        }

        public virtual MoveStatus Move(MoveDirection direction, IPlayer player)
        {
            var validMove = MoveStatus.Invalid;
            foreach (var board in _boards)
            {
                validMove = Move(direction, board, player);
            }

            return validMove;
        }

        public MoveStatus CheckStatus(IBoard board, IPlayer player)
        {
            var width = board.Width;
            var height = board.Height;

            if (board.ContainsNoPieces()) return MoveStatus.Invalid;

            if (board.ContainsNoPieces(player)) return MoveStatus.Invalid;

            if (!board.IsFull()) return MoveStatus.Valid;

            // left move
            // work through each number from left to right move each that can move left (position 0 or space on left) (flag that something has moved)
            for (var row = 0; row < height; row++)
            {
                for (var currCol = 0; currCol < width - 1; currCol++)
                {
                    var currentCell = board.Arena[row][currCol];
                    currentCell.Status = MergeStatus.None;

                    for (var testCol = currCol + 1; testCol < width; testCol++)
                    {
                        var testCell = board.Arena[row][testCol];
                        testCell.Status = MergeStatus.None;

                        if (testCell.Type == CellType.Wall) break;

                        var thisIsValid = CheckCanMergeCells(testCell, currentCell, board);
                        if (thisIsValid)
                        {
                            return MoveStatus.Valid;
                        }

                        if (testCell.ContainsPlayablePiece) break;
                    }
                }
            }

            for (var row = 0; row < height; row++)
            {
                for (var currCol = width - 1; currCol > 0; currCol--)
                {
                    var currentCell = board.Arena[row][currCol];
                    currentCell.Status = MergeStatus.None;

                    for (var testCol = currCol - 1; testCol >= 0; testCol--)
                    {
                        var testCell = board.Arena[row][testCol];
                        testCell.Status = MergeStatus.None;

                        if (testCell.Type == CellType.Wall) break;

                        var thisIsValid = CheckCanMergeCells(testCell, currentCell, board);
                        if (thisIsValid)
                        {
                            return MoveStatus.Valid;
                        }

                        if (testCell.ContainsPlayablePiece) break;

                        if (testCell.Status != MergeStatus.Merged) continue;

                        currentCell.Status = MergeStatus.Merged;
                        break;
                    }
                }

                for (var col = 0; col < width; col++)
                {
                    for (var currRow = 0; currRow < height - 1; currRow++)
                    {
                        var currentCell = board.Arena[currRow][col];
                        currentCell.Status = MergeStatus.None;

                        for (var testRow = currRow + 1; testRow < height; testRow++)
                        {
                            var testCell = board.Arena[testRow][col];
                            testCell.Status = MergeStatus.None;

                            if (testCell.Type == CellType.Wall) break;

                            var thisIsValid = CheckCanMergeCells(testCell, currentCell, board);
                            if (thisIsValid)
                            {
                                return MoveStatus.Valid;
                            }

                            if (testCell.ContainsPlayablePiece) break;

                            if (testCell.Status != MergeStatus.Merged) continue;

                            currentCell.Status = MergeStatus.Merged;
                            break;
                        }
                    }
                }
            }

            for (var col = 0; col < width; col++)
            {
                for (var currRow = height - 1; currRow > 0; currRow--)
                {
                    var currentCell = board.Arena[currRow][col];
                    currentCell.Status = MergeStatus.None;

                    for (var testRow = currRow - 1; testRow >= 0; testRow--)
                    {
                        var testCell = board.Arena[testRow][col];
                        testCell.Status = MergeStatus.None;

                        if (testCell.Type == CellType.Wall) break;

                        var thisIsValid = CheckCanMergeCells(testCell, currentCell, board);
                        if (thisIsValid)
                        {
                            return MoveStatus.Valid;
                        }

                        if (testCell.ContainsPlayablePiece) break;

                        if (testCell.Status != MergeStatus.Merged) continue;

                        currentCell.Status = MergeStatus.Merged;
                        break;
                    }
                }
            }

            return MoveStatus.Invalid;
        }

        public MoveStatus Attack(MoveDirection direction, IBoard targetBoard, IPlayer player)
        {
            CurrentPlayer = player;

            var sourceBoard = _boards.FirstOrDefault(x => x.Owner.Name == player.Name);

            var attack = new AttackDetails { Direction = direction, Owner = player, SourceBoard = sourceBoard, TargetBoard = targetBoard };

            if (sourceBoard == null) return MoveStatus.Valid;

            if (!CanAttack(direction, sourceBoard, player)) return MoveStatus.Invalid;

            //DisplayGridInDebug(sourceBoard);
            Log.DebugFormat("------------------");
            //DisplayGridInDebug(targetBoard);

            var numberOfLimboCellsBefore = GameLimboCells.Count();

            var validMove = MoveStatus.Invalid;

            var valid = GatherAttackingPieces(attack);

            Log.DebugFormat("//gather attack pieces result is {0}", valid);

            if (valid != MoveStatus.Invalid) validMove = valid;

            valid = Move(direction, sourceBoard, player);

            Log.DebugFormat("//moving pieces on source board result is {0}", valid);

            if ((valid != MoveStatus.Invalid || GameLimboCells.Count() == numberOfLimboCellsBefore)) validMove = MoveStatus.Valid;

            valid = Move(direction, targetBoard, player);

            Log.DebugFormat("//moving pieces on target board result is {0}", valid);

            if (valid != MoveStatus.Invalid) validMove = valid;

            Log.DebugFormat("//overall move is {0}", valid);

            return validMove;
        }

        private MoveStatus GatherAttackingPieces(AttackDetails attack)
        {
            switch (attack.Direction)
            {
                case MoveDirection.Left:
                    GatherPiecesForLeftAttack(attack);
                    break;

                case MoveDirection.Right:
                    GatherPiecesForRightAttack(attack);
                    break;

                case MoveDirection.Up:
                    GatherPiecesForUpAttack(attack);
                    break;

                case MoveDirection.Down:
                    GatherPiecesForDownAttack(attack);
                    break;
            }

            return MoveStatus.Valid;
        }

        private void GatherPiecesForLeftAttack(AttackDetails attack)
        {
            var width = attack.SourceBoard.Width;
            var height = attack.SourceBoard.Height;

            for (var row = 0; row < height; row++)
            {
                for (var currCol = 0; currCol < width - 1; currCol++)
                {
                    var currentCell = attack.SourceBoard.Arena[row][currCol];
                    currentCell.Status = MergeStatus.None;

                    if (currentCell.Type == CellType.Wall)
                    {
                        continue;
                    }

                    if (currentCell.ContainsPlayablePiece)
                    {
                        if (currentCell.Owner == attack.Owner)
                        {
                            Log.DebugFormat("Adding Cell ({0},{1}) to limbo", currCol, row);

                            PutAttackPiecesInLimbo(attack, currentCell);
                        }

                        break;
                    }
                }
            }
        }

        private void GatherPiecesForRightAttack(AttackDetails attack)
        {
            var width = attack.SourceBoard.Width;
            var height = attack.SourceBoard.Height;

            for (var row = 0; row < height; row++)
            {
                for (var currCol = width - 1; currCol > 0; currCol--)
                {
                    var currentCell = attack.SourceBoard.Arena[row][currCol];
                    currentCell.Status = MergeStatus.None;

                    if (currentCell.Type == CellType.Wall)
                    {
                        continue;
                    }

                    if (currentCell.ContainsPlayablePiece)
                    {
                        if (currentCell.Owner != attack.Owner)
                        {
                            break;
                        }

                        Log.DebugFormat("Adding Cell ({0},{1}) to limbo", currCol, row);

                        PutAttackPiecesInLimbo(attack, currentCell);

                        break;
                    }
                }
            }
        }

        private void GatherPiecesForUpAttack(AttackDetails attack)
        {
            var width = attack.SourceBoard.Width;
            var height = attack.SourceBoard.Height;

            for (var currCol = 0; currCol < width - 1; currCol++)
            {
                for (var row = 0; row < height - 1; row++)
                {
                    var currentCell = attack.SourceBoard.Arena[row][currCol];
                    currentCell.Status = MergeStatus.None;

                    if (currentCell.Type == CellType.Wall)
                    {
                        continue;
                    }

                    if (currentCell.ContainsPlayablePiece)
                    {
                        if (currentCell.Owner == attack.Owner)
                        {
                            Log.DebugFormat("Adding Cell ({0},{1}) to limbo", currCol, row);

                            PutAttackPiecesInLimbo(attack, currentCell);
                        }

                        break;
                    }
                }
            }
        }

        private void GatherPiecesForDownAttack(AttackDetails attack)
        {
            var width = attack.SourceBoard.Width;
            var height = attack.SourceBoard.Height;

            for (var col = 0; col < width - 1; col++)
            {
                for (var curRow = height - 1; curRow > 0; curRow--)
                {
                    var currentCell = attack.SourceBoard.Arena[curRow][col];
                    currentCell.Status = MergeStatus.None;

                    if (currentCell.Type == CellType.Wall)
                    {
                        continue;
                    }

                    if (currentCell.ContainsPlayablePiece)
                    {
                        if (currentCell.Owner == attack.Owner)
                        {
                            Log.DebugFormat("Adding Cell ({0},{1}) to limbo", col, curRow);

                            PutAttackPiecesInLimbo(attack, currentCell);
                        }

                        break;
                    }
                }
            }
        }

        private void PutAttackPiecesInLimbo(AttackDetails attack, ICell currentCell)
        {
            var locationMaskX = 1;
            var locationMaskY = 1;

            var moveLocationMaskX = attack.SourceBoard.Width - 1;
            var moveLocationMaskY = attack.SourceBoard.Height - 1;

            if (attack.Direction == MoveDirection.Left || attack.Direction == MoveDirection.Right)
            {
                locationMaskX = 0;
                moveLocationMaskY = (int)currentCell.Location.Y;
            }
            else
            {
                moveLocationMaskX = (int)currentCell.Location.X;
                locationMaskY = 0;
            }

            if (attack.Direction == MoveDirection.Left)
            {
                moveLocationMaskX = 0;
            }
            else if (attack.Direction == MoveDirection.Up)
            {
                moveLocationMaskY = 0;
            }

            var containsMatchingCell = GameLimboCells.Any(x => x.AttackDirection == attack.Direction
                                                               && x.Source.Owner.Equals(attack.SourceBoard.Owner)
                                                               && x.Target.Owner.Equals(attack.TargetBoard.Owner)
                                                               && ((int)x.Location.X * locationMaskX) == ((int)currentCell.Location.X * locationMaskX)
                                                               && ((int)x.Location.Y * locationMaskY) == ((int)currentCell.Location.Y * locationMaskY));

            if (containsMatchingCell) return;

            var instruction = new Instruction(InstructionType.Move, new GridPosition
            {
                X = (int)currentCell.Location.X,
                Y = (int)currentCell.Location.Y
            },
            new GridPosition
            {
                X = moveLocationMaskX,
                Y = moveLocationMaskY
            });

            attack.SourceBoard.Instructions.Add(instruction);

            instruction = new Instruction(InstructionType.Delete, new GridPosition
            {
                X = (int)currentCell.Location.X,
                Y = (int)currentCell.Location.Y
            });

            attack.SourceBoard.Instructions.Add(instruction);

            SetOwnershipBetweenCells(new Cell(new Vector(moveLocationMaskX, moveLocationMaskY), 0, attack.Owner), currentCell, attack.SourceBoard, attack.Owner);

            GameLimboCells.Add(new LimboCell(currentCell)
            {
                Health = currentCell.Health,
                Status = currentCell.Status,
                Strength = currentCell.Strength,
                Type = currentCell.Type,
                Source = attack.SourceBoard,
                Target = attack.TargetBoard,
                AttackDirection = attack.Direction,
                Owner = attack.Owner
            });

            currentCell.Strength = 0;
            currentCell.Value = null;
            currentCell.Health = 0;
        }

        public bool CanAttack(MoveDirection direction, IBoard targetBoard, IPlayer player)
        {
            return player.CanAttack && (!targetBoard.ContainsNoPieces(targetBoard.Owner) || FetchAttackingPieces(targetBoard, player, direction).Any());
        }

        public virtual MoveStatus Move(MoveDirection direction, IBoard board)
        {
            var validMove = MoveStatus.Invalid;
            var width = board.Width;
            var height = board.Height;

            // left move
            // work through each number from left to right move each that can move left (position 0 or space on left) (flag that something has moved)
            if (direction == MoveDirection.Left)
            {
                for (var row = 0; row < height; row++)
                {
                    for (var currCol = 0; currCol < width - 1; currCol++)
                    {
                        var currentCell = board.Arena[row][currCol];
                        currentCell.Status = MergeStatus.None;

                        for (var testCol = currCol + 1; testCol < width; testCol++)
                        {
                            var testCell = board.Arena[row][testCol];
                            testCell.Status = MergeStatus.None;

                            if (testCell.Type == CellType.Wall) break;

                            var thisIsValid = MergeCells(testCell, currentCell, board);
                            if (thisIsValid)
                            {
                                validMove = MoveStatus.Valid;
                            }

                            if (testCell.ContainsPlayablePiece && !thisIsValid) break;

                            if (testCell.Status != MergeStatus.Merged) continue;

                            currentCell.Status = MergeStatus.Merged;
                            break;
                        }
                    }
                }
            }

            // right move
            // work through each number from right to left move each that can move right (position [max] or space on right) (flag that something has moved)
            if (direction == MoveDirection.Right)
            {
                for (var row = 0; row < height; row++)
                {
                    for (var currCol = width - 1; currCol > 0; currCol--)
                    {
                        var currentCell = board.Arena[row][currCol];
                        currentCell.Status = MergeStatus.None;

                        for (var testCol = currCol - 1; testCol >= 0; testCol--)
                        {
                            var testCell = board.Arena[row][testCol];
                            testCell.Status = MergeStatus.None;

                            if (testCell.Type == CellType.Wall) break;

                            var thisIsValid = MergeCells(testCell, currentCell, board);
                            if (thisIsValid)
                            {
                                validMove = MoveStatus.Valid;
                            }

                            if (testCell.ContainsPlayablePiece && !thisIsValid) break;

                            if (testCell.Status != MergeStatus.Merged) continue;

                            currentCell.Status = MergeStatus.Merged;
                            break;
                        }
                    }
                }
            }

            // left move
            // work through each number from top to bottom move each that can move up (position 0 or space above) (flag that something has moved)
            if (direction == MoveDirection.Up)
            {
                for (var col = 0; col < width; col++)
                {
                    for (var currRow = 0; currRow < height - 1; currRow++)
                    {
                        var currentCell = board.Arena[currRow][col];
                        currentCell.Status = MergeStatus.None;

                        for (var testRow = currRow + 1; testRow < height; testRow++)
                        {
                            var testCell = board.Arena[testRow][col];
                            testCell.Status = MergeStatus.None;

                            if (testCell.Type == CellType.Wall) break;

                            var thisIsValid = MergeCells(testCell, currentCell, board);
                            if (thisIsValid)
                            {
                                validMove = MoveStatus.Valid;
                            }

                            if (testCell.ContainsPlayablePiece && !thisIsValid) break;

                            if (testCell.Status != MergeStatus.Merged) continue;

                            currentCell.Status = MergeStatus.Merged;
                            break;
                        }
                    }
                }
            }

            // down move
            // work through each number from bottom to top move each that can move down (position [max] or space below) (flag that something has moved)
            if (direction == MoveDirection.Down)
            {
                for (var col = 0; col < width; col++)
                {
                    for (var currRow = height - 1; currRow > 0; currRow--)
                    {
                        var currentCell = board.Arena[currRow][col];
                        currentCell.Status = MergeStatus.None;

                        for (var testRow = currRow - 1; testRow >= 0; testRow--)
                        {
                            var testCell = board.Arena[testRow][col];
                            testCell.Status = MergeStatus.None;

                            if (testCell.Type == CellType.Wall) break;

                            var thisIsValid = MergeCells(testCell, currentCell, board);
                            if (thisIsValid)
                            {
                                validMove = MoveStatus.Valid;
                            }

                            if (testCell.ContainsPlayablePiece && !thisIsValid) break;

                            if (testCell.Status != MergeStatus.Merged) continue;

                            currentCell.Status = MergeStatus.Merged;
                            break;
                        }
                    }
                }
            }

            if (board.ContainsNoPieces())
            {
                return board == _boards.FirstOrDefault() ? MoveStatus.GameEnded : MoveStatus.Victory;
            }

            return validMove;
        }

        public MoveStatus CheckStatus(IBoard board)
        {
            var width = board.Width;
            var height = board.Height;

            if (board.ContainsNoPieces()) return MoveStatus.GameEnded;

            if (!board.IsFull()) return MoveStatus.Valid;

            // left move
            // work through each number from left to right move each that can move left (position 0 or space on left) (flag that something has moved)
            for (var row = 0; row < height; row++)
            {
                for (var currCol = 0; currCol < width - 1; currCol++)
                {
                    var currentCell = board.Arena[row][currCol];
                    currentCell.Status = MergeStatus.None;

                    for (var testCol = currCol + 1; testCol < width; testCol++)
                    {
                        var testCell = board.Arena[row][testCol];
                        testCell.Status = MergeStatus.None;

                        if (testCell.Type == CellType.Wall) break;

                        var thisIsValid = CheckCanMergeCells(testCell, currentCell, board);
                        if (thisIsValid)
                        {
                            return MoveStatus.Valid;
                        }

                        if (testCell.ContainsPlayablePiece) break;
                    }
                }
            }

            for (var row = 0; row < height; row++)
            {
                for (var currCol = width - 1; currCol > 0; currCol--)
                {
                    var currentCell = board.Arena[row][currCol];
                    currentCell.Status = MergeStatus.None;

                    for (var testCol = currCol - 1; testCol >= 0; testCol--)
                    {
                        var testCell = board.Arena[row][testCol];
                        testCell.Status = MergeStatus.None;

                        if (testCell.Type == CellType.Wall) break;

                        var thisIsValid = CheckCanMergeCells(testCell, currentCell, board);
                        if (thisIsValid)
                        {
                            return MoveStatus.Valid;
                        }

                        if (testCell.ContainsPlayablePiece) break;

                        if (testCell.Status != MergeStatus.Merged) continue;

                        currentCell.Status = MergeStatus.Merged;
                        break;
                    }
                }

                for (var col = 0; col < width; col++)
                {
                    for (var currRow = 0; currRow < height - 1; currRow++)
                    {
                        var currentCell = board.Arena[currRow][col];
                        currentCell.Status = MergeStatus.None;

                        for (var testRow = currRow + 1; testRow < height; testRow++)
                        {
                            var testCell = board.Arena[testRow][col];
                            testCell.Status = MergeStatus.None;

                            if (testCell.Type == CellType.Wall) break;

                            var thisIsValid = CheckCanMergeCells(testCell, currentCell, board);
                            if (thisIsValid)
                            {
                                return MoveStatus.Valid;
                            }

                            if (testCell.ContainsPlayablePiece) break;

                            if (testCell.Status != MergeStatus.Merged) continue;

                            currentCell.Status = MergeStatus.Merged;
                            break;
                        }
                    }
                }
            }

            for (var col = 0; col < width; col++)
            {
                for (var currRow = height - 1; currRow > 0; currRow--)
                {
                    var currentCell = board.Arena[currRow][col];
                    currentCell.Status = MergeStatus.None;

                    for (var testRow = currRow - 1; testRow >= 0; testRow--)
                    {
                        var testCell = board.Arena[testRow][col];
                        testCell.Status = MergeStatus.None;

                        if (testCell.Type == CellType.Wall) break;

                        var thisIsValid = CheckCanMergeCells(testCell, currentCell, board);
                        if (thisIsValid)
                        {
                            return MoveStatus.Valid;
                        }

                        if (testCell.ContainsPlayablePiece) break;

                        if (testCell.Status != MergeStatus.Merged) continue;

                        currentCell.Status = MergeStatus.Merged;
                        break;
                    }
                }
            }

            return MoveStatus.Invalid;
        }

        public virtual MoveStatus Attack(MoveDirection direction)
        {
            return MoveStatus.Valid;
        }

        public virtual MoveStatus Attack(MoveDirection direction, IBoard board)
        {
            return MoveStatus.Valid;
        }

        public virtual bool CheckCanMergeCells(ICell sourceCell, ICell targetCell, IBoard board)
        {
            if (!sourceCell.ContainsPlayablePiece) return false;
            if (targetCell == null && sourceCell.Type == CellType.Playable)
            {
                return true;
            }

            if (targetCell == null) return true;

            if (targetCell.Type == CellType.Wall) return false;

            if (targetCell.Value > 2 && targetCell.Value != sourceCell.Value) return false;
            if (targetCell.Value == 1 && sourceCell.Value != 2) return false;
            if (targetCell.Value == 2 && sourceCell.Value != 1) return false;

            return true;
        }

        public virtual bool MergeCells(ICell sourceCell, ICell targetCell, IBoard board)
        {
            if (!sourceCell.ContainsPlayablePiece) return false;
            if (targetCell == null && sourceCell.Type == CellType.Playable)
            {
                sourceCell.Value = null;
                return true;
            }

            if (targetCell == null) return true;

            if (targetCell.Type == CellType.Wall) return false;

            if (targetCell.Value != null)
            {
                if (targetCell.Value > 2 && targetCell.Value != sourceCell.Value) return false;
                if (targetCell.Value == 1 && sourceCell.Value != 2) return false;
                if (targetCell.Value == 2 && sourceCell.Value != 1) return false;
            }

            targetCell.Value = (targetCell.Value ?? 0) + sourceCell.Value;
            targetCell.Health = sourceCell.Health;
            targetCell.Owner = sourceCell.Owner;

            sourceCell.Value = null;
            sourceCell.Health = 0;
            return true;
        }

        public virtual void ChangeGameMode(GameMode newMode, IBoard board)
        {
            board.GameMode = newMode;
        }

        public virtual bool IsGameMode(GameMode testMode, IBoard board)
        {
            return (board.GameMode == testMode);
        }

        public virtual bool ClearEnemyCells(IBoard board, IPlayer owner)
        {
            foreach (var enemyCell in board.Cells.Where(x => x.Owner != owner))
            {
                enemyCell.Owner = owner;
                enemyCell.Value = null;
                enemyCell.Health = 0;
            }
            return true;
        }

        public virtual void UpgradePieces(IBoard board, IPlayer owner)
        {
        }

        public virtual LimboCellCollection GameLimboCells { get; set; }

        public void ClearLimboCells()
        {
            GameLimboCells = new LimboCellCollection();
        }

        public virtual MoveStatus DeleteCell(ICell sourceCell, IBoard board)
        {
            return MoveStatus.Valid;
        }

        public virtual bool ClearOwnerCells(IBoard board, IPlayer owner)
        {
            foreach (var enemyCell in board.Cells.Where(x => x.Owner == owner))
            {
                enemyCell.Owner = owner;
                enemyCell.Value = null;
                enemyCell.Health = 0;
            }
            return true;
        }
    }
}