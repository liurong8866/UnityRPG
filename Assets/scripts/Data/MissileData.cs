
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class MissileData : MonoBehaviour
	{
		public enum DamageType{
			Physical,
			Magical,
		}
		//初始粒子位置
		//public Vector3 Position = Vector3.zero;

		public GameObject ActiveParticle;
		public GameObject DieParticle;
		public GameObject HitParticle;
		public GameObject ReleaseParticle;

		public float Velocity = 20;
		//子弹的碰撞体大小0.8 才合适 能攻击到 史莱姆 太矮了 半径小 攻击不到
		public float Radius = 0.8f;

		public float MaxDistance = 10;

		//弹跳次数
		public int NumRicochets = 0;
		//弹跳 随机转向方向
		public int RandomAngle = 45;

		//是否掉落到地面上 抛物线形式
		//public bool TrackGround = false;
		
        //是否掉落到特定的位置 抛物线形式
		public bool GotoPosition = false;

		//是否穿透碰撞的对象
		public bool piercing = false;
		//子弹转方向角的最大速率 只用来表现
		public float maxTurnRate = 0;
		//地震粒子在死亡的时候 等待效果消失的时间
		public float DieWaitTime = 0;

		//是否攻击多个目标 爆炸技能需要攻击多个目标爆裂新星 explosive
		//爆炸伤害的范围比较大 而不是 攻击的子弹本身比较大
		public bool IsAOE = false;
		public float AOERadius = 0;

		//抛出的陷阱宠物 子弹 不会对碰撞对象造成伤害 
		public bool DontHurtObject = false;
		//子弹爆炸产生屏幕震动
		public CameraShakeData shakeData;
	}

}
