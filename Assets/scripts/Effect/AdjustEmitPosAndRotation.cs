﻿using UnityEngine;
using System.Collections;

namespace MyLib {
	/// <summary>
	/// 根据当前的transform.rotation 调整Effectlayer的发射位置和旋转初始角度 
	/// </summary>
	public class AdjustEmitPosAndRotation : MonoBehaviour {

		//需要等待SKillStateMachine 设置Rotation完成 之后才调整当前的粒子参数
		//动画等配置完成之后 启动粒子
		void Start () {
			var el = GetComponent<EffectLayer> ();
			var oldPos = el.EmitPoint;
			var ydeg = transform.rotation.eulerAngles.y;
			el.EmitPoint = transform.rotation*oldPos;

			el.OriRotationMin = (int)(el.OriRotationMin+ ydeg);
			el.OriRotationMax = (int)(el.OriRotationMax+ ydeg);
			Log.AI ("Adjust Effect Layer emitPoint and rotation "+gameObject.name+" "+ydeg);
		}

	}
}
