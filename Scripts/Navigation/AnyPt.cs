using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnyPt : MonoBehaviour
{
    [SerializeField] private GameObject EditorModel;

    public void Awake()
    {
        GetComponent<Renderer>().enabled = false;
        EditorModel.GetComponent<Renderer>().enabled = false;
    }

}
