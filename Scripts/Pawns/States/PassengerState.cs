using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PassengerState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        playerRef = player;

        if (playerRef.myTransport.CanAcceptPassenger() )
        {
            playerRef.CargoLocation = playerRef.myTransport.GetPassengerStation();
            //myTransport = ship;
            //ship.GetPassengerStation();

            //makeCargoTrue();



            Debug.Log("ENTER");

            playerRef.transform.localScale = playerRef.transform.localScale * 0.5f;

            player.GetComponent<Transform>().transform.position = player.CargoLocation.GetComponent<Transform>().position;
            player.GetComponent<Transform>().transform.rotation = player.CargoLocation.GetComponent<Transform>().rotation;

            if (player.Anim != null)
            {
                player.Anim.SetTrigger("inCargo");
            }

            

            playerRef.Controller.enabled = false;
            playerRef.myNavMesh.enabled = false;

            playerRef.hasMoveOrders = false;

            playerRef.SwitchAIState(playerRef.NoAIState);


        }
        else
        {
            //playerRef.myTransport == null;
            Debug.Log("ENTER FAIL");
            playerRef.SwitchState(playerRef.IdlingState);
        }







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
    public override void ExitState(PlayerStateManager player)
    {



        Debug.Log("EXIT");
        playerRef.transform.position = playerRef.UnboardLocation;
        playerRef.transform.localScale = playerRef.transform.localScale * 2;

        playerRef.CargoLocation.GetComponent<ShipStation>().IsOccupied = false;
        playerRef.CargoLocation.GetComponent<CapsuleCollider>().enabled = true;

        playerRef.GuardLocation = playerRef.transform.position;
        playerRef.moveDestination = playerRef.transform.position;
        playerRef.hasMoveOrders = false;

        playerRef.myTransport.Unload();


        if (playerRef.isBeingPlayed)
        {
            playerRef.makePlayable();
        }
        else
        {
            playerRef.makeUnplayable();
        }
        


    }




    public override void UpdateState(PlayerStateManager player)
    {
        if (playerRef.myTransport != null)
        {
            playerRef.myTransport.GetInputVector(playerRef.InputVector);
        }
        player.GetComponent<Transform>().transform.position = player.CargoLocation.GetComponent<Transform>().position;
        player.GetComponent<Transform>().transform.rotation = player.CargoLocation.GetComponent<Transform>().rotation;
    }
}
