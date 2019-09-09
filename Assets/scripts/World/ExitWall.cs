
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class ExitWall : MonoBehaviour
    {
        Transform doorOpenBeam;
        public Transform colliderObj;
        List<Transform> beams;
        void Awake() {
            doorOpenBeam = Util.FindChildRecursive(transform, "doorOpenBeam");
            doorOpenBeam.gameObject.SetActive(false);
            colliderObj = transform.FindChild("collider");
            beams = Util.FindAllChild(transform, "enterBeam_entrance");
        }
        // Use this for initialization
        void Start()
        {
        }

        /// <summary>
        /// 区域怪物被杀光，则开启屏蔽门
        /// </summary>
        public void ZoneClear()
        {
            gameObject.SetActive(true);
            colliderObj.gameObject.SetActive(false);
            StartCoroutine(WaitShowDoorOpenEffect());
        }

        /// <summary>
        /// 显示门开启的效果
        /// </summary>
        /// <returns>The show door open effect.</returns>
        IEnumerator WaitShowDoorOpenEffect()
        {
            while (true)
            {
                var players = Physics.OverlapSphere(transform.position, 4, SkillDamageCaculate.GetDamageLayer());
                foreach (Collider p in players)
                {
                    if (p.tag == GameTag.Player)
                    {
                        StartCoroutine(ShowOpenEffect());
                        yield break;
                    }
                }
                yield return null;
            }

        }

        /// <summary>
        /// 显示门开启的效果
        /// </summary>
        /// <returns>The open effect.</returns>
        IEnumerator ShowOpenEffect()
        {
            doorOpenBeam.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            foreach (Transform t in beams)
            {
                t.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 玩家远离上个Room则关闭门
        /// </summary>
        public void CloseDoor()
        {
            colliderObj.gameObject.SetActive(true);
            foreach (Transform t in beams)
            {
                t.gameObject.SetActive(true);
            }

        }

    }


}