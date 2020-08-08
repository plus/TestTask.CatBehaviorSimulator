using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBall
{
    Transform Transform { get; }
}

public class Ball : MonoBehaviour, IBall
{
    public Transform Transform => transform;
}
