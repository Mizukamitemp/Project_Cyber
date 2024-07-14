using UnityEngine;
using UnityEngine.InputSystem;

public class AttackState : PlayerBaseState
{
    float timer = 0;
    bool damageDealt = false;
    public GameObject BlockEffect;
    public GameObject BloodEffect;
    private bool AttackBlocked;
    
    public override void EnterState(PlayerStateManager player)
    {
        timer = 0f;
        damageDealt = false;
        //Debug.Log("--Enter Attack");
        AttackBlocked = false;
        playerRef = player;



        if (player.DecreaseFatigue(5000))
        {


            if (player.Anim != null)
            {
                player.Anim.SetTrigger("Attack");
            }

            // PlayerRef берется из playerbasestate и является ссылкой на PlayerStateManager
            Vector3 ShootDirection = playerRef.GetComponent<PlayerStateManager>().transform.rotation * Vector3.forward;
            GameObject PlasmaShot = GameObject.Instantiate(playerRef.GetComponent<PlayerStateManager>().PlasmaProj, playerRef.GetComponent<PlayerStateManager>().Gun1.transform.position, playerRef.GetComponent<PlayerStateManager>().transform.rotation);
            PlasmaShot.GetComponent<PlasmaProjScript>().Setup(ShootDirection, playerRef.gameObject);
            player.PlaySound_Fire();
        }
        playerRef.SwitchState(playerRef.AimingState);


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
        
        timer = timer + Time.deltaTime;

        if (timer > 0.2 && damageDealt == false)
        {
            if (player.isBeingPlayed)
            {
                damageDealt = true;
            }
            else if (player.hasTarget && player.MYTarget != null)
            {
                if (player.MYTarget.GetComponent<PlayerStateManager>() && !player.MYTarget.GetComponent<PlayerStateManager>().IsBlocking())
                {
                    GameObject BloodEffect = GameObject.Instantiate(player.MYTarget.GetComponent<PlayerStateManager>().BloodEffect, player.MYTarget.GetComponent<PlayerStateManager>().FigureCenter.transform.position + player.MYTarget.GetComponent<PlayerStateManager>().transform.forward * 0.1f, player.MYTarget.GetComponent<PlayerStateManager>().transform.rotation);
                    player.MYTarget.GetComponent<PlayerStateManager>().TakeDamage(player.MeleeDamage);
                    damageDealt = true;
                    Debug.Log("Hit auto 1!");
                }
                else if (player.MYTarget.GetComponent<PlayerStateManager>() && player.MYTarget.GetComponent<PlayerStateManager>().IsBlocking())
                {
                    // Debug.Log("Attack blocked!");
                    //GameObject BlockEffect = GameObject.Instantiate(player.MYTarget.GetComponent<PlayerStateManager>().BlockEffect, player.MYTarget.GetComponent<PlayerStateManager>().transform.position + new Vector3(0, player.MeleeOffsetY) + player.MYTarget.GetComponent<PlayerStateManager>().transform.forward * 0.5f, player.MYTarget.GetComponent<PlayerStateManager>().transform.rotation);
                    GameObject BlockEffect = GameObject.Instantiate(player.MYTarget.GetComponent<PlayerStateManager>().BlockEffect, player.MYTarget.GetComponent<PlayerStateManager>().FigureCenter.transform.position + player.MYTarget.GetComponent<PlayerStateManager>().transform.forward * 0.5f, player.MYTarget.GetComponent<PlayerStateManager>().transform.rotation);

                    // player.GetComponent<PlayerStateManager>().TakeDamage(player.MeleeDamage);
                    AttackBlocked = true;
                    damageDealt = true;
                    
                }
            }
        }
    }
}
