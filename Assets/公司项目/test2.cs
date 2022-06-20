using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject GameObject;
    void Start()
    {
        GameObject.transform.SetSiblingIndex(0);
    }


}
