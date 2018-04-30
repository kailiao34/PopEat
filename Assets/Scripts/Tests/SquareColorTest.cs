using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareColorTest : MonoBehaviour {
	
	public static AlgoClasses.Probability p;

	private void Awake() {
		p = new AlgoClasses.Probability();
		p.SetProb(new int[] { 1, 3 });
	}
	
}
