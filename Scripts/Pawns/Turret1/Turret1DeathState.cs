
using UnityEngine;

public class Turret1DeathState : Turret1BaseState
{
    public override void EnterState(Turret1Script player)
    {
        player.GetComponent<Turret1Script>().DestroyedEffect.SetActive(true);
        player.GetComponent<Turret1Script>().MYBarrel.SetActive(false);
    }
    public override void ExitState(Turret1Script player)
    {

    }



    public override void UpdateState(Turret1Script player)
    {
        player.transform.Rotate(0.0f, 25.0f, 0.0f, Space.Self);//GetComponent<Turret1Script>().

    }
}
