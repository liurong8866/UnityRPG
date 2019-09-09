using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SkillBut  :  IUserInterface
    {
        UILabel coldTime;
        public int index;
        void Awake()
        {
            coldTime = GetLabel("ColdTime");
        }

        void Update()
        {
            var cd = SkillDataController.skillDataController.GetCoolTime(index);
            if(cd > 0) {
                coldTime.gameObject.SetActive(true);
                coldTime.text = ((int)cd).ToString();
            }else {
                coldTime.gameObject.SetActive(false);
            }
        }
    }
}