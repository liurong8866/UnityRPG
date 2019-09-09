
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using KBEngine;
using System.Collections;

namespace KBEngine
{
	public class Dbg
	{
		public static void INFO_MSG (object s)
		{
			Debug.Log (s);
		}
	
		public static void DEBUG_MSG (object s)
		{
			Debug.Log (s);
		}
	
		public static void WARNING_MSG (object s)
		{
			Debug.LogWarning (s);
		}
	
		public static void ERROR_MSG (object s)
		{
			Debug.LogError (s);
		}

		public static void Assert (bool v, object s)
		{
			if (v) {
				Debug.LogError (s);
			}
		}
	}
}
