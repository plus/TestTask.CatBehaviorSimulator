namespace Plus.CatSimulator
{
    public class FoodPoolElement : IFoodPoolElement
    {
        public IFood Food { get; }
        public bool Spawned { get; set; }

        public FoodPoolElement(IFood food, bool spawned)
        {
            Food = food;
            Spawned = spawned;
        }
    }
}