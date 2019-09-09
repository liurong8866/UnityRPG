using UnityEngine;
using System.Collections;

namespace MyLib
{
    //移动和旋转接口
    public class TankPhysicComponent : GeneraPhysic 
    {
        Rigidbody rigid;
        Vector3 moveValue = Vector3.zero;
        private Vector3 mvDir;
        private bool rot = false;

        NpcAttribute attribute;
        public float maxVelocityChange = 3.0f;
        public float maxRotateChange = 3.0f;
        private float gravity = 1f;

        private GameObject tower;
        private bool grounded = false;

        public static float Multi = 2;
        void Start()
        {
            //var box = Util.FindChildRecursive(transform, "boxColldier").gameObject;
            //rigid =  box.GetComponent<Rigidbody>();
            rigid = this.GetComponent<Rigidbody>();
            Physics.IgnoreLayerCollision((int)GameLayer.Npc, (int)GameLayer.Npc);

            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rigid.mass = 100;
            //rigid.constraints = RigidbodyConstraints.FreezeAll & ~RigidbodyConstraints.FreezePositionY;

            rigid.useGravity = true;
            attribute = GetComponent<NpcAttribute>();
            tower = Util.FindChildRecursive(transform, "tower").gameObject;
            maxRotateChange = attribute.ObjUnitData.jobConfig.rotateSpeed;
        }

        void OnCollisionEnter(Collision col) {
            /*
            Debug.Log("OnCollisionEnter: "+col.gameObject.layer);
            if(col.gameObject.layer == (int)GameLayer.Npc) {
                //var attr = NetworkUtil.GetAttr(col.gameObject);
                var rev = col.relativeVelocity;
                //col.rigidbody.AddForce(-Multi*rev, ForceMode.VelocityChange);
                //this.rigidbody.AddForce(-Multi*rev, ForceMode.VelocityChange);
                this.rigid.MovePosition(this.rigid.position);
            }
            */
        }

        public override void MoveSpeed(Vector3 moveSpeed)
        {
            moveValue = moveSpeed * attribute.GetMoveSpeedCoff();
        }
        public override void TurnTo(Vector3 moveDirection)
        {
            /*
            var y1 = transform.eulerAngles.y;
            var y2 = Quaternion.LookRotation (moveDirection).eulerAngles.y;
            var y3 = Mathf.LerpAngle(y1, y2, attribute.GetMoveSpeedCoff());
            transform.rotation = Quaternion.Euler(new Vector3(0, y3, 0));
            */
            mvDir = moveDirection;
            rot = true;
        }

        //旋转炮台 射击时候 或者安静的时候自动归位
        public override void TurnTower(Vector3 moveDirection)
        {
            return;
            var y = Quaternion.LookRotation(moveDirection).eulerAngles.y;
            Log.Sys("TowerRotate: " + y);
            tower.transform.rotation = Quaternion.Euler(new Vector3(0, y, 0));
        }

        void OnCollisionStay()
        {
            grounded = true;
        }

        void FixedUpdate()
        {
            /*
            if (grounded)
            {
                var targetVelocity = moveValue;
                var oldVelocity = rigid.velocity;
                var velocityDiff = targetVelocity - oldVelocity;
                velocityDiff.y = 0;
                velocityDiff.x = Mathf.Clamp(velocityDiff.x, -maxVelocityChange, maxVelocityChange);
                velocityDiff.z = Mathf.Clamp(velocityDiff.z, -maxVelocityChange, maxVelocityChange);
                rigidbody.AddForce(velocityDiff, ForceMode.VelocityChange);
                moveValue = new Vector3(0, 0, 0);
            }
            */
            //moveValue.y = -gravity;
            var mv = moveValue*Time.fixedDeltaTime;
            this.rigid.MovePosition(this.rigid.position+mv);
            moveValue = Vector3.zero;

            //rigidbody.AddForce(new Vector3(0, -gravity * rigidbody.mass, 0));
            grounded = false;


            if (rot)
            {
                var forwardDir = mvDir;
                var curDir = transform.forward;
                curDir.y = 0;
                forwardDir.y = 0;

                var diffDir = Quaternion.FromToRotation(curDir, forwardDir);
                var diffY = diffDir.eulerAngles.y;
                Log.Sys("diffYIs: " + diffY);
                if (diffY > 180)
                {
                    diffY = diffY - 360;
                }
                if (diffY < -180)
                {
                    diffY = 360 + diffY;
                }

                var rate = 1.0f;
                /*
                var abs = Mathf.Abs(diffY);
                if(abs < 20) {
                    rate = abs/20.0f;
                }
                */

                var dy = Mathf.Clamp(diffY, -maxRotateChange, maxRotateChange);
                //var curSpeed = rigid.angularVelocity;
                //var diffVelocity = dy-curSpeed.y;
                Log.Sys("DirY: " + dy + " diffY: " + diffY);
                //*Time.fixedDeltaTime
                var delta = Quaternion.Euler(new Vector3(0, dy, 0));
                this.rigid.MoveRotation(this.rigid.rotation*delta);
                //rigid.AddTorque(Vector3.up * diffVelocity, ForceMode.VelocityChange);
                rot = false;
            }
        }
        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (ClientApp.Instance.testAI)
            {
                Gizmos.color = Color.red;
                var st = transform.position + new Vector3(0, 2, 0);
                var ed = st + mvDir * 4;
                Gizmos.DrawLine(st, ed);
                //Gizmos.DrawSphere(st, 4);
            }
        }
        #endif
    }


}