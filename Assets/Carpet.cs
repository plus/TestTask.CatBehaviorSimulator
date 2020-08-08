using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plus.CatSimulator
{
    public interface ICarpet
    {
        Transform Transform { get; }
    }

    public class Carpet : MonoBehaviour, ICarpet
    {
        public Transform Transform => transform;
    }
}