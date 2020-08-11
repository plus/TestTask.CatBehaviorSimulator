namespace Plus.CatSimulator
{
    public interface ICatAction
    {
        void Run();
    }

    public abstract class AbstractCatAction : ICatAction
    {
        public abstract void Run();
    }
}