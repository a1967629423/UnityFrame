using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DSM
{
    public abstract class State
    {
        // Start is called before the first frame update
        public StateMachine context = null;
        public bool _Awaked = false;
        public bool _IsAttach = false;
        public string Name = "";
        public virtual void Awake()
        {
            _Awaked = true;
        }
        public virtual void Start()
        {

        }
        public virtual void Quit()
        {

        }
        public virtual void done()
        {
            if (_IsAttach) context.attachQuit(this);
        }
        public override string ToString()
        {
            return string.Format("State:{0}",this.Name);
        }

    }
}

