using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class Map105 : CScene
    {
        void  Start()
        {
            StartCoroutine(TakeCare());
        }
        IEnumerator TakeCare() {
            GameObject  myplayer = null;
            while(myplayer == null) {
                yield return new WaitForSeconds(1);
                myplayer = ObjectManager.objectManager.GetMyPlayer();
            }
            yield return new WaitForSeconds(3);

            string[] text = new string[]{
                "传闻此地有大量魔神魂魄盘踞，我需万分小心。",
            };
            NpcDialogInterface.ShowTextList(text, null);
        }
    }


}