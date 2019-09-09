#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace MyLib 
{
    public class TestShader : MonoBehaviour
    {
        [ButtonCallFunc()]public bool SetOther;

        public void SetOtherMethod()
        {
            var me = ObjectManager.objectManager.GetMyAttr();
            me.SetTeamHideShader();
        }

    }
}

#endif