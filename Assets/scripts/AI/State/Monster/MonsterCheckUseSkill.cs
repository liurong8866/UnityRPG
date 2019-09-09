using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class MonsterCheckUseSkill : MonoBehaviour
    {
        void Awake() {
        }

        void Start() {
            StartCoroutine(CheckUseSkill());
        }
        /// <summary>
        ///女刺客定期检测是否可以闪烁技能 
        /// </summary>
        /// <returns>The use skill.</returns>
        IEnumerator CheckUseSkill() {
            var aiChar = GetComponent<AIBase>().GetAI();
            var skillInfo = GetComponent<SkillInfoComponent>();
            var aniEvent = GetComponent<MyAnimationEvent>();
            while(true) {
                if(aiChar.state.type == AIStateEnum.COMBAT) {
                    var skillData = skillInfo.GetRandomSkill();
                    Log.AI("UseSkillData ");
                    if(skillData != null) {
                        aniEvent.InsertMsg(new MyAnimationEvent.Message(MyAnimationEvent.MsgType.IDLE));
                        aniEvent.OnSkill(skillData);
                        yield return new WaitForSeconds(2);
                    }
                }
                yield return new WaitForSeconds(0.3f);
            }
        }

    }
}
