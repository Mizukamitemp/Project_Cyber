
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public abstract class PlayerBaseState
{
    public PlayerStateManager playerRef;    // Это ссылка на игрока, используемая в функциях, не принимающих ссылку на игрока в виде аргумента
    public abstract void EnterState(PlayerStateManager player);
    public abstract void ExitState(PlayerStateManager player);
    public abstract void UpdateState(PlayerStateManager player);
    public abstract void execOnMouseClick(InputValue value);    // Активируется с PlayerController
    public abstract void execOnAttack(InputValue value);
    public abstract void execOnBlock(InputValue value);
    public abstract void execOnMovement(InputValue value);    //двигаем игрока, если юнит управляется игроком
    public abstract void execOnRPGStop(InputValue value);   //Зажат шифт как в Диабло
    public abstract void execOnUse(InputValue value);

}
