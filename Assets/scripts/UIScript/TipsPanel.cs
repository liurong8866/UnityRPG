
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
	public class TipsPanel : IUserInterface
	{
		UILabel content;
		VoidDelegate onOk;
		void Awake ()
		{
			onOk = null;
			SetCallback("okButton", OnOk);
			SetCallback ("cancelButton", OnCancel);
			content = GetLabel ("ContentLabel");
		}
		void OnOk(GameObject g) {
			Hide (null);
			if (onOk != null) {
				onOk(null);
			}
			//GameInterface_Tips.tipsInterface.EnterScene ();
		}
		void OnCancel(GameObject g) {
			Hide (null);
		}


		public void SetContent(string con) {
			content.text = con;
		}
		public void SetOk(VoidDelegate del) {
			onOk = del;
		}
	}

}