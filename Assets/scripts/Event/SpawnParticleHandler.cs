using UnityEngine;
using System.Collections;

namespace EventHandler {
	public class SpawnParticleHandler : IEventHandler {
		#region implemented abstract members of IEventHandler

		public override void OnEvent (MyLib.MyEvent evt)
		{
            GameObject p;
            if(string.IsNullOrEmpty(evt.particle)) {
                p = GameObject.Instantiate (evt.particle2) as GameObject;
            }else {
    			var parName = "particles/" + evt.particle;
                Log.Ani ("Skill spawn particle "+parName);
    			p = GameObject.Instantiate (Resources.Load<GameObject> (parName)) as GameObject;
            }
			NGUITools.AddMissingComponent<RemoveSelf> (p);
			if (!string.IsNullOrEmpty( evt.boneName)) {
				p.transform.parent = MyLib.Util.FindChildRecursive(evt.player.transform, evt.boneName);

				p.transform.localPosition = evt.particleOffset;
				p.transform.localRotation = Quaternion.identity;
				p.transform.localScale = Vector3.one;
			} else {
                var sync = p.AddComponent<MyLib.SyncPosWithTarget>();
                sync.target = evt.player;
                var xft = p.GetComponent<XffectComponent>();//.enabled = false;
                xft.enabled = false;

				p.transform.localPosition = evt.particleOffset;
				p.transform.localRotation = Quaternion.identity;
				p.transform.localScale = Vector3.one;

                ClientApp.Instance.StartCoroutine(EnableXft(xft));
			}
		}
        IEnumerator EnableXft(XffectComponent xft) {
            yield return null;
            xft.enabled = true;
        }
		public override void Init ()
		{
			regEvent = new System.Collections.Generic.List<MyLib.MyEvent.EventType> () {
				MyLib.MyEvent.EventType.SpawnParticle,
			};
		}
		#endregion

	}
}
