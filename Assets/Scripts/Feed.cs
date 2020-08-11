using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Plus.CatSimulator
{
    public interface IFood
    {
        event EventHandler<EventArgs> Eat;
        void EatMe();
        Transform Transform { get; }
    }

    public class Feed : MonoBehaviour, IFood
    {
        public Transform Transform => transform;

        public event EventHandler<EventArgs> Eat;

        public void EatMe()
        {
            Eat?.Invoke(this, new EventArgs());
        }
    }
}
