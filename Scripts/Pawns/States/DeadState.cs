using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class DeadState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.isDead = true;
        player.CurrentAIState = player.NoAIState;


        if (player.myNavMesh.enabled == true) player.myNavMesh.isStopped=true;

        if (player.Anim != null)
        {
            player.Anim.SetBool("isRunning", false);
            player.Anim.SetBool("isAiming", false);
            player.Anim.SetBool("isDead", true);
        }

        //player.MakeDead();

        player.DeathLocation = player.transform.position;

        player.GetComponent<CapsuleCollider>().enabled = false;
        player.Controller.enabled = false;
        player.UnmakeLockTarget();
    }

    public override void ExitState(PlayerStateManager player)
    {
        //Debug.Log("Exiting DEAD");
    }

    public override void execOnMouseClick(InputValue value)
    {
    }
    public override void execOnAttack(InputValue value)
    {
    }
    public override void execOnBlock(InputValue value)
    {
    }
    public override void execOnMovement(InputValue value)  //двигаем игрока, если юнит управляется игроком
    {
    }
    public override void execOnRPGStop(InputValue value) //Зажат шифт как в Диабло
    {
    }
    public override void execOnUse(InputValue value)
    {
    }
    public override void UpdateState(PlayerStateManager player)
    {



    }
}