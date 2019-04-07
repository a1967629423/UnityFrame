using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DSM
{
    [DState(typeof(testRunState),typeof(testMachine),"run")]
    [DLinkTo(typeof(testRunState),typeof(testDefState),"run")]
    public class testRunState : testState
    {
        public override void Start()
        {
            Debug.Log("run state is run");
        }
    }
}

