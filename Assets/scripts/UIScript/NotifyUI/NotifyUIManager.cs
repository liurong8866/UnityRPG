using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class NotifyData
    {
        public string text;
        public float time;
        public System.Action<GameObject> cb;
        public bool forceTime;
    }

    public class NotifyUIManager : MonoBehaviour
    {
        List<NotifyData> nd = new List<NotifyData>();
        public static NotifyUIManager Instance;

        void Awake()
        {
            Instance = this;
        }

        public void AddNotify(string text, float time, System.Action<GameObject> cb, bool forceTime)
        {
            nd.Add(new NotifyData()
            {
                text = text,
                time = time,
                cb = cb,
                forceTime = forceTime
            });
        }
        // Use this for initialization
        void Start()
        {
            StartCoroutine(CheckNotify());
        }

        IEnumerator CheckNotify()
        {
            float lastFt = 0.1f;
            GameObject lastNotify = null;
            while (true)
            {
                if (nd.Count > 0 && WindowMng.windowMng.GetUIRoot() != null)
                {
                    if (NotifyUI.Instance == null || NotifyUI.Instance.activeSelf == false)
                    {
                        if (lastNotify != null && lastNotify.activeSelf)
                        {
                            yield return new WaitForSeconds(lastFt);
                        }
                        var not = nd [0];
                        nd.RemoveAt(0);
                        var g = WindowMng.windowMng.PushTopNotify("UI/NotifyLog");
                        if (g != null)
                        {
                            g.GetComponent<NotifyUI>().SetText(not.text);
                            var ft = not.time;
                            if (!not.forceTime)
                            {
                                if (nd.Count >= 2)
                                {
                                    ft /= 2;
                                }
                                if (nd.Count >= 4)
                                {
                                    ft /= 4;
                                }
                                if (nd.Count >= 6)
                                {
                                    ft /= 4;    
                                }
                            }
                            lastFt = ft;
                            g.GetComponent<NotifyUI>().SetDurationTime(ft);
                            lastNotify = g;
                        }
                        if (not.cb != null)
                        {
                            not.cb(g);
                        }
                    } else
                    {
                        NotifyUI.Instance.GetComponent<NotifyUI>().ShortTime();
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

    
    }

}