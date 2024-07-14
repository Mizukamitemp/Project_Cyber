
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;

public class TurrIdleState : Turret1BaseState
{

    public override void EnterState(Turret1Script player)
    {

    }
    public override void ExitState(Turret1Script player)
    {

    }
    public override void UpdateState(Turret1Script player)
    {



        player.transform.Rotate(0.0f, 1.0f, 0.0f, Space.Self);//GetComponent<Turret1Script>().


        if (player.GetComponent<Turret1Script>().MYTarget != null)
        {
            Vector3 TargetTruePosition = player.GetComponent<Turret1Script>().MYTarget.transform.position;
            TargetTruePosition.y += 0.7f;
            Vector3 toTarget = TargetTruePosition - player.transform.position;

            if (Vector3.Angle(player.transform.forward, toTarget) <= 15)
            {
                

                if (Physics.Raycast(player.transform.position, toTarget, out RaycastHit hit, 40000))//player.transform.TransformDirection(toTarget)
                {
                    //Debug.Log($"we hit:--" + hit.collider.gameObject.name);

                    if (hit.transform.tag == "SelectableUnit")
                    {
                        // Debug.Log($"CAN HIT YA!");
                        player.GetComponent<Turret1Script>().SwitchState(player.GetComponent<Turret1Script>().TrState);

                    }


                }

            }

        }

    }


}
