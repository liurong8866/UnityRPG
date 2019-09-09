using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class LightningSystem : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(ShowLightning());
        }

        IEnumerator ShowLightning()
        {
            yield return new WaitForSeconds(3);

            var gi = GraphInit.Instance;
            var initLightCoff = GraphInit.Instance.lightCoff;
            var lt = GraphInit.Instance.lightningTime;

            while (true)
            {
                var passTime = 0.0f;
                while (passTime < GraphInit.Instance.lightningTime)
                {
                    var rate = passTime / lt;
                    var value = GraphInit.Instance.lightningCurve.Evaluate(rate);
                    gi.lightCoff = initLightCoff + value * gi.lightningMagnitude;
                    gi.InitNowMethod();
                    passTime += Time.deltaTime;
                    yield return null;
                }
                gi.lightCoff = initLightCoff;
                gi.InitNowMethod();
                yield return new WaitForSeconds(5);
            }
        }
    }

}