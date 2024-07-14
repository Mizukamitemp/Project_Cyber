using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static UnityEngine.UI.CanvasScaler;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class AttackAIState : PlayerBaseAIState
{
    private float timer;

    private PlayerStateManager playerRef;
    //bool BlockingJustStarted = true;
    public override void EnterAIState(PlayerStateManager player)
    {
        timer = 0;
       // Debug.Log("1. AttackAIState");



        //Debug.Log("2. UpdateAIState-----------------------------------------");
        player.MaybeDisableAi();    //внезапно здох
        

        Vector3 TargetTruePosition = player.MYTarget.transform.position;
        TargetTruePosition.y += 0.7f;
        Vector3 TruePosition = player.transform.position;
        TruePosition.y += 0.7f;
        Vector3 toTarget = TargetTruePosition - TruePosition;



        //if (player.MYTarget.GetComponent<PlayerStateManager>().isDead == true)
        //{
        //    player.SwitchAIState(player.IdleAIState);
        //}

        if (!player.IsStationary
&& (CanHitTarget(player) == false || (Vector3.Distance(player.transform.position, player.MYTarget.transform.position) > player.MeleeRange))
&& player.isDead == false && !player.GetStandGround()
&& (player.MYTarget.GetComponent<PlayerStateManager>().isDead == false))
        {
            //Debug.Log("attacking - little closer");

            //player.MoveTo(player.MYTarget.transform.position);
            if (player.myNavMesh.isStopped) player.myNavMesh.isStopped = false;
            player.myNavMesh.SetDestination(player.MYTarget.transform.position);
            if (player.CurrentState != player.WalkingState)

            {
                player.SwitchState(player.WalkingState);
                //Debug.Log("walk");
            } 
        }

    }



    public override void ExitAIState(PlayerStateManager player)
    {
       // Debug.Log("6. exit attack ai");
    }

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

    public override void UpdateAIState(PlayerStateManager player)
    {
        //Debug.Log("attacking - loop");
        timer = timer + Time.fixedDeltaTime;
        //if (player.isInPlayersArmy && player.hasMoveOrders && player.GetIgnoreWhenMoving())
        //{
        //    Debug.Log("MAd1!!!!!");
        //    player.SwitchAIState(player.IdleAIState);
        //
        //}



        // Это мы можем стрелять
        if ((CanHitTarget(player) == true 
            && (Vector3.Distance(player.transform.position, player.MYTarget.transform.position) < player.MeleeRange)) || player.IsStationary && (player.MYTarget.GetComponent<PlayerStateManager>().isDead == false))
            
            {
                //Debug.Log("3. yes shoot");
                player.SwitchAIState(player.ShootAIState);
            }


        //Debug.Log("3. no shoot");
        //player.myNavMesh.SetDestination(player.MYTarget.transform.position);

        if (timer > 4)
        {
            player.SwitchAIState(player.IdleAIState);
        }


    }



}
