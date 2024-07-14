using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RTSCam : MonoBehaviour
{
    public PlayerController playerController;
    public Transform viewTarget;
    public Vector3 offset;
    public Vector3 campos;

    // Start is called before the first frame update
    void Start()
    {
        viewTarget = playerController.PlayerPawn.GetComponent<Transform>();



        //offset.x = 0f;
        //offset.y = 20f;
        //offset.z = -18f;
        //transform.rotation = Quaternion.Euler(40f, 0f, 0f);

        offset.x = 0f;
        offset.y = 20f;
        offset.z = -18f;
        transform.rotation = Quaternion.Euler(40f, 0f, 0f);


    }

    // Update is called once per frame
    void Update()
    {

        if (!playerController.PlayerPawn.GetComponent<PlayerStateManager>().isDead)
        {


            transform.position = new Vector3(viewTarget.position.x + offset.x, viewTarget.position.y + offset.y, viewTarget.position.z + offset.z);
            //(viewTarget.transform.position) + (transform.forward * offset.z) + (transform.up * offset.y) + (transform.right * offset.x);
        }
    }
}
