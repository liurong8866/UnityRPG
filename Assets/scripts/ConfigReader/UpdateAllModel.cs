using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
namespace MyLib
{
    public class UpdateAllModel : MonoBehaviour
    {
        [ButtonCallFunc()] public bool UpdateModel;

        public void UpdateModelMethod()
        {
            var com = gameObject.GetComponentsInChildren<ComplexLayout>();
            foreach (var c in com)
            {
                c.UpdateModelMethod();
            }
        }

        [ButtonCallFunc()] public bool Remove;

        public void RemoveMethod()
        {
            var com = gameObject.GetComponentsInChildren<ComplexLayout>();
            foreach (var c in com)
            {
                c.RemoveMethod();
            }
        }
    }
}
#endif