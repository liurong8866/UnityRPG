using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class SimpleIAP
{
	private static SimpleIAP _Instance;
	public static SimpleIAP GetInstance() {
		if(_Instance == null) {
			_Instance = new SimpleIAP();
		}
		return _Instance;
	}
	#if UNITY_IOS && !UNITY_EDITOR
	[DllImport ("__Internal")]
	private static extern void requestProducts(string productIds);
	[DllImport ("__Internal")]
	private static extern void Charge(string chargeItem);
#endif

	public void LoadProducst (string productIds)
	{
#if UNITY_IOS && !UNITY_EDITOR
		requestProducts(productIds);
#endif
	}
	public void ChargeItem(string chargeItem) {
#if UNITY_IOS && !UNITY_EDITOR
		Charge(chargeItem);
#endif

	}
}
