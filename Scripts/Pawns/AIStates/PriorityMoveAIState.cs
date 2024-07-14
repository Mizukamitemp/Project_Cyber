using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class PriorityMoveAIState : PlayerBaseAIState
{
    private float timer;
    private Vector3 FormationDestination;
    //bool BlockingJustStarted = true;
    public override void EnterAIState(PlayerStateManager player)
    {
        //Debug.Log("AI: PRIORITY MOVE");
        timer = 0;
        player.MaybeDisableAi();


        if (player.CurrentState !=player.WalkingState) player.SwitchState(player.WalkingState);



        FormationDestination.x = player.GuardLocation.x - player.FormationOffset.x;
        FormationDestination.z = player.GuardLocation.z - player.FormationOffset.y;
        FormationDestination.y = player.GuardLocation.y;
        if (player.myNavMesh.enabled)
        { 
        player.myNavMesh.isStopped = false;
        player.myNavMesh.SetDestination(FormationDestination);
        }
    }

public override void ExitAIState(PlayerStateManager player)
    {

    }

    public override void UpdateAIState(PlayerStateManager player)
    {
        timer = timer + Time.fixedDeltaTime;

        if (timer > 0.3)
        {
            player.SwitchAIState(player.IdleAIState);
        }
    }
}
