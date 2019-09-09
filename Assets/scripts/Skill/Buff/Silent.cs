using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class Silent : IEffect 
    {
        public override bool CanUseSkill()
        {
            return false;
        }

    }

}