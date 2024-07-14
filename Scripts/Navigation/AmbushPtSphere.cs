using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbushPtSphere : AnyPt
{
    [SerializeField] private float _positionradius = 10; // как далеко от центра точки прячемся
    public float PositionRadius { get { return _positionradius; } set { _positionradius = value; } }
}
