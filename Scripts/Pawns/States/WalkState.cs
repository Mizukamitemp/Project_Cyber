using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class WalkState : PlayerBaseState
{
    private Vector2 InputVector; // Получаем сюда инпут с PlayerController
    private Vector3 MoveVector;
    public override void EnterState(PlayerStateManager player)
    {
        //Debug.Log("Entering WALK");
        playerRef = player;

        if (player.Anim != null)
        {
            playerRef.Anim.SetBool("isAiming", false);
            playerRef.Anim.SetBool("isRunning", true);
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        //Debug.Log("Exiting WALK");


        if (player.Anim != null)
        {
            player.Anim.SetBool("isRunning", false);
        }

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
    }

    public override void UpdateState(PlayerStateManager player)
    {
        playerRef.DecreaseFatigue(100);// утомление



        // InputVector = value.Get<Vector2>(); // получили с клавиатуры WASD

        if (!playerRef.RPGControlStyle && playerRef.isBeingPlayed)
        {

            // Идем в состояние Idle если не жмем кнопки
            if (playerRef.InputVector.x == 0f && playerRef.InputVector.y == 0f)
            {

                playerRef.SwitchState(playerRef.IdlingState);
            }


            playerRef.ReceivedRotation = Quaternion.Euler(0f, playerRef.ReceivedRotation.eulerAngles.y, 0f); // Ежесекундно получаем с камеры с PlayerController
            MoveVector = playerRef.ReceivedRotation * new Vector3(playerRef.InputVector.x, 0f, playerRef.InputVector.y); // Будем идти туда куда смотрит кам

            if (playerRef.LockTarget != null)   // Если есть залоченная цель
            {
                // Задаем поворот на залоченную цель
                Vector3 worldPosition = new Vector3(playerRef.LockTarget.transform.position.x, playerRef.transform.position.y, playerRef.LockTarget.transform.position.z);
                playerRef.transform.LookAt(worldPosition);
            }
            else if (playerRef.InputVector.x != 0f || playerRef.InputVector.y != 0f)  //Нет залоченной цели и есть ввод с клавы
            {
                // Задаем поворот на направление движения
                playerRef.transform.rotation = Quaternion.RotateTowards(playerRef.transform.rotation, Quaternion.LookRotation(MoveVector), 500f * Time.fixedDeltaTime);
            }
            //return;
        }

        else if (playerRef.RPGControlStyle)
        {
            


            Quaternion rotation;

            // видимо, еще никуда и не шел - смотрим куда смотрели
            if (playerRef.clickLocation.x == playerRef.transform.position.x && playerRef.clickLocation.z == playerRef.transform.position.z)
            {
                
                rotation = playerRef.transform.rotation;
                
            }
            else // вообще, это значит, что отпустили мышь
            {
                Vector3 normalized = (playerRef.clickLocation - playerRef.transform.position).normalized;
                normalized.y = 0f;
                rotation = Quaternion.LookRotation(normalized);
                
            }

            // если не совпадают локация и точка клика, то смотрим туда
            if (playerRef.clickLocation != playerRef.transform.position)
            {
                playerRef.transform.rotation = rotation;
            }


          // далее - муть с SHIFT в Diablo


            // если не у цели и шифт не зажат
            if (Vector3.Distance(playerRef.clickLocation, playerRef.transform.position) > 1f && !playerRef.RPGControlStopped)
            {
                if (playerRef.RPGControlWasStopped) // но ранее БЫЛАОСТАНОВКА (отпустили шифт)
                {
                    playerRef.clickLocation = playerRef.transform.position;
                }
                else // но ранее НЕ БЫЛО СТАНОВКИ - тупо идем
                {
                    MoveVector = playerRef.transform.forward;
                }
                playerRef.RPGControlWasStopped = false;
            }
            else // у цели либо шифт зажат: не двигаемся и заявляем что БЫЛАОСТАНОВКА для следующей итерации
            {

                MoveVector = Vector3.zero;
                playerRef.RPGControlWasStopped = true;
                playerRef.SwitchState(playerRef.IdlingState);
            }
        }

        // Перемещаем игрока с помощью CharacterController
        if (playerRef.isBeingPlayed && playerRef.Controller.enabled) playerRef.Controller.Move(playerRef.WalkSpeed * MoveVector * Time.deltaTime);



    }
}