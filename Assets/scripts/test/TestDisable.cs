using UnityEngine;
using System.Collections;

public class TestDisable : MonoBehaviour
{
    void OnEnable()
    {
        Debug.LogError("Enable");
    }

    void OnDisable()
    {
        Debug.LogError("Disable");
    }
}
