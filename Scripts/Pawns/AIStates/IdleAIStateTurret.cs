using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class IdleAIStateTurret : IdleAIState
{
    private float timer;
    
    public override void EnterAIState(PlayerStateManager player)
    {
        //Debug.Log("0. idle turret");
        timer = 0;



        player.MaybeDisableAi();    //внезапно здох

        // Если это не игрок
        if (!player.isBeingPlayed)
        {


            // Это сокр. MakeUnplayable Из PlayerStateManager
            player.myNavMesh.enabled = true;
            player.Controller.enabled = false;
            player.isBeingPlayed = false;
            player.InputVector = Vector2.zero;
            player.GetComponent<CapsuleCollider>().enabled = true;








            // Просто назначили MYTarget и hasTarget (проверяем через него)
            CheckForTargets(player);



            // Условия атаки, если появилась цель в CheckForTargets
            // Иначе игнорируем возможность атаки 
            if (
                // Нет приказа идти, не сторожит или сторожит и ушел за дистанцию преследования
                (!player.hasMoveOrders && player.HasTarget() && (!player.IsGuardingThatPoint || !player.checkExceededPursuitDistance()))
                ||
                // Есть приказ идти и не стоит режим не атаковать при движении
                (player.hasMoveOrders && player.HasTarget() && !player.GetIgnoreWhenMoving())
                ||
                // Уже нет приказа идти (дошел) и стоит режим не атаковать при движении
                (!player.hasMoveOrders && player.HasTarget() && player.GetIgnoreWhenMoving())

               )
            {
                //player.SwitchAIState(player.AttackAIState);

            }
            else if (!player.IsStationary &&
                    (player.IsGuardingThatPoint && player.checkAwayFromGuardPoint() == true && !player.HasTarget())
                    ||
                    (player.IsGuardingThatPoint && player.checkExceededPursuitDistance() == true && player.HasTarget())
                    ||
                    (player.hasMoveOrders && player.checkAwayFromGuardPoint() == true)
                    ) // он сторож и возвращается на стартовую точку
            {
                //  Debug.Log("AI: RETURNING");
                // if (player.guardingThatPoint && player.checkAwayFromGuardPoint() == true && !player.HasTarget()) Debug.Log("AI: RETURNING AFTER KILL");
                // if (player.guardingThatPoint && player.checkExceededPursuitDistance() == true && player.HasTarget()) Debug.Log("EXCEEDED PURS");
                // if (player.hasMoveOrders && player.checkAwayFromGuardPoint() == true) Debug.Log("MOVING ORDERS");


                //player.SwitchAIState(player.PriorityMoveAIState); // мы - башня!
                //Debug.Log("MAd1!!!!!");
            }
            else
            {
                //Debug.Log("NO ORDERS");
                player.moveDestination = player.transform.position;
                player.myNavMesh.isStopped = true;
                player.SwitchState(player.IdlingState);
            }

        }
    }

    
    /*
    private bool CanHitTarget(PlayerStateManager playerRef)
    {
        Vector3 TargetTruePosition = playerRef.MYTarget.transform.position;
        TargetTruePosition.y += 0.7f;
        Vector3 TruePosition = playerRef.transform.position;
        TruePosition.y += 0.7f;
        Vector3 toTarget = TargetTruePosition - TruePosition;

        //Debug.Log("0. can hit");
        if (Vector3.Distance(playerRef.transform.position, playerRef.MYTarget.transform.position) > playerRef.MeleeRange)
        {

            return false;
        }


        if (Physics.Raycast(TruePosition, toTarget, out RaycastHit hit, 40000))//player.transform.TransformDirection(toTarget)
        {
            // Debug.Log($"AA  we hit:--" + hit.collider.gameObject.name);

            if (hit.transform == playerRef.MYTarget.transform)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    */
    public override void UpdateAIState(PlayerStateManager player)
    {
        //Debug.Log("rot");

        base.UpdateAIState(player);


        player.transform.Rotate(0.0f, 1.0f, 0.0f, Space.Self);

        

        if (player.HasTarget() ) 
        { 
        Vector3 TargetTruePosition = player.MYTarget.transform.position;
        Vector3 toTarget = TargetTruePosition - player.transform.position;

        //Debug.Log(Vector3.Angle(player.transform.forward, toTarget));

            if (Vector3.Angle(player.transform.forward, toTarget) <= 15)
            {
                //Debug.Log("right angle");






                if(player.CanHitTarget(player.gameObject, player.MYTarget, player.MeleeRange))
                {
                    //Debug.Log("can hit - going AttackAIState");
                    player.SwitchAIState(player.AttackAIState);

                }
                

            }
        }

        

    }

}
