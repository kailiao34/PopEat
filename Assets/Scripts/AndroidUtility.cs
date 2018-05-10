using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidUtility : MonoBehaviour {
	
	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape)) {
			//LogUI.Show("Escape");
		}
	}

	//private void OnApplicationPause(bool pause) {
	//	if (pause) Application.Quit();
	//}

}
