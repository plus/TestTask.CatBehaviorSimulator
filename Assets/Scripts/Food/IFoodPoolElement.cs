namespace Plus.CatSimulator
{
    public interface IFoodPoolElement
    {
        IFood Food { get; }
        bool Spawned { get; set; }
    }
}