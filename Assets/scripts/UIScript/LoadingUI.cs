
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class LoadingUI : IUserInterface
    {
        Transform load;
        //set Async
        public AsyncOperation async;

        void Start()
        {
            load = this.transform;
        }

        IEnumerator LoadVillage(string name)
        {
            var slider = Util.FindChildRecursive(load, "loadBar").GetComponent<UISlider>();


            //yield return async;
            while (!async.isDone)
            {
                slider.value = async.progress;
                yield return null;
            }
            slider.value = 1;

        }


        //开始加载场景静态数据资源
        public void ShowLoad(string name)
        {
            if (gameObject != null)
            {
                load.gameObject.SetActive(true);
                StartCoroutine(LoadVillage(name));
            }
        }

    }
}