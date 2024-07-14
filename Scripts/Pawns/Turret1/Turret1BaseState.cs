
using UnityEngine;

public abstract class Turret1BaseState
{

    public abstract void EnterState(Turret1Script player);
    public abstract void ExitState(Turret1Script player);
    public abstract void UpdateState(Turret1Script player);
}
