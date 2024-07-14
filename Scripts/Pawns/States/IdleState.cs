using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static UnityEngine.Rendering.DebugUI;

public class IdleState : PlayerBaseState
{

    //bool BlockingJustStarted = true;
    public override void EnterState(PlayerStateManager player)
    {


        


        //Debug.Log("--Idle State");

        playerRef = player;

        if (playerRef.Anim != null)
        {
            playerRef.Anim.SetBool("isRunning", false);
            playerRef.Anim.SetBool("isDead", false);
            //player.Anim.SetBool("inCargo", false);

        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        //Debug.Log("Exiting IDLE");
    }

    public override void execOnMouseClick(InputValue value)
    {
    }
    public override void execOnAttack(InputValue value)
    {
        playerRef.SwitchState(playerRef.AimingState);
    }

    public override void execOnBlock(InputValue value)
    {
        playerRef.SwitchState(playerRef.BlockingState);
    }

    public override void execOnMovement(InputValue value) //двигаем игрока, если юнит управляется игроком
    {


    }
    public override void execOnRPGStop(InputValue value) //Зажат шифт как в Диабло
    {
        if (value.isPressed) 
        {
            playerRef.SwitchState(playerRef.AimingState);
        }


    }
    public override void execOnUse(InputValue value)
    {
        RaycastHit hit;
        if (Physics.Raycast(playerRef.FigureCenter.transform.position, playerRef.transform.forward, out hit, playerRef.MeleeRange * 2))
        {

            //Debug.Log($"MOUSE1" + hit.collider.name);
            if (hit.transform.tag == "ShipTransport" && hit.transform.gameObject.GetComponent<ShipController>())
            {
                playerRef.myTransport = hit.transform.gameObject.GetComponent<ShipController>();
                playerRef.SwitchState(playerRef.CargoState);

            }
        }
    }
    public override void UpdateState(PlayerStateManager player)
    {

        playerRef.DecreaseFatigue(-100);//восстанавливаем утомление
        

        if (!playerRef.IsStationary && (playerRef.InputVector.x != 0f || playerRef.InputVector.y != 0f))
        {
            playerRef.SwitchState(playerRef.WalkingState);
        }
    }
}
