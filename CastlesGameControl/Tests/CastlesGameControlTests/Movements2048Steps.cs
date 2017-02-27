using System;
using System.Collections.Generic;
using System.Linq;
using CastlesGameControl.Environment;
using CastlesGameControl.Game;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TechTalk.SpecFlow;

namespace CastlesGameControlTests
{
    [Binding]
    public class Movements2048Steps
    {
        private Dictionary<string, MoveDirection> _directions = new Dictionary<string, MoveDirection>
        {
            { "LEFT", MoveDirection.Left },
            { "RIGHT", MoveDirection.Right },
            { "UP", MoveDirection.Up },
            { "DOWN", MoveDirection.Down }
        };

        private ILog _log;

        public Movements2048Steps()
        {
            _log = MockRepository.GenerateMock<ILog>();
        }

        [Given(@"I have a game board set up as")]
        public void GivenIHaveAGameBoardSetUpAs(Table table)
        {
            var columnCount = GetColumnCount(table);
            var rowCount = table.RowCount;

            var board1 = new Board(columnCount + 2, rowCount + 2, _log);

            var game = new TwoOhFourEightGameLogic(new List<Board> { board1 }, _log);

            PopulateBoard(table, rowCount, columnCount, board1);

            ScenarioContext.Current.Add("game", game);
            ScenarioContext.Current.Add("board1", board1);
        }

        private static void PopulateBoard(Table table, int rowCount, int columnCount, Board board1)
        {
            for (var row = 0; row < rowCount; row++)
            {
                var rowValues = table.Rows[row].Values.ToArray();
                for (var col = 0; col < columnCount; col++)
                {
                    if (!string.IsNullOrEmpty(rowValues[col]))
                    {
                        board1.Arena[row + 1][col + 1].Value = int.Parse(rowValues[col]);
                    }
                }
            }
        }

        private static int GetColumnCount(Table table)
        {
            var columnCount = 1;
            while (table.ContainsColumn($"Column{columnCount}"))
            {
                columnCount++;
            }

            columnCount--;
            return columnCount;
        }

        [When(@"I move (.*)")]
        public void WhenIMove(string direction)
        {
            var game = (TwoOhFourEightGameLogic)ScenarioContext.Current["game"];
            var board1 = (Board)ScenarioContext.Current["board1"];
            var moveDirection = _directions[direction.ToUpper()];

            var status = game.Move(moveDirection, board1);

            ScenarioContext.Current.Add("MoveStatus", status);
        }

        [Then(@"it is a valid move")]
        public void ThenItIsAValidMove()
        {
            Assert.IsTrue((MoveStatus)ScenarioContext.Current["MoveStatus"] == MoveStatus.Valid);
        }

        [Then(@"it is not a valid move")]
        public void ThenItIsNotAValidMove()
        {
            Assert.IsFalse((MoveStatus)ScenarioContext.Current["MoveStatus"] == MoveStatus.Valid);
        }

        [Then(@"the resultant game board is")]
        public void ThenTheResultantGameBoardIs(Table table)
        {
            var columnCount = GetColumnCount(table);
            var rowCount = table.RowCount;
            var expectantBoard = new Board(columnCount + 2, rowCount + 2, _log);

            PopulateBoard(table, rowCount, columnCount, expectantBoard);

            var board1 = (Board)ScenarioContext.Current["board1"];

            foreach (var board1Cell in board1.Cells)
            {
                var expectantCell = expectantBoard.Cells.Single(x => x.Location == board1Cell.Location);
                Assert.IsTrue(board1Cell.Value.Equals(expectantCell.Value));
            }
        }
    }
}