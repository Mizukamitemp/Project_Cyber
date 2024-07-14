using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockEffectSelfSestruct : MonoBehaviour
{
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > 2) 
        {
            Destroy(gameObject);
        }
    }
}
