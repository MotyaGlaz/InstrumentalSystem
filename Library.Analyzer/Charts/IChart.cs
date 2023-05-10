namespace Library.Analyzer.Charts
{
    public interface IChart : IReadOnlyChart
    {
        bool Enqueue(int index, IState state);
    }
}