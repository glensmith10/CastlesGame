using System.Drawing;

namespace CastlesGameControl.Environment
{
    public class Player : IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IBoard Board { get; set; }
        public Color PlayerColor { get; set; }
        public int Score { get; set; }
        public int MoveScore { get; set; }

        public bool CanAttack => true;

        public bool Equals(IPlayer player)
        {
            return Id == player?.Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}