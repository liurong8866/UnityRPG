
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

/*
 * A Trigger To Check Whether Enter A Zone
 */
public class EnterZone : MonoBehaviour
{
    public delegate void OnVoidDelegate();
    public OnVoidDelegate OnEnter;
    public bool Enter = false;

    void Awake()
    {
        Enter = false;
    }
    // Use this for initialization
    void Start()
    {
	
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
			
            Enter = true;
            if (OnEnter != null)
            {
                OnEnter();
            }
        }
    }
}

