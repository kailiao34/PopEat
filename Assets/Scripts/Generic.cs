using UnityEngine;
using UnityEngine.UI;

public class Generic : MonoBehaviour {

	[SerializeField]
	GlobalData GlocalDataAsset;
	static GlobalData g;
	public static GlobalData gData {
		get {
			if (g == null) g = FindObjectOfType<Generic>().GlocalDataAsset;
			return g;
		}
	}

	private void Awake() {
		g = GlocalDataAsset;
	}

	private void OnApplicationQuit() {
		if (UIRoomManager.client != null) UIRoomManager.client.ApplicationQuit();
		//print("Quit");
	}
}
