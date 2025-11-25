using System;
using UnityEngine;

[Serializable]
public class WeightObject
{
    [field: SerializeField]
    public GameObject GameObject {  get; set; }

    [field: SerializeField]
    public float Weight { get; set; } = 1f;
}
