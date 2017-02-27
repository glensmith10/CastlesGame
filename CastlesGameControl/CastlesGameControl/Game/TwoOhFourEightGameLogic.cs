using CastlesGameControl.Environment;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CastlesGameControl.Game
{
    public class TwoOhFourEightGameLogic : GameLogicBase
    {
        public TwoOhFourEightGameLogic(ILog log) : base(log)
        {
        }

        public TwoOhFourEightGameLogic(IEnumerable<Board> boards, ILog log)
            : base(boards, log)
        {
        }

        public override bool MergeCells(ICell sourceCell, ICell targetCell, IBoard board)
        {
            Log.DebugFormat("Cell ({0},{1}) merging with ({2},{3})", sourceCell.Location.X, sourceCell.Location.Y, targetCell.Location.X, targetCell.Location.Y);

            if (!SourceCellIsValidForMove(sourceCell)) return false;

            if (!TargetCellIsValidForMove(sourceCell, targetCell)) return false;

            if (!targetCell.Value.HasValue)
            {
                targetCell.CurrentLocation = sourceCell.Location;

                AddMoveInstruction(sourceCell.Location, targetCell.Location, board);

                targetCell.Health = sourceCell.Health;
                targetCell.Strength = sourceCell.Strength;
                targetCell.Status = MergeStatus.Moved;

                sourceCell.Status = MergeStatus.Moved;
            }
            else
            {
                AddMoveInstruction(sourceCell.Location, targetCell.Location, board);
                AddDeleteInstruction(sourceCell.Location, board);

                AddDeleteInstruction(
                    targetCell.Status == MergeStatus.Moved ? targetCell.CurrentLocation : targetCell.Location, board);

                var newValue = (sourceCell.Value ?? 0) + (targetCell.Value ?? 0);

                AddUpgradeInstruction(targetCell.Location, board, newValue);

                if (CurrentPlayer != null)
                {
                    CurrentPlayer.MoveScore = 0;
                    CurrentPlayer.MoveScore = newValue * 2;
                    CurrentPlayer.Score += newValue * 2;
                }

                targetCell.Health = 100;
                targetCell.Strength = (sourceCell.Strength + targetCell.Strength) / 2;

                sourceCell.Status = MergeStatus.Merged;
            }

            targetCell.Value = (targetCell.Value ?? 0) + sourceCell.Value;
            targetCell.Owner = sourceCell.Owner;

            sourceCell.Value = null;
            sourceCell.Health = 0;
            sourceCell.Strength = 0;

            return true;
        }

        private static void AddUpgradeInstruction(Vector location, IBoard board, int newValue)
        {
            var instruction = new Instruction(InstructionType.NewFromUpgrade, new GridPosition
            {
                X = (int)location.X,
                Y = (int)location.Y
            })
            { Value = newValue };
            board.Instructions.Add(instruction);
        }

        private static bool TargetCellIsValidForMove(ICell sourceCell, ICell targetCell)
        {
            if (targetCell.Type == CellType.Wall) return false;

            return targetCell.Value == null || targetCell.Value == sourceCell.Value;
        }

        private static bool SourceCellIsValidForMove(ICell sourceCell)
        {
            return sourceCell.ContainsPlayablePiece;
        }

        private static void AddDeleteInstruction(Vector location, IBoard board)
        {
            var instruction = new Instruction(InstructionType.Delete, new GridPosition
            {
                X = (int)location.X,
                Y = (int)location.Y
            });

            board.Instructions.Add(instruction);
        }

        private static void AddMoveInstruction(Vector sourceLocation, Vector targetLocation, IBoard board)
        {
            var instruction = new Instruction(InstructionType.Move,
                new GridPosition
                {
                    X = (int)sourceLocation.X,
                    Y = (int)sourceLocation.Y
                },
                new GridPosition
                {
                    X = (int)targetLocation.X,
                    Y = (int)targetLocation.Y
                });

            board.Instructions.Add(instruction);
        }

        public override bool AttackCells(ICell sourceCell, ICell targetCell, IBoard board)
        {
            Log.DebugFormat("Cell ({0},{1}) attacking ({2},{3})", sourceCell.Location.X, sourceCell.Location.Y, targetCell.Location.X, targetCell.Location.Y);

            if (SourceCellIsValidForMove(sourceCell)) return false;

            if (targetCell.Type == CellType.Wall) return false;

            if (targetCell.Value == null) Log.DebugFormat("targetCell.Value == null");

            if (targetCell.Value != null)
            {
                var targetWeightedHealth = targetCell.Health * targetCell.Value;
                var sourceWeightedStrength = sourceCell.Strength * sourceCell.Value;
                var newTargetHealth = targetWeightedHealth - sourceWeightedStrength;

                if (newTargetHealth > 0)
                {
                    var healthIncrease = (newTargetHealth / targetCell.Value).Value - targetCell.Health;
                    Log.DebugFormat("Health increased by: {0}", healthIncrease);

                    AddRaiseHealthInstruction(targetCell.Location, board, healthIncrease);

                    targetCell.Health = (newTargetHealth / targetCell.Value).Value;
                    targetCell.Status = MergeStatus.Attacked;

                    return true;
                }

                var newValue = targetCell.Value ?? 0;

                targetCell.Value = sourceCell.Value;
                targetCell.Owner = sourceCell.Owner;
                targetCell.Health = sourceCell.Health;
                targetCell.Strength = sourceCell.Strength;

                Log.DebugFormat("Killed object @({0},{1})", targetCell.Location.X, targetCell.Location.Y);

                AddKilledInstruction(targetCell.Location, board, targetCell.Owner, targetCell.Value);

                if (CurrentPlayer != null)
                {
                    CurrentPlayer.MoveScore = 0;
                    CurrentPlayer.MoveScore = newValue * 2;
                    CurrentPlayer.Score += newValue * 2;
                }

                AddMoveInstruction(sourceCell.Location, targetCell.Location, board);

                sourceCell.Status = MergeStatus.Merged;
                sourceCell.Value = null;
                sourceCell.Health = 0;

                return true;
            }

            targetCell.Value = (targetCell.Value ?? 0) + sourceCell.Value;
            sourceCell.Value = null;

            return true;
        }

        private static void AddKilledInstruction(Vector location, IBoard board, IPlayer player, int? value)
        {
            var instruction = new Instruction(InstructionType.Killed, new GridPosition
            {
                X = (int)location.X,
                Y = (int)location.Y
            })
            {
                Owner = player,
                Value = value
            };
            board.Instructions.Add(instruction);
        }

        private static void AddRaiseHealthInstruction(Vector location, IBoard board, int healthIncrease)
        {
            var instruction = new Instruction(InstructionType.AddHealth, new GridPosition
            {
                X = (int)location.X,
                Y = (int)location.Y
            })
            {
                Value = healthIncrease
            };

            board.Instructions.Add(instruction);
        }

        public override MoveStatus DeleteCell(ICell sourceCell, IBoard board)
        {
            var instruction = new Instruction(InstructionType.Delete, new GridPosition
            {
                X = (int)sourceCell.Location.X,
                Y = (int)sourceCell.Location.Y
            });
            board.Instructions.Add(instruction);
            sourceCell.Value = null;

            return MoveStatus.Valid;
        }

        public override bool ClearEnemyCells(IBoard board, IPlayer owner)
        {
            foreach (var enemyCell in board.Cells.Where(x => x.Owner != owner))
            {
                if (enemyCell.Value != null)
                {
                    var instruction = new Instruction(InstructionType.Delete, new GridPosition
                    {
                        X = (int)enemyCell.Location.X,
                        Y = (int)enemyCell.Location.Y
                    })
                    { Owner = enemyCell.Owner, Value = enemyCell.Value };
                    board.Instructions.Add(instruction);
                }
                enemyCell.Owner = owner;
                enemyCell.Value = null;
                enemyCell.Health = 0;
            }

            DestroyAttackingPieces(board);

            return true;
        }

        public override void UpgradePieces(IBoard board, IPlayer owner)
        {
            foreach (var ownerCell in board.Cells.Where(x => x.Owner == owner))
            {
                if (ownerCell.Value != null)
                {
                    ownerCell.Owner = owner;
                    ownerCell.Health = 100;

                    var instruction = new Instruction(InstructionType.NewFromUpgrade, new GridPosition
                    {
                        X = (int)ownerCell.Location.X,
                        Y = (int)ownerCell.Location.Y
                    })
                    { Owner = ownerCell.Owner, Value = ownerCell.Value * 2 };
                    board.Instructions.Add(instruction);

                    instruction = new Instruction(InstructionType.Delete, new GridPosition
                    {
                        X = (int)ownerCell.Location.X,
                        Y = (int)ownerCell.Location.Y
                    })
                    { Owner = ownerCell.Owner, Value = ownerCell.Value };
                    board.Instructions.Add(instruction);

                    ownerCell.Value *= 2;
                }
            }
        }

        public override bool ClearOwnerCells(IBoard board, IPlayer owner)
        {
            foreach (var ownerCell in board.Cells.Where(x => x.Owner == owner))
            {
                if (ownerCell.Value != null)
                {
                    var instruction = new Instruction(InstructionType.Delete, new GridPosition
                    {
                        X = (int)ownerCell.Location.X,
                        Y = (int)ownerCell.Location.Y
                    })
                    { Owner = ownerCell.Owner, Value = ownerCell.Value };
                    board.Instructions.Add(instruction);
                }
                ownerCell.Owner = owner;
                ownerCell.Value = null;
                ownerCell.Health = 0;
            }

            return true;
        }

        public void DestroyAttackingPieces(IBoard board)
        {
            var attackingPieces = GameLimboCells.Where(x => x.ContainsPlayablePiece
                                                            && x.Target.Owner.Equals(board.Owner)).ToList();

            foreach (var attackPiece in attackingPieces)
            {
                var instruction = new Instruction(InstructionType.Delete, new GridPosition
                {
                    X = (int)attackPiece.Location.X,
                    Y = (int)attackPiece.Location.Y
                })
                {
                    Owner = attackPiece.Owner,
                    Value = attackPiece.Value
                };
                board.Instructions.Add(instruction);
            }

            foreach (var attackingPiece in GameLimboCells.Where(x => x.ContainsPlayablePiece
                                                            && x.Target.Owner.Equals(board.Owner)).ToList())
            {
                GameLimboCells.Remove(attackingPiece);
            }
        }
    }
}