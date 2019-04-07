using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DSM
{
    [DStateMachine(typeof(testMachine))]
    public class testMachine : StateMachine
{
    // Start is called before the first frame update
    public override void Start()
    {
        Debug.Log(this.MachineInfo.StateInfo.Count);
        Debug.Log(this.EventList.Count);
        Debug.Log(this.nowState==null?"null":this.nowState.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
}

