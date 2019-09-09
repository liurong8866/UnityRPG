using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    ///闪烁移动到攻击目标附近 
    /// </summary>
    public class ShanShuoTarget : MonoBehaviour
    {
        public GameObject endParticle;
        SkillLayoutRunner runner;
        void Awake() {
            Log.Sys("CreateShanShuo");
        }
        // Use this for initialization
        void Start()
        {
            runner = transform.parent.GetComponent<SkillLayoutRunner>();
            StartCoroutine(ShanShuoNow());
        }

        IEnumerator ShanShuoNow() {
            yield return new WaitForSeconds(1);
            var atk = runner.stateMachine.attacker;
            if(atk != null) {
                Log.Sys("ShanShuoNow "+atk.transform.position);
                var target = atk.GetComponent<CommonAI>().targetPlayer;
                Log.Sys("ShanShuoTarget "+target.transform.position);
                atk.transform.position = target.transform.position + new Vector3(Random.Range(-3.0f, 3.0f), 0.2f, Random.Range(-3.0f, 3.0f));
                if(endParticle != null) {
                    var g = GameObject.Instantiate (endParticle) as GameObject;
                    g.transform.parent = runner.transform;
                    g.transform.position = atk.transform.position;
                    g.transform.localRotation = Quaternion.identity;
                    g.transform.localScale = Vector3.one;
                }
                BackgroundSound.Instance.PlayEffect("skill/palaceTeleport");
            }else {
                Log.Sys("ShanShuo NoAttacker ");
            }
        }

    
        // Update is called once per frame
        void Update()
        {
    
        }
    }

}