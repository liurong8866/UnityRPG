
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
using System; 
using System.Collections;
namespace KBEngine
{
public class KBEMath {
	public static float int82angle(SByte angle, bool half)
	{
		float halfv = 128f;
		if(half == true)
			halfv = 254f;
		
		halfv = ((float)angle) * ((float)System.Math.PI / halfv);
		return halfv;
	}
	
	public static bool almostEqual(float f1, float f2, float epsilon)
	{
		return Math.Abs( f1 - f2 ) < epsilon;
	}
}
}
