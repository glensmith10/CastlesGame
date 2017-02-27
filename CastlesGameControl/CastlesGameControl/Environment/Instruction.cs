namespace CastlesGameControl.Environment
{
    public enum InstructionType
    {
        Killed = -3,
        Delete = -2,
        ChangeOwnership = -1,
        Information = 0,
        Move = 1,
        New = 2,
        NewFromUpgrade = 3,
        AddHealth = 4
    }

    public struct GridPosition
    {
        public int X;
        public int Y;
    }

    public class Instruction : IInstruction
    {
        public InstructionType Action { get; set; }
        public GridPosition Source { get; set; }
        public GridPosition Destination { get; set; }
        public int? Value { get; set; }
        public string Description { get; set; }
        public bool Complete { get; set; }
        public IPlayer Owner { get; set; }

        public Instruction(InstructionType action, GridPosition source, GridPosition destination)
        {
            Action = action;
            Source = source;
            Destination = destination;
        }

        public Instruction(InstructionType action, GridPosition location) : this(action, location, new GridPosition())
        { }

        public Instruction(string description)
        {
            Action = InstructionType.Information;
            Description = description;
        }

        public override string ToString()
        {
            switch (Action)
            {
                case InstructionType.Move:
                    return $"Move piece from [{Source.X},{Source.Y}] to [{Destination.X},{Destination.Y}]";

                case InstructionType.New:
                    return $"New piece @ [{Source.X},{Source.Y}] value {Value}";

                case InstructionType.NewFromUpgrade:
                    return $"New upgraded piece @ [{Source.X},{Source.Y}] value {Value}";

                case InstructionType.ChangeOwnership:
                    return $"New owner @ [{Source.X},{Source.Y}] called {Owner.Name}";

                case InstructionType.Information:
                    return Description;

                case InstructionType.Delete:
                    return $"Delete piece @ [{Source.X},{Source.Y}]";

                case InstructionType.Killed:
                    return $"Killed piece @ [{Source.X},{Source.Y}]";

                case InstructionType.AddHealth:
                    return $"Add Health @ [{Source.X},{Source.Y}] by {Value}";

                default:
                    return "Unknown instruction";
            }
        }
    }
}