using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPt : AnyPt
{
    public PatrolPt _nextpatrolpoint;
    public PatrolPt NextPatrolPoint { get { return _nextpatrolpoint; } set { _nextpatrolpoint = value; } }


    private PatrolPt PrevPatrolPoint;
    //private PatrolPt FirstPatrolPoint;
    //[SerializeField] private bool CircularMode = true;
    //[SerializeField] private GameObject EditorModel;






}
