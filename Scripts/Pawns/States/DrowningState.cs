using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class DrowningState : PlayerBaseState
{
    Vector3 m_EulerAngleVelocity = new Vector3(13, 0, 0);
    public override void EnterState(PlayerStateManager player)
    {
        player.DeathLocation = player.transform.position;

        player.GetComponent<Rigidbody>().isKinematic = false;

        if (player.Anim != null)
        {
            player.Anim.SetBool("isDrown", true);
        }


        player.isDead = true;
        player.GetComponent<CapsuleCollider>().enabled = true;

        player.Controller.enabled = false;


        player.TakeDamage(player.HitPoints);


    }

    public override void ExitState(PlayerStateManager player)
    {

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

        //player.transform.Translate(Vector3.down * 0.1f, Space.World);
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
        player.GetComponent<Rigidbody>().MoveRotation(player.GetComponent<Rigidbody>().rotation * deltaRotation);

    }
}