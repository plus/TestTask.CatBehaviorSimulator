﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICarpet
{
    Transform Transform { get; }
}

public class Carpet : MonoBehaviour, ICarpet
{
    public Transform Transform => transform;
}
