using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class ShootingAIState : PlayerBaseAIState
{
    private float timer1;
    //private float timer2;
   // private float timer3;
    private PlayerStateManager playerRef;
    //bool BlockingJustStarted = true;
    public override void EnterAIState(PlayerStateManager player)
    {

        //Debug.Log("----ShootingAIState--------");
        timer1 = 0;
        //timer2 = 0;
        // timer3 = 0;

        player.MaybeDisableAi();    //внезапно здох
        if (!player.isBeingPlayed && player.myNavMesh.enabled) player.myNavMesh.isStopped = true;
        player.SwitchState(player.AimingState);
    }



    public override void ExitAIState(PlayerStateManager player)
    {

    }

    public override void UpdateAIState(PlayerStateManager player)
    {
        //Debug.Log("SHOO");
        player.MaybeDisableAi();    //внезапно здох
        timer1 = timer1 + Time.fixedDeltaTime;
        
       // timer3 = timer3 + Time.fixedDeltaTime;
        Vector3 TargetTruePosition = player.MYTarget.transform.position;
        TargetTruePosition.y += 0.7f;
        Vector3 TruePosition = player.transform.position;
        TruePosition.y += 0.7f;
        Vector3 toTarget = player.MYTarget.GetComponent<PlayerStateManager>().FigureCenter.transform.position - player.Gun1.transform.position;


        if (player.AngleDir(player.transform.forward, toTarget, player.transform.up) > 0)
        {
            player.transform.Rotate(0.0f, player.RotateSpeed, 0.0f, Space.Self);
        }
        else
        {
            player.transform.Rotate(0.0f, -player.RotateSpeed, 0.0f, Space.Self);
        }





        if (timer1 > player.AttackRate && (player.VectorAngleHorizontal(player.transform.forward, toTarget) <= 5) && Vector3.Distance(player.transform.position, player.MYTarget.transform.position) <= player.MeleeRange)
        {

            if (player.GetComponent<PlayerStateManager>().Gun1 != null)
            {
                GameObject PlasmaShot = GameObject.Instantiate(player.GetComponent<PlayerStateManager>().PlasmaProj, player.GetComponent<PlayerStateManager>().Gun1.transform.position, player.transform.rotation);
                //Vector3 ShootDirection = player.transform.rotation * Vector3.forward;
                PlasmaShot.GetComponent<PlasmaProjScript>().Setup(toTarget, player.gameObject);
                player.GetComponent<PlayerStateManager>().PlaySound_Fire();
            }

            timer1 = timer1 + Time.fixedDeltaTime;
            player.SwitchAIState(player.IdleAIState); //раньше он уходил на AttackAIState, но убрали чтобы мог проверить, не отменил ли игрок команду через движение + игнор опасности
        }
        else if (timer1 > player.AttackRate)
        {
            player.SwitchAIState(player.IdleAIState);
        }

       
    }



}
