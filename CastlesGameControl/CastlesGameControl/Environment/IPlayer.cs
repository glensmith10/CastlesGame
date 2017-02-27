using System.Drawing;

namespace CastlesGameControl.Environment
{
    public interface IPlayer
    {
        IBoard Board { get; set; }
        bool CanAttack { get; }
        int Id { get; set; }
        int MoveScore { get; set; }
        string Name { get; set; }
        Color PlayerColor { get; set; }
        int Score { get; set; }

        bool Equals(IPlayer player);

        string ToString();
    }
}