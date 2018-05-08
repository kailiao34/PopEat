using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidUtility : MonoBehaviour {
	
	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape)) {
			LogUI.Show("Escape");
		}
	}

	//private void OnApplicationFocus(bool focus) {
		
	//}

	private void OnApplicationQuit() {
		if (UIRoomManager.client != null) UIRoomManager.client.ApplicationQuit();
		//print("Quit");
	}
}
