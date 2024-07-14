
using UnityEngine.AI;
using UnityEngine;

public class PSTurrete : PlayerStateManager
{
    public override void Awake()
    {
        DeathState = new DeadStateTurret();
        IdleAIState = new IdleAIStateTurret();

        base.Awake();





    }

}
