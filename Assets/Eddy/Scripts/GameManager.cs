using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Q)) {
			SceneManager.LoadScene("CommuncationWithServerTest");
		}
	}
}
