using UnityEngine;
using System.Collections;

namespace MyLib
{
    public partial class Map103 : CScene 
    {
        EvtHandler eh;
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
            if(step == 10) {
                StartCoroutine(StoryProgress());
            }
        }

        //Load 主城剧情 如何表现CG 镜头控制 运用
        //加载主城场景
        //跨场景的剧情
        //场景暗下来 主镜头关闭
        IEnumerator StoryProgress() {
            yield return StartCoroutine(NpcDialogInterface.WaitPlayerInit());
            var text = new string[] {
                "是夜，你独自一人闯入了试炼之地，与此同时, 在村中",
            };
            yield return StartCoroutine(NpcDialogInterface.WaitHandler(
                (cb)=>{
                    NpcDialogInterface.ShowTextList(text, ()=>{
                    cb();
                    }
                );
            }));
                
            NpcDialogInterface.SetBlackAndStopAI();
            text = new string[]{
                "至若：东湖哥哥，你怎么了，夜里来这里干什么？",
                "东湖：我... 我... 我也不知道怎么了，只觉得胸中热气鼓荡，神不知鬼不觉就来到这里了",
                "至若：你不会魔气未净, 我去找巨牙子爷爷",
                "东湖：我... 我... 嘿嘿，小姑娘，不用去了，本王要借你的至阴之气一用",
                "至若：东湖哥哥你怎么了，为何突然说话声音变了， 东湖哥哥 东湖哥哥 ，东... 湖... 哥..., 我头有点晕...",
                "是夜，一夜无话",
            };
            yield return StartCoroutine(
                NpcDialogInterface.WaitHandler((cb)=>{
                    NpcDialogInterface.ShowTextList(text, ()=>{
                        cb();        
                    });
                })
            );
            NpcDialogInterface.ResetBlackAndAI();


            text = new string[]{
                "自从那日与魔神交手之后，心中总是有不详预感，不知道现在村子里面怎么样了，等找到父母留下的宝物，就尽快回去了",
            };

            yield return StartCoroutine(
                NpcDialogInterface.WaitHandler((cb)=>{
                    NpcDialogInterface.ShowTextList(text, ()=>{
                        cb();        
                    });
                })
            );

            yield return StartCoroutine(eh.WaitEvt(MyEvent.EventType.EnterNextZone));
            text = new string[] {
                "前方为何散发出幽幽蓝光，难道宝物就在那里",
            };
            yield return StartCoroutine(
                NpcDialogInterface.WaitHandler((cb)=>{
                    NpcDialogInterface.ShowTextList(text, ()=>{
                        cb();        
                    });
                })
            );

            yield return StartCoroutine(eh.WaitEvt(MyEvent.EventType.BossSpawn));
            text = new string[] {
                "？？？：我被困在这匣子里面整整一千年了，刑木，你怕是早就死了吧哈哈哈",
                "你是何方妖魔，竟敢直呼我先祖的名讳",
                "小娃娃，刚好，我已千年没有进食了，正好吃掉你,解解馋",
            };
            yield return StartCoroutine(
                NpcDialogInterface.WaitHandler((cb)=>{
                    NpcDialogInterface.ShowTextList(text, ()=>{
                        cb();        
                    });
                })
            );

            MyEventSystem.PushEventStatic(MyEvent.EventType.SpeakOver);

            yield return StartCoroutine(eh.WaitEvt(MyEvent.EventType.BossDead));

            text = new string[]{
                "这散落一地的宝石，似乎蕴含着极大的力量，回去问问巨牙子爷爷",
            };

            yield return StartCoroutine(
                NpcDialogInterface.WaitHandler((cb)=>{
                    NpcDialogInterface.ShowTextList(text, ()=>{
                        cb();        
                    });
                })
            );

            yield return StartCoroutine(eh.WaitEvt(MyEvent.EventType.LevelFinish));
            BattleManager.battleManager.GameOver();
            GameInterface_Player.SetIntState(GameBool.cunZhangState, 11);
        }
    }

}