namespace CastlesGameControl.Environment
{
    public interface IInstruction
    {
        InstructionType Action { get; set; }
        bool Complete { get; set; }
        string Description { get; set; }
        GridPosition Destination { get; set; }
        IPlayer Owner { get; set; }
        GridPosition Source { get; set; }
        int? Value { get; set; }

        string ToString();
    }
}