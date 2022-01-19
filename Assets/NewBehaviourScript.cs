using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : SingleMonoBase<test>
{
    public override void Init()
    {
        base.Init();
        
    }

    void Start()
    {
        var t= NewBehaviourScript.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
