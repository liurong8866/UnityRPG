using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class Map101 : CScene
    {
        protected override void Awake()
        {
            base.Awake();
        }
        bool inNextZone = false;
        bool bossSpawn = false;
        bool bossDead = false;
        protected override void OnEvent(MyEvent evt)
        {
            if(evt.type == MyEvent.EventType.EnterNextZone) {
                inNextZone = true;
            }else if(evt.type == MyEvent.EventType.BossSpawn) {
                bossSpawn = true;
            }else if(evt.type == MyEvent.EventType.BossDead) {
                bossDead = true;
            }else if(evt.type == MyEvent.EventType.LevelFinish) {
                
            }
        }


        void  Start()
        {
            var step = GameInterface_Player.GetIntState(GameBool.cunZhangState);
            Log.Sys("Current Step is "+step);
            if(step == 1) {
                StartCoroutine(CreateZhiRuoAndDongHu());
            }else {
                StartCoroutine(NormalBoss());
            }
        }
        IEnumerator NormalBoss() {
            while(!bossSpawn) {
                yield return new WaitForSeconds(1);
            } 
            MyEventSystem.PushEventStatic(MyEvent.EventType.SpeakOver);

            while(!bossDead) {
                yield return new WaitForSeconds(2);
            }
            BattleManager.battleManager.GameOver();
        }


        IEnumerator CreateZhiRuoAndDongHu() {
            this.regEvt = new System.Collections.Generic.List<MyEvent.EventType>(){
                MyEvent.EventType.EnterNextZone, 
                MyEvent.EventType.BossSpawn,
                MyEvent.EventType.BossDead,
                MyEvent.EventType.LevelFinish,
            };
            RegEvent();


            GameObject  myplayer = null;
            while(myplayer == null) {
                yield return new WaitForSeconds(1);
                myplayer = ObjectManager.objectManager.GetMyPlayer();
            }
            var myp = new GameObject("TempNpcPos");
            yield return new WaitForSeconds(1);
            myp.transform.position = myplayer.transform.position+ new Vector3(Random.Range(-3.0f, 3.0f), 0.2f, Random.Range(-3.0f, 3.0f)); 
            ObjectManager.objectManager.CreateNpc(Util.GetNpcData(20008), myp);
            yield return new WaitForSeconds(1);
            myp = new GameObject("TempNpcPos");
            myp.transform.position = myplayer.transform.position+ new Vector3(Random.Range(-3.0f, 3.0f), 0.2f, Random.Range(-3.0f, 3.0f)); 
            ObjectManager.objectManager.CreateNpc(Util.GetNpcData(20009), myp);

            string[] text = new string[]{
                "东湖：这里真的好黑呀...",
                "至若：{0}哥哥,我好怕",
                "别怕至若，我会保护你的，跟紧我。",
                "大家和我一起向前走吧",
            };
            var c = 0;
            foreach(var t in text) {
                text[c] = string.Format(t, ObjectManager.objectManager.GetMyName());
                c++;
            }
            NpcDialogInterface.ShowTextList(text, null);

            while(!inNextZone) {
                yield return new WaitForSeconds(1);
            }
            inNextZone = false;

            text = new string[]{
                "这里的怪物很多，大家小心\n我感受到前面有强大的气息",
                "至若：{0}哥哥也要小心。"
            };
            c = 0;
            foreach(var t in text) {
                text[c] = string.Format(t, ObjectManager.objectManager.GetMyName());
                c++;
            }
            NpcDialogInterface.ShowTextList(text, null);


            while(!inNextZone) {
                yield return new WaitForSeconds(1);
            }
            inNextZone = false;

            text = new string[]{
                "巨牙子爷爷说过这里都只是些魔王子子孙孙，为什么我感觉如此不安？",
                "至若：我也是，空气中的血腥味越来越浓了。",
                "东湖：咱们早点找到宝贝，回去吧。",
                "恩，我们继续前进吧。",
            };
            c = 0;
            foreach(var t in text) {
                text[c] = string.Format(t, ObjectManager.objectManager.GetMyName());
                c++;
            }
            NpcDialogInterface.ShowTextList(text, null);


            while(!bossSpawn) {
                yield return new WaitForSeconds(1);
            }

            text = new string[]{
                "???: 愚蠢的人类你们唤醒了我，承受我的怒火吧~~",
                "东湖: 上古魔神的声音，这次死定了",
                "这鬼地方怎么会有上古魔神，大家小心",
                "魔神: 好久没有见到新鲜血肉了，这次就让我吃饱吧，哈哈哈",
            };
            bool sover = false;
            NpcDialogInterface.ShowTextList(text, delegate() {
                sover = true;
           });
            while(!sover) {
                yield return new WaitForSeconds(1);
            }
            MyEventSystem.PushEventStatic(MyEvent.EventType.SpeakOver);

            while(!bossDead) {
                yield return new WaitForSeconds(2);
            }


            text = new string[]{
                "魔神:哈哈你们激怒我了，要不是本王刚刚复活，又怎么会败在你们手上\n本王还会回来的，走之前，送你们一个礼物，哈哈~",
                ".......啊，东湖你没事吧?",
                "东湖: 我...我没事......",
            };
            bool donghuSi = false;
            NpcDialogInterface.ShowTextList(text, delegate() {
                donghuSi = true;
                
            });
            while(!donghuSi) {
                yield return new WaitForSeconds(1);
            }
            var npc = NpcManager.Instance.GetNpcObj("东湖");
            var att = npc.GetComponent<NpcAttribute>();
            att.ChangeHP(-att.HP_Max);

            text = new string[]{
                "东湖，东湖，醒醒..醒醒...",
                "至若，我们快送东湖回去找村长",
            };
            bool gover = false;
            NpcDialogInterface.ShowTextList(text, delegate(){
                gover = true;
            });
            while(!gover) {
                yield return new WaitForSeconds(1);
            }

            GameInterface_Player.SetIntState(GameBool.cunZhangState, 2);
            BattleManager.battleManager.GameOver();
        }
    
    }
}
