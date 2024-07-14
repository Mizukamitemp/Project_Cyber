using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class IdleAIState : PlayerBaseAIState
{
    private float timer = 0;
   

    public override void EnterAIState(PlayerStateManager player)
    {
        //Debug.Log("IDLE START");
        timer = 0;

        // Если это не игрок
        if (!player.isBeingPlayed)
        {
            player.MaybeDisableAi();    //внезапно здох

            // Это сокр. MakeUnplayable Из PlayerStateManager
            player.myNavMesh.enabled = true;
            player.Controller.enabled = false;
            player.isBeingPlayed = false;
            player.InputVector = Vector2.zero;
            player.GetComponent<CapsuleCollider>().enabled = true;


            // Просто назначили MYTarget и hasTarget
            // hasTarget сбрасывается если цели нет долго - можно принимать решение
            CheckForTargets(player);

            // Проверяем, можем ли прятаться

            if (player.lostTarget // однажды видел цель и потерял контакт
                && player.IsHiding // в принципе должен прятаться
                && !player.IsInPlayersArmy // не находится в отряде игрока
                                           //&& !player.hasMoveOrders // не имеет приказа идти куда-то прямо сейчас
                && !player.HasHidingPoint
                && !player.IsHidden // уже достиг точки где прятаться
                && !player.IsStationary)
            {
                //Debug.Log($"Let's go hiding!" + player.lostTarget);
                if (player.MYTarget != null)
                {
                    GetHidingPoint(player, player.MYTarget.transform.position);  // от какой позиции прячемся
                }
            }
            else if (player.NextPatrol != null && !player.IsInPlayersArmy && !player.hasMoveOrders && !player.IsHidden && !player.HasHidingPoint && !player.IsStationary)
            {
                //Debug.Log("Let's get patrol point!");
                GetPatrolPoint(player);  // идем патрулировать если надо патрулировать
            }
            else
            {
                //Debug.Log("We're not hiding and we're not getting a new patrol point...");

            }



            // Условия атаки, если появилась цель в CheckForTargets
            // Иначе игнорируем возможность атаки 
            if (
                // Нет приказа идти, 
                (!player.hasMoveOrders // нет приказа идти
                && player.HasTarget() // есть актуальная цель, не потерянная
                && (!player.IsGuardingThatPoint || !player.checkExceededPursuitDistance())) //не сторожит или сторожит и не ушел за дистанцию преследования
                ||
                // Есть приказ идти и не стоит режим не атаковать при движении
                (player.hasMoveOrders && player.HasTarget() && !player.GetIgnoreWhenMoving())
                ||
                // Уже нет приказа идти (дошел) и стоит режим не атаковать при движении
                (!player.hasMoveOrders && player.HasTarget() && player.GetIgnoreWhenMoving())
               )
            {
                //Debug.Log("We're attacking...");
                player.SwitchAIState(player.AttackAIState);
            }
            else if (!player.IsStationary && !player.IsInPlayersArmy && (player.IsGuardingThatPoint && (player.checkAwayFromGuardPoint() == true || player.NextPatrol!=null) && !player.HasTarget() && !player.HasHidingPoint)
                    ||
                    (!player.IsStationary && !player.IsInPlayersArmy && player.IsGuardingThatPoint && player.checkExceededPursuitDistance() == true && player.HasTarget())
                    ||
                    (!player.IsStationary && player.hasMoveOrders && (player.checkAwayFromGuardPoint() == true || player.NextPatrol != null))
                    ) // он сторож и возвращается на стартовую точку
            {
                // ОТЛАДКА:
                //Debug.Log("We're NOT attacking...");
                //if (!player.isStationary && !player.IsInPlayersArmy && (player.IsGuardingThatPoint && (player.checkAwayFromGuardPoint() == true || player.NextPatrol != null) && !player.HasTarget() && !player.HasHidingPoint)) Debug.Log("------AI: RETURNING AFTER KILL or lost target");
                //if (!player.isStationary && !player.IsInPlayersArmy && player.IsGuardingThatPoint && player.checkExceededPursuitDistance() == true && player.HasTarget()) Debug.Log("------EXCEEDED PURS");
                //if (!player.isStationary && player.hasMoveOrders && (player.checkAwayFromGuardPoint() == true || player.NextPatrol != null)) Debug.Log("------MOVING ORDERS");


                player.SwitchAIState(player.PriorityMoveAIState);
                
            }
            else 
            {
                //Debug.Log($"-------NO ORDERS" + player.checkAwayFromGuardPoint());
                player.moveDestination = player.transform.position;
                player.myNavMesh.isStopped = true;
                player.IsHidden = true; // добежал и считает, что спрятался
                player.SwitchState(player.IdlingState); // это состояние Pawn а не AI - просто чтобы он не бежал и пр.
            }

        }
    }

    public bool GetHidingPoint(PlayerStateManager playerRef, Vector3 enemyPos)
    {
        //Debug.Log("GetHidingPoint");
        GameObject chosenPoint = null;
        GameObject[] hidingPlaces;
        float maxDistance = 50;
        
        List<GameObject> bestPlaces = new List<GameObject>();

        hidingPlaces = GameObject.FindGameObjectsWithTag("NavAmbush");

        //Debug.Log($"Overall: " + hidingPlaces.Length);

        foreach (GameObject go in hidingPlaces)
        {
            chosenPoint = go;
            if (Vector3.Distance(go.transform.position, enemyPos) < maxDistance)
            {
                bestPlaces.Add(go);
            }
        }

        if (bestPlaces.Count != 0)
        {
            int placeNumber = Mathf.RoundToInt(Random.Range(0, bestPlaces.Count));

        //Debug.Log($"Random number: " + placeNumber);
        //Debug.Log($"Coll size: " + bestPlaces.Count);


            chosenPoint = bestPlaces[placeNumber];
        }


        if (chosenPoint != null)
        {
            //if (playerRef.MYTarget != null)
            //{
                Vector3 direction = chosenPoint.transform.position - playerRef.MYTarget.transform.position;

            direction = Vector3.Normalize(direction);

            //Debug.Log($"Direction x: " + direction.x);
            //Debug.Log($"Direction z: " + direction.z);


            direction = direction * 10;// chosenPoint.GetComponent<AmbushPtSphere>().PositionRadius;
                direction.y = 0;


            //Debug.Log($"enemyPos x: " + enemyPos.x);
            //Debug.Log($"enemyPos z: " + enemyPos.z);


            playerRef.GuardLocation = chosenPoint.transform.position + direction;



            //}
            //else 
            //{
            //    playerRef.guardLocation = chosenPoint.transform.position;
            //}

            //Debug.Log("HidingPoint found");
            playerRef.IsGuardingThatPoint = true;
            
            //playerRef.pursueDistance = 0;
            playerRef.hasMoveOrders = true;
            //playerRef.IgnoreWhenMoving = true;
            playerRef.HasHidingPoint = true;

        }
        return false;
    }

    public bool GetPatrolPoint(PlayerStateManager playerRef)
    {
        if (Vector3.Distance(playerRef.transform.position, playerRef.NextPatrol.transform.position) < 2)

        {
            playerRef.NextPatrol = playerRef.NextPatrol.NextPatrolPoint;
        }

        playerRef.IsGuardingThatPoint = true;
        playerRef.GuardLocation = playerRef.NextPatrol.transform.position;
        playerRef.hasMoveOrders = true;



        return true;
    }
    public override void ExitAIState(PlayerStateManager player)
    {

    }

    public override void UpdateAIState(PlayerStateManager player)
    {
        timer = timer + Time.fixedDeltaTime;

        if (timer > 1)
        {
            //Debug.Log("restart idle from IdleAiState");
           
            player.SwitchAIState(player.IdleAIState);
        }

    }

    public virtual void CheckForTargets(PlayerStateManager player)
    {

        //Debug.Log("-----------------check target params");
        //player.timeWithoutTarget += 1;

        GameObject[] AllUnits;
        float minDistance = player.AgrDistance;    //agrDistance это дальность агра
        Dictionary<int, GameObject> targetList = new Dictionary<int, GameObject>();

        targetList.Clear();
        AllUnits = GameObject.FindGameObjectsWithTag("SelectableUnit");

            foreach (GameObject aUnit in AllUnits)
            {

            if (aUnit.GetComponent<PlayerStateManager>() && aUnit.GetComponent<PlayerStateManager>().Side != player.Side && aUnit.GetComponent<PlayerStateManager>().isDead == false)
                {
                    if (player.CanHitTarget(player.gameObject, aUnit, player.AgrDistance))  // вижу и агрюсь а не вижу и могу попасть - агр против дальности
                    {
                        //Debug.Log("have a  target ");
                        minDistance = Vector3.Distance(player.transform.position, aUnit.transform.position);
                        player.MYTarget = aUnit;
                        player.hasTarget = true;    // Увидел цель - значит есть цель!
                        player.timeWithoutTarget = 0;
                        player.IsHidden = false;
                        player.HasHidingPoint = false;
                    }
                }
            }

        // Мы сбрасываем цель только если она уже некоторое время не видна
        if (player.hasTarget && (player.timeWithoutTarget > player.timeToForgetTarget))
        {
            player.hasTarget = false;
            player.lostTarget = true;
        }


    }
}
