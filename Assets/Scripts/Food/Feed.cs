using System;
using UnityEngine;

namespace Plus.CatSimulator
{
    public class Feed : MonoBehaviour, IFood
    {
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public event EventHandler<EventArgs> Eat;

        public void EatMe()
        {
            Eat?.Invoke(this, new EventArgs());
        }

        public void SetActive(bool isActive)
        {
            transform.gameObject.SetActive(isActive);
        }
    }
}
