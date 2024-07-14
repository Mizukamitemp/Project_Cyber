using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class NoAIState : PlayerBaseAIState
{
    private float timer;
    //bool BlockingJustStarted = true;
    public override void EnterAIState(PlayerStateManager player)
    {
        timer = 0;


        player.myNavMesh.enabled = false;
        player.Controller.enabled = true;
        player.isBeingPlayed = true;
        player.clickLocation = player.transform.position;

        if (player.RPGControlStyle)
        {
            player.GetComponent<CapsuleCollider>().enabled = false;
        }
        else
        {
            player.GetComponent<CapsuleCollider>().enabled = false;
        }


    }

    public override void ExitAIState(PlayerStateManager player)
    {

    }

    public override void UpdateAIState(PlayerStateManager player)
    {
     //   timer += Time.fixedDeltaTime;


    //    if (timer > 1 && player.CurrentState != player.CargoState) 
    //    {
    //        player.SwitchAIState(player.IdleAIState);
    //    }

    }



}
