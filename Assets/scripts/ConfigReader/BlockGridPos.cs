using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class BlockGridPos : MonoBehaviour
    {
        [ButtonCallFunc()]public bool Sort;

        public void SortMethod()
        {
            var g = transform;
            var count = 0;
            foreach (Transform child in g)
            {
                count++;
                //t.localPosition = Vector3.zero;
                //foreach (Transform child in t)
                //{
                float x = child.localPosition.x;
                int xc = Mathf.RoundToInt(x / 4.0f) * 4;
                float z = child.localPosition.z;
                int zc = Mathf.RoundToInt(z / 4.0f) * 4;

                float y = child.localPosition.y;
                child.localPosition = new Vector3(xc, y, zc);
                //}
            }
        }

    }

}