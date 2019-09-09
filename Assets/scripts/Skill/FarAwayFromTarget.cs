using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class FarAwayFromTarget : MonoBehaviour
    {

        public GameObject endParticle;
        SkillLayoutRunner runner;
        void Awake() {
            Log.Sys("CreateFarAway");
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
                var rd1 = Random.Range(8, 10);
                var rd2 = Random.Range(8, 10);
                var dir = Random.Range(0,2);
                if(dir == 0) {
                    dir = 1;
                }else {
                    dir = -1;
                }

                var dir2 = Random.Range(0,2);
                if(dir2 == 0) {
                    dir2 = 1;
                }else {
                    dir2 = -1;
                }

                atk.transform.position = target.transform.position + new Vector3(rd1*dir, 0.2f, rd2*dir2);
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
    
    }
}