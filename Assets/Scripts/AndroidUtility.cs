using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidUtility : MonoBehaviour {

	public Animator ExitAppAnimator; //呼叫 退出程式 介面

	void Update() {
		if (Input.GetKeyUp(KeyCode.Escape)) {
			ExitAppUI();
		}
	}

	//private void OnApplicationPause(bool pause) {
	//	if (pause) Application.Quit();
	//}



	//======呼叫 Exit APP 介面========
	public void ExitAppUI() {
		ExitAppAnimator.SetBool("exitApp", !ExitAppAnimator.GetBool("exitApp"));
	}

	//======退出遊戲指令=======
	public void ExitApp() {
		Application.Quit();
		//print("exit confirmed");
	}

}
