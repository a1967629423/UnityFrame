using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
namespace DSM
{
    public struct StateInfor
    {
        public State Ins;
        public String Name;
    }
    public struct SSMEvent
    {
        public State Source;
        public State Target;
        public string Event;
    }
    public class StateMachine : MonoBehaviour
    {
        // Start is called before the first frame update
        public State nowState = null;
        public List<State> attachStates = new List<State>();
        public List<StateInfor> stateInfors = new List<StateInfor>();
        public List<SSMEvent> EventList = new List<SSMEvent>();
        public DStateManage.SStateMachineInfo MachineInfo = null;
        private static bool _Inited = false;
        public StateMachine()
        {

        }
        public virtual void Awake()
        {
            Init();
        }
        public virtual void Start()
        {

        }
        bool instanceof(Type s, Type t)
        {
            Type baseT = s;
            while (baseT != null && baseT != t)
            {
                baseT = baseT.BaseType;
            }
            return baseT == t;
        }
        void Init()
        {
            if (!_Inited)
            {
                List<Type> stateList = new List<Type>();
                var types = Assembly.GetExecutingAssembly().GetTypes();
                var namesp = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;
                foreach (var item in types)
                {
                    if (item!=typeof(StateMachine)&&instanceof(item, typeof(StateMachine)))
                    {
                        item.GetCustomAttribute(typeof(DStateMachine));
                    }else if (item != typeof(State)&&instanceof(item, typeof(State)))
                    {
                        stateList.Add(item);
                    }
                }
                foreach (var item in stateList)
                {
                    item.GetCustomAttributes(typeof(DState), false);
                    item.GetCustomAttributes(typeof(DDefaultState), false);
                    item.GetCustomAttributes(typeof(DLinkTo), false);
                }
                _Inited = true;
            }
            this.MachineInfo = DStateManage.Infos.Find(new Predicate<DStateManage.SStateMachineInfo>(value =>
            {
                return value.StateMachineType == this.GetType();
            }));
            if (this.MachineInfo != null)
            {
                foreach (var item in this.MachineInfo.StateInfo)
                {
                    object[] pushval = { };
                    var Instance = item.StateType.GetConstructors()[0].Invoke(pushval) as State;
                    Instance.context = this;
                    Instance.Name = item.Name;
                    this.stateInfors.Add(new StateInfor()
                    {
                        Ins = Instance,
                        Name = item.Name
                    });
                }
                foreach (var item in this.MachineInfo.StateLinkInfo)
                {
                    this.EventList.Add(new SSMEvent()
                    {
                        Source = this.stateInfors.Find(new Predicate<StateInfor>(value => { return value.Ins.GetType() == item.SourceType; })).Ins,
                        Target = this.stateInfors.Find(new Predicate<StateInfor>(value => { return value.Ins.GetType() == item.TargetType; })).Ins,
                        Event = item.EventName
                    });

                }
            }
            else
            {
                this.MachineInfo = new DStateManage.SStateMachineInfo(this.GetType());
            }
            //切换到默认状态
            emit("start");
        }
        public void emit(string EventName)
        {
            foreach (var item in this.EventList)
            {
                if (item.Source == this.nowState && item.Event == EventName)
                {
                    changeState(item.Target);
                    return;
                }
            }
        }
        public void changeState(State cs)
        {
            if (this.nowState != null) this.nowState.Quit();
            this.nowState = cs;
            if (!cs._Awaked) cs.Awake();
            cs.Start();
        }
        private IEnumerator attachStateStart<T>(T cs) where T : State
        {
            yield return null;
            cs.Start();
        }
        public T attachState<T>() where T : State
        {
            object[] pushval = { this };
            T cs = null;
            try
            {
                cs = typeof(T).GetConstructors()[0].Invoke(pushval) as T;
                if (cs != null)
                {
                    cs.Awake();
                    this.StartCoroutine(attachStateStart<T>(cs));
                    attachStates.Add(cs);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.ToString());
            }
            return cs;
        }
        public void attachQuit(State cs)
        {
            attachStates.Remove(cs);
        }
        public T[] getAttachs<T>() where T : State
        {
            List<T> arr = new List<T>();
            foreach (var item in attachStates)
            {
                if (item.GetType() == typeof(T))
                {
                    arr.Add(item as T);
                }
            }
            return arr.ToArray();

        }
        public T getAttach<T>() where T : State
        {
            var arr = getAttachs<T>();
            return arr.Length > 0 ? arr[0] : null;
        }
        public void forEachAttach(string functionName, params object[] args)
        {
            foreach (var item in attachStates)
            {
                var info = item.GetType().GetMethod(functionName);
                if (info != null)
                {
                    info.Invoke(item, args);
                }

            }
        }
        public override string ToString()
        {
            return string.Format("nowState:{0}\nEventCount:{1}\nattachStateCount:{2}", nowState.Name, EventList.Count, attachStates.Count);
        }


    }
}


