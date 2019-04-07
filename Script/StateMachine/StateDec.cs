using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
namespace DSM
{
    public static class DStateManage
    {
        public class SStateMachineInfo
        {
            public Type StateMachineType;
            public List<SStateInfo> StateInfo;
            public List<SStateLinkInfo> StateLinkInfo;
            public List<string> Events;
            public SStateMachineInfo(Type StateMachineType)
            {
                this.StateMachineType = StateMachineType;
                StateInfo = new List<SStateInfo>();
                StateLinkInfo = new List<SStateLinkInfo>();
                Events = new List<string>();
            }
        }
        public struct SStateInfo
        {
            public Type StateType;
            public string Name;
        }
        public struct SStateLinkInfo
        {
            public Type SourceType;
            public Type TargetType;
            public string EventName;
        }
        public class SStateAttrInfo
        {
            public Type self;
            public Type MachineType;
            public SStateAttrInfo(Type self,Type MachineType)
            {
                this.self = self;
                this.MachineType = MachineType;
            }
        }
        public static List<SStateMachineInfo> Infos = new List<SStateMachineInfo>();
        public static List<SStateAttrInfo> StateMap = new List<SStateAttrInfo>();
    }
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class DStateMachine : System.Attribute
    {
        public DStateMachine(Type self)
        {
            Debug.Log("DStateMachine");
            DStateManage.Infos.Add(new DStateManage.SStateMachineInfo(self));
        }

    }
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class DState : System.Attribute
    {
        public Type MachineType;
        public DState(Type self, Type MachineType, string name)
        {
            var info = DStateManage.Infos.Find(new Predicate<DStateManage.SStateMachineInfo>((DStateManage.SStateMachineInfo value) =>
            {
                return value.StateMachineType == MachineType;
            }));
            if (info != null)
            {
                Debug.Log("DState");
                info.StateInfo.Add(new DStateManage.SStateInfo()
                {
                    StateType = self,
                    Name = name
                });
            }
            var Map = DStateManage.StateMap.Find(new Predicate<DStateManage.SStateAttrInfo>(value=>{return value.self == self;}));
            if(Map==null)
            {
                DStateManage.StateMap.Add(new DStateManage.SStateAttrInfo(self,MachineType));
            }
            this.MachineType = MachineType;

        }

    }
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class DLinkTo : System.Attribute
    {
        public DLinkTo(Type self, Type target, string eventName)
        {

            var attr = DStateManage.StateMap.Find(new Predicate<DStateManage.SStateAttrInfo>(value=>{return value.self==self;}));
            if (attr != null)
            {
                Debug.Log("DLinkTo");
                var MachineType = attr.MachineType;
                var info = DStateManage.Infos.Find(new Predicate<DStateManage.SStateMachineInfo>((DStateManage.SStateMachineInfo value) =>
                {
                    return value.StateMachineType == MachineType;
                }));
                if (info != null)
                {
                    
                    info.StateLinkInfo.Add(new DStateManage.SStateLinkInfo()
                    {
                        SourceType = self,
                        TargetType = target,
                        EventName = eventName
                    });
                    if (!info.Events.Exists(new Predicate<string>(value => { return value == eventName; })))
                    {
                        info.Events.Add(eventName);
                    }
                }
            }
        }
        
        // public DLinkTo(Type self, string target, string eventName)
        // {

        // }

    }
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
        public sealed class DDefaultState : System.Attribute
        {
            public DDefaultState(Type self)
            {
                var attr = DStateManage.StateMap.Find(new Predicate<DStateManage.SStateAttrInfo>(value=>{return value.self==self;}));
                if(attr!=null)
                {
                    var MachineType = attr.MachineType;
                    var info = DStateManage.Infos.Find(new Predicate<DStateManage.SStateMachineInfo>((DStateManage.SStateMachineInfo value) =>
                    {
                        return value.StateMachineType == MachineType;
                    }));
                    if(info!=null)
                    {
                        if(info.Events.Exists(new Predicate<string>(value=>{return value=="start";})))
                        {
                            Debug.LogWarning("'start' State has been define");
                        }
                        else
                        {
                            info.Events.Add("start");
                        }
                        info.StateLinkInfo.Add(new DStateManage.SStateLinkInfo(){
                            SourceType = null,
                            TargetType = self,
                            EventName="start"
                        });
                    }
                }
            }
        }
}
