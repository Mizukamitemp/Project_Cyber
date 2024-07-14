using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using static UnityEngine.Rendering.DebugUI;

public class BlockState : PlayerBaseState
{
    //float timer = 0;
    //public GameObject BlockEffect;
    //bool damageDealt = false;
    public override void EnterState(PlayerStateManager player)
    {
        playerRef = player;
        //timer = 0f;
        //player.BlockEffect.SetActive(true);

        player.ForceFieldInstance.SetActive(true);

        // GameObject BlockEffect = GameObject.Instantiate(player.BlockEffect , player.transform.position + new Vector3(0, player.MeleeOffsetY)+player.transform.forward*0.5f, player.transform.rotation);

        if (player.Anim != null)
        {
            player.Anim.SetBool("isBlocking", true);

        }
    }

    public override void execOnMouseClick(InputValue value)
    {
    }
    public override void execOnAttack(InputValue value)
    {
    }
    public override void execOnBlock(InputValue value)
    {
        playerRef.SwitchState(playerRef.IdlingState);
    }

    public override void execOnMovement(InputValue value) //двигаем игрока, если юнит управляется игроком
    {
    }
    public override void execOnRPGStop(InputValue value) //Зажат шифт как в Диабло
    {
    }
    public override void execOnUse(InputValue value)
    {
    }
    public override void ExitState(PlayerStateManager player)
    {
        // player.SelectObject.GetComponent<Renderer>().enabled = false;
        player.ForceFieldInstance.SetActive(false);
        player.Anim.SetBool("isBlocking", false);
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (!player.DecreaseFatigue(200))
        {
            player.SwitchState(player.IdlingState);
        }
        
        
        
        
        // утомление
        /*       timer = timer + Time.deltaTime;
               if (timer > 0.5)
               {
                   player.SwitchState(player.IdlingState);
               } */
    }

}
