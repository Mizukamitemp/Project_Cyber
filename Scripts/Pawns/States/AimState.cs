using UnityEngine;
using UnityEngine.InputSystem;

public class AimState : PlayerBaseState
{
    //private PlayerStateManager theplayer;
    public override void EnterState(PlayerStateManager player)
    {
       //Debug.Log("--Aim State");
        playerRef = player;
        //player.IsAiming = true;
        if (player.Anim != null)
        {
            player.Anim.SetBool("isAiming", true);
        }

    }

    public override void execOnMouseClick(InputValue value)
    {
        if (value.isPressed)
        {
            playerRef.SwitchState(playerRef.AttState);
        }
    }
    public override void execOnAttack(InputValue value)
    {
        //Debug.Log("--EXIT Aim State");
        playerRef.Anim.SetBool("isAiming", false);
        //playerRef.IsAiming = false;
        playerRef.SwitchState(playerRef.IdlingState);

    }
    public override void execOnBlock(InputValue value)
    {
    }

    public override void execOnMovement(InputValue value) //двигаем игрока, если юнит управляется игроком
    {
    }
    public override void execOnRPGStop (InputValue value) //Зажат шифт как в Диабло
    {
        if (!value.isPressed)
        {
            playerRef.Anim.SetBool("isAiming", false);
            //playerRef.IsAiming = false;
            playerRef.SwitchState(playerRef.IdlingState);
        }
    }

    public override void execOnUse(InputValue value) 
    {
    }
    public override void ExitState(PlayerStateManager player)
    {
    }

    public override void UpdateState(PlayerStateManager player)
    {

        if (playerRef.RPGControlStyle)
        {
            Quaternion rotation;

            // видимо, еще никуда и не шел - смотрим куда смотрели
            if (playerRef.hoverLocation.x == playerRef.transform.position.x && playerRef.hoverLocation.z == playerRef.transform.position.z)
            {
                rotation = playerRef.transform.rotation;
            }
            else // вообще, это значит, что отпустили мышь
            {
                Vector3 normalized = (playerRef.hoverLocation - playerRef.transform.position).normalized;
                normalized.y = 0f;
                rotation = Quaternion.LookRotation(normalized);
            }

            // если не совпадают локация и точка клика, то смотрим туда
            if (playerRef.hoverLocation != playerRef.transform.position)
            {
                playerRef.transform.rotation = rotation;
            }

        }


    }
}
