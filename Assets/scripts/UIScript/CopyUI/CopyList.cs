using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class CopyList : IUserInterface
    {
        List<GameObject> levels;
        GameObject Cell;
        int curChapter = -1;
        List<LevelInfo> allLevels;

        UIGrid grid;
        void Awake() {
            regEvt = new System.Collections.Generic.List<MyEvent.EventType> () {
                MyEvent.EventType.OpenCopyUI,
                MyEvent.EventType.UpdateCopy,
            };
            RegEvent();

            SetCallback ("closeButton", Hide);
            levels = new List<GameObject>();
            Cell = GetName("Cell");
            Cell.SetActive(false);

            grid = GetGrid("Grid");

            SetCallback("last", OnLast);
            SetCallback("next", OnNext);
        }
        void OnLast() {
            if(curChapter <= 1) {
                return;
            }
            curChapter--;
            UpdateFrame();
        }

        void OnNext() {
            var maxChapter = CopyController.copyController.GetCurrentChapter();
            if(curChapter >= maxChapter) {
                return;
            }
            curChapter++;
            UpdateFrame();
        }

        protected override void OnEvent (MyEvent evt)
        {
            Log.GUI ("Update CopyController Update Gui ");
            UpdateFrame ();
        }
        void OnLevel(int levId){
            Log.GUI("OnLevelId "+levId);
            var levData = allLevels[levId];
            if(levData.levelLocal.condition != 0 && allLevels.Count > (levId+1)) {
                var num = BackPack.backpack.GetItemCount(0, levData.levelLocal.condition);
                var item = Util.GetItemData(0, levData.levelLocal.condition);
                if(num <= 0) {
                    WindowMng.windowMng.ShowNotifyLog(string.Format("[ff1010]{0}[-]被魔神强大结界所笼罩,需要[ff1010]{1}[-]打破结界, 似乎[ff1010]商店[-]有此物可以获得。", 
                              levData.levelLocal.name, item.ItemName));
                    return;
                }else {
                    GameInterface_Backpack.UseItem(levData.levelLocal.condition);
                }
            }

            CopyController.copyController.SelectLevel (curChapter, allLevels[levId]);
            WorldManager.worldManager.WorldChangeScene(CopyController.copyController.SelectLevelInfo.levelLocal.id, false);
            Log.GUI("OnCopyLevel "+levId);
        }
        void UpdateFrame() {
            if (curChapter == -1) {
                curChapter = CopyController.copyController.GetCurrentChapter();
            }
            if (curChapter == -1) {
                return;
            }
            Log.GUI("curChapter "+curChapter);
            
            allLevels = CopyController.copyController.GetChapterLevel (curChapter);
            bool lastUnPass = true;
            Log.GUI ("Level Count "+levels.Count+ " "+allLevels.Count);
            for(int i = 0; i < levels.Count; i++){
                levels[i].SetActive(false);
            }
            for (int i = 0; i < allLevels.Count; i++) {
                //NewCell
                if(i >= levels.Count){
                    var nc = GameObject.Instantiate(Cell) as GameObject;
                    nc.transform.parent = Cell.transform.parent;
                    Util.InitGameObject(nc);
                    levels.Add(nc);
                }

                var lcell = levels[i];
                lcell.SetActive(true);
                if(allLevels[i].levelServer.IsPass) {
                    var cc = lcell.GetComponent<CopyCell>();
                    cc.SetTitle(allLevels[i].levelLocal.name);
                    int levelId = i;
                    cc.SetBtnCb(delegate(GameObject g){
                        OnLevel(levelId);
                    });
                }else {
                    if(lastUnPass){
                        lastUnPass = false;
                        var cc = lcell.GetComponent<CopyCell>();
                        cc.SetTitle(allLevels[i].levelLocal.name);
                        int levelId = i;
                        cc.SetBtnCb(delegate(GameObject g){
                            OnLevel(levelId);
                        });

                    }else {
                        lcell.SetActive(false);
                        break;
                    }
                }
            }

            grid.repositionNow = true;
        }

        // Use this for initialization
        void Start()
        {
    
        }
    
        // Update is called once per frame
        void Update()
        {
    
        }
    }
}
