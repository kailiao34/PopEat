using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickerTest : MonoBehaviour {
	
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.A)) {
			float t = Random.Range(5f, 10f);
			Ticker.StartTicker(t, () => { print(t); }); 
		}
	}
}
