using UnityEngine;
using System.Collections;
namespace MyLib {
    /// <summary>
    /// 用于技能层粒子效果相关配置 
    /// </summary>
	public class SkillLayoutConfig : MonoBehaviour {
        /// <summary>
        /// 粒子位置
        /// </summary>
		public Vector3 Position;
        /// <summary>
        /// 粒子对象 
        /// </summary>
		public GameObject particle;
        /// <summary>
        /// 粒子效果延迟时间 
        /// </summary>
        public float delayTime;
        /// <summary>
        ///将粒子设定到某个骨骼的位置 
        /// </summary>
		public string boneName="";

	}
}
