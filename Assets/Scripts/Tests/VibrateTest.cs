using UnityEngine;
using System.Collections;
 
public class VibrateTest : MonoBehaviour {

	float time, timeCount;

	private void Update() {
		if (timeCount < time) {
			//Handheld.Vibrate();
			timeCount += Time.deltaTime;
		} else {
			enabled = false;
		}
	}

	public void Vibrate(float time) {
		this.time = time;
		timeCount = 0;
		enabled = true;
	}

	//	AndroidJavaObject v;

	//	void Start() {
	//#if UNITY_ANDROID
	//		using (AndroidJavaClass p = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
	//			using (AndroidJavaObject a = p.GetStatic<AndroidJavaObject>("currentActivity")) {
	//				v = a.Call<AndroidJavaObject>("getSystemService", "vibrator");
	//			}
	//		}
	//#endif
	//	}

	//	public void Vibrate(int time) { // 此部分要單獨寫成方法(函式)，不可單獨呼叫
	//		v.Call("vibrate", time);
	//	}

}