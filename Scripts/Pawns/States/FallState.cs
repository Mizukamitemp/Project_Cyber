using UnityEngine;
using UnityEngine.InputSystem;

public class FallState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering FALL");
    }

    public override void ExitState(PlayerStateManager player)
    {
        Debug.Log("Exiting FALL");
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
    public override void execOnMovement(InputValue value) //двигаем игрока, если юнит управляется игроком
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
        if (player.Controller.isGrounded)
        {
            player.SwitchState(player.IdlingState);
        }
        else
        {
            //player.Move();
        }


    }
}