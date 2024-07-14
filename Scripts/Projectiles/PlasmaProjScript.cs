using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaProjScript : MonoBehaviour
{
    private Vector3 ShootDir;
    //public PlayerStateManager Instigator;
    public GameObject Instigator;
    public GameObject HitEffect;
    public GameObject ShotEffect;

    public void Setup(Vector3 Dir, GameObject Player)
    {
        ShootDir = Dir.normalized;
        Instigator = Player;
        Destroy(gameObject, 3);
        Instantiate(ShotEffect, transform.position, transform.rotation);
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;

    }

    private void FixedUpdate()
    {

        GetComponent<Rigidbody>().position += ShootDir * Time.deltaTime * 70f;

    }

    private void OnTriggerEnter(Collider target)
    {
        if (target.GetComponent<PlayerStateManager>() != null && target.gameObject != Instigator)
        {
            //Debug.Log("expl 1");
            target.GetComponent<PlayerStateManager>().TakeDamage(1);
            Instantiate(HitEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        if (target.gameObject == Instigator)//target.GetComponent<Turret1Script>() != null
        {
            //Debug.Log("hit self");
        }

        if ((target.tag == "Ground" || target.tag == "Untagged") && target.gameObject != Instigator && target.gameObject != Instigator.GetComponent<PlayerStateManager>().ModelInstance)// Последнее - это модель объекта с собственным коллижном, вроде
        {
            //Debug.Log("expl 2");
            //Debug.Log(target.gameObject.name);

            Instantiate(HitEffect, transform.position, transform.rotation);
            Destroy(gameObject);   
        }
    }
}
