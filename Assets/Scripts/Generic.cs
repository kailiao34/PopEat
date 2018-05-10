using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generic : MonoBehaviour {

	[SerializeField]
	GlobalData GlocalDataAsset;
	public static GlobalData gData;

	private void Awake() {
		gData = GlocalDataAsset;
	}
	

	private void OnApplicationQuit() {
		if (UIRoomManager.client != null) UIRoomManager.client.ApplicationQuit();
		//print("Quit");
	}
}
