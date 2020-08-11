using System;
using UnityEngine;

namespace Plus.CatSimulator
{
    public interface IFood
    {
        event EventHandler<EventArgs> Eat;
        void EatMe();
        Vector3 Position { get; set; }
        void SetActive(bool isActive);
    }
}