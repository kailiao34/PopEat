using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareColorTest : MonoBehaviour {

	public ColorList c;
	public static Color[] colors;
	public static AlgoClasses.Probability p;

	private void Awake() {
		colors = c.colors.ToArray();
		p = new AlgoClasses.Probability();
		p.SetProb(new int[] { 1, 3 });
	}
	
}
