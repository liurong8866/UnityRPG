﻿using UnityEngine;
using System.Collections;

namespace MyLib
{
	/// <summary>
	/// 调整粒子的发射方向 根据 当前的旋转方向
	/// </summary>
	public class AdjustDirAlways : MonoBehaviour
	{
		Vector2 initDeg;
		Vector3 oldPos;
		void Awake() {


		}
		//需要等待SKillStateMachine 设置Rotation完成 之后才调整当前的粒子参数
		//动画等配置完成之后 启动粒子
		void Start ()
		{
			var el = GetComponent<EffectLayer> ();
			oldPos = el.EmitPoint;
			initDeg = new Vector2 (el.OriRotationMin, el.OriRotationMax);
			Log.AI ("Adjust Effect Layer emitPoint and rotation " + gameObject.name + " ");
		}

		void Update() {
			var el = GetComponent<EffectLayer> ();
			var ydeg = transform.rotation.eulerAngles.y;
			el.EmitPoint = transform.rotation * oldPos;
			
			el.OriRotationMin = (int)(initDeg.x + ydeg);
			el.OriRotationMax = (int)(initDeg.y + ydeg);

		}
	}

}