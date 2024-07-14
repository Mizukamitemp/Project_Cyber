using UnityEngine;
using UnityEngine.InputSystem;

public class DeadStateTurret : DeadState
{
    public override void EnterState(PlayerStateManager player)
    {
        base.EnterState(player);
        if (player.ModelInstance.GetComponent<ModelScript>().DeathEffect != null) player.ModelInstance.GetComponent<ModelScript>().DeathEffect.SetActive(true);
        player.Gun1.SetActive(false);
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

        player.transform.Rotate(0.0f, 25.0f, 0.0f, Space.Self);//GetComponent<Turret1Script>().

    }
}