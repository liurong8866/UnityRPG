
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    [RequireComponent(typeof(KBEngine.KBNetworkView))]
    //[RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MyAnimationEvent))]
    [RequireComponent(typeof(BloodBar))]
    [RequireComponent(typeof(NpcAttribute))]
    [RequireComponent(typeof(CommonAI))]
    [RequireComponent(typeof(ShadowComponent))]
    [RequireComponent(typeof(CharacterInfo))]
    [RequireComponent(typeof(SkillInfoComponent))]
    //[RequireComponent(typeof(PhysicComponent))]
    [RequireComponent(typeof(BuffComponent))]
    [RequireComponent(typeof(LogicCommand))]
    [RequireComponent(typeof(MoveController))]
    [RequireComponent(typeof(NpcEquipment))]
    public class AIBase : MonoBehaviour
    {
        public bool ignoreFallCheck = false;
        protected NpcAttribute attribute;
        protected AICharacter ai;

        public AICharacter GetAI()
        {
            return ai;
        }

        public List<MyLib.MyEvent.EventType> regEvt = null;

        public void RegEvent()
        {
            if (regEvt != null)
            {
                foreach (MyLib.MyEvent.EventType t in regEvt)
                {
                    MyLib.MyEventSystem.myEventSystem.RegisterEvent(t, OnEvent);
                }
            }
			
        }

        void DropEvent()
        {
            if (regEvt != null)
            {
                foreach (MyLib.MyEvent.EventType t in regEvt)
                {
                    MyLib.MyEventSystem.myEventSystem.dropListener(t, OnEvent);
                }
            }
        }

        protected virtual void OnEvent(MyLib.MyEvent evt)
        {
        }

        protected virtual void OnDestroy()
        {
            DropEvent();
        }

        public KBEngine.KBNetworkView photonView
        {
            get
            {
                return GetComponent<KBEngine.KBNetworkView>();
            }
        }

        protected void AddEvent(MyLib.MyEvent.EventType t)
        {
            regEvt.Add(t);
            MyLib.MyEventSystem.myEventSystem.RegisterEvent(t, OnEvent);
        }
        #if UNITY_EDITOR
        void OnGUI()
        {
            if (ClientApp.Instance.testAI)
            {
                var pos = transform.position;
                if (Camera.main != null)
                {
                    var sp =Camera.main.WorldToScreenPoint(pos+new Vector3(0, 2.0f, 0));
                    //Debug.Log("sp "+sp);
                    GUI.Label(new Rect(sp.x - 50, Screen.height-sp.y, 100, 100), GetAI().state.type.ToString());
                    //GUI.Label(new Rect(0, 0, 100, 100), GetAI().state.type.ToString());
                }
            }
        }   
        #endif
    }

}