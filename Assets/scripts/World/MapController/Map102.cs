using UnityEngine;
using System.Collections;

namespace MyLib
{
    public partial class Map102 : CScene 
    {
        EvtHandler  eh;
        protected override void Awake() {
            base.Awake();
            eh = gameObject.AddComponent<EvtHandler>();
            eh.AddEvent(MyEvent.EventType.EnterNextZone, null);
            eh.AddEvent(MyEvent.EventType.BossSpawn, null);
            eh.AddEvent(MyEvent.EventType.BossDead, null);
            eh.AddEvent(MyEvent.EventType.LevelFinish, null);

        }

        void Start() {
            var step = GameInterface_Player.GetIntState(GameBool.cunZhangState);
            if(step == 4) {
                StartCoroutine(CreateZhiRuo());
            }
        }

        IEnumerator CreateZhiRuo() {
            GameObject  myplayer = null;
            while(myplayer == null) {
                yield return new WaitForSeconds(1);
                myplayer = ObjectManager.objectManager.GetMyPlayer();
            }

            var myp = new GameObject("TempNpcPos");
            yield return new WaitForSeconds(1);
            myp.transform.position = myplayer.transform.position+ new Vector3(Random.Range(-3.0f, 3.0f), 0.2f, Random.Range(-3.0f, 3.0f)); 
            ObjectManager.objectManager.CreateNpc(Util.GetNpcData(20008), myp);

            string[] text = new string[]{
                "至若：哥哥,这里真的有苍冥水么",
                "我也不知道，咱们先看看吧",
            };
            NpcDialogInterface.ShowTextList(text, null);

            yield return StartCoroutine(eh.WaitEvt(MyEvent.EventType.EnterNextZone));
            text = new string[]{
                "至若：为何寻了半天也没见苍冥水的踪迹？",
                "难道因为魔神苏醒，连苍冥水也被毁掉了？",
                "至若：若是如此那可怎么办？",
                "现在也只能走一步看一步了",
            };
            NpcDialogInterface.ShowTextList(text, null);

            yield return StartCoroutine(eh.WaitEvt(MyEvent.EventType.BossSpawn));

            var sover = false;
            text = new string[]{
                "wu.......",
                "至若：这是什么声音？",
                "至若小心。",
            };
            NpcDialogInterface.ShowTextList(text, ()=>sover=true);
            while(!sover) {
                yield return null;
            }

            MyEventSystem.PushEventStatic(MyEvent.EventType.SpeakOver);
           
            yield return StartCoroutine(eh.WaitEvt(MyEvent.EventType.BossDead));
            text = new string[]{
                "至若：那闪闪发光的是什么东西？",
                "好像是一个瓶子，有一股清幽之气",
                "至若：先收起来吧，回去问问巨牙子爷爷",
            };

            bool over = false;
            NpcDialogInterface.ShowTextList(text, ()=>{
                over = true;
            });
            while(!over) {
                yield return null;
            }
            GameInterface_Player.SetIntState(GameBool.cunZhangState, 5);
            yield return StartCoroutine(eh.WaitEvt(MyEvent.EventType.LevelFinish));

            text = new string[]{
                "幽冥之中传来一阵笑声，嘿嘿嘿",
            };
            over = false;
            NpcDialogInterface.ShowTextList(text, ()=>{
                over = true;
            });
            while(!over) {
                yield return null;
            }


            BattleManager.battleManager.GameOver();
        }
    }

}