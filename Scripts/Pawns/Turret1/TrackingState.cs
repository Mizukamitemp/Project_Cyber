
using UnityEngine;

public class TrackingState : Turret1BaseState
{
    float timer = 0;
    float timertargetlost = 0;
    public override void EnterState(Turret1Script player)
    {
        Debug.Log($"TRACKING!");




    }
    public override void ExitState(Turret1Script player)
    {

    }

    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    public override void UpdateState(Turret1Script player)
    {
        timer = timer + Time.deltaTime;







        if (player.GetComponent<Turret1Script>().MYTarget != null)
        {
            Vector3 TargetTruePosition = player.GetComponent<Turret1Script>().MYTarget.transform.position;
            TargetTruePosition.y += 0.7f;
            Vector3 toTarget = TargetTruePosition - player.transform.position;





            if (AngleDir(player.transform.forward, toTarget, player.transform.up) > 0)
            {
                player.transform.Rotate(0.0f, 1.0f, 0.0f, Space.Self);
            }
            else
            {
                player.transform.Rotate(0.0f, -1.0f, 0.0f, Space.Self);
            }




            if (Physics.Raycast(player.transform.position, toTarget, out RaycastHit hit, 40000))//player.transform.TransformDirection(toTarget)
            {
                //Debug.Log($"we hit:--" + hit.collider.gameObject.name);

                if (hit.transform.tag != "SelectableUnit")
                {
                    // Debug.Log($"CAN HIT YA!");

                    timertargetlost = timertargetlost + Time.deltaTime;

                    if (timertargetlost>5)
                    {
                        player.GetComponent<Turret1Script>().SwitchState(player.GetComponent<Turret1Script>().IdState);
                    }
                    
                }
                else 
                {
                    timertargetlost = 0;
                }
            }







            if (timer > 1)
            {
            if (Vector3.Angle(player.transform.forward, toTarget) <= 5)
            {
                if (player.GetComponent<Turret1Script>().MYBarrel != null)
                {
                    GameObject PlasmaShot = GameObject.Instantiate(player.GetComponent<Turret1Script>().PlasmaProj, player.GetComponent<Turret1Script>().MYBarrel.transform.position, player.transform.rotation);
                    Vector3 ShootDirection = player.transform.rotation * Vector3.forward;
                    PlasmaShot.GetComponent<PlasmaProjScript>().Setup(toTarget, player.gameObject);
                }
            }
                timer = 0;
            }


            }








    }
}
