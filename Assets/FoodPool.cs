using System;
using UnityEngine;
using System.Linq;

namespace Plus.CatSimulator
{
    public interface IFoodPool
    {
        IFood[] GetFoods();
    }

    public interface IFoodPoolElement
    {
        IFood Food { get; }
        bool Spawned { get; set; }
    }

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

    public class FoodPool : MonoBehaviour, IFoodPool
    {
        private IFood[] foods;
        private IFoodPoolElement[] poolElements;


        private void Start()
        {
            foods = transform.GetComponentsInChildren<IFood>();
            poolElements = new FoodPoolElement[foods.Length];
            for (int i = 0; i < foods.Length; i++)
            {
                foods[i].Eat += Food_Eat;
                foods[i].Transform.gameObject.SetActive(false);
                
                poolElements[i] = new FoodPoolElement(foods[i], false);
            }
        }

        public IFood[] GetFoods()
        {
            var result = poolElements.Where(i => !i.Spawned).Select(i => i.Food).ToArray();

            for (int i = 0; i < poolElements.Length; i++)
            {
                poolElements[i].Spawned = true;
                poolElements[i].Food.Transform.gameObject.SetActive(true);
            }

            return result;
        }

        private void Food_Eat(object sender, EventArgs e)
        {
            var poolElement = poolElements.Where(i => i.Food == sender).First();
            RemoveFood(poolElement);
        }

        private void RemoveFood(IFoodPoolElement poolElement)
        {
            poolElement.Food.Transform.gameObject.SetActive(false);
            poolElement.Spawned = false;
        }
    }
}