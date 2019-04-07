using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DSM
{
    [DState(typeof(testDefState),typeof(testMachine),"Default")]
    
    [DLinkTo(typeof(testDefState),typeof(testRunState),"run")]
    [DDefaultState(typeof(testDefState))]
    public class testDefState : testState
    {
        public override void Start()
        {
            Debug.Log("Default is run");
        }
        public override void test()
        {
            
        }
    }
}

