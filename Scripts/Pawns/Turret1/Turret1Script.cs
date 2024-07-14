using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Turret1Script : MonoBehaviour
{

    public TurrIdleState IdState = new TurrIdleState();
    public TrackingState TrState = new TrackingState();
    public Turret1DeathState TurrDeathState = new Turret1DeathState();
    public Turret1BaseState CurrentState;
    public GameObject MYTarget;
    public GameObject PlasmaProj;
    public GameObject MYBarrel;
    public GameObject DestroyedEffect;
    public int hitPoints = 3;



    // Start is called before the first frame update
    void Start()
    {
        CurrentState = IdState;
        CurrentState.EnterState(this);
    }

    void FixedUpdate()
    {

        CurrentState.UpdateState(this);


    }

    public void SwitchState(Turret1BaseState state)
    {
        CurrentState.ExitState(this);
        CurrentState = state;
        state.EnterState(this);
    }


    public void TakeDamage(int damage)
    {

        if (hitPoints > 0)
        {
            hitPoints--;
        }

        if (hitPoints < 0)
        {
            hitPoints = 0;
        }

        if (hitPoints <= 0)
        {
            SwitchState(TurrDeathState);
        }
    }

}
