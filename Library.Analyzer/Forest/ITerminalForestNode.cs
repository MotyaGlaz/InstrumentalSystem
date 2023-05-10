namespace Library.Analyzer.Forest
{
    public interface ITerminalForestNode : IForestNode
    {
        char Capture { get; }
    }
}