using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static AlgoClasses.Probability colorPicker;

	private void Awake() {
		colorPicker = new AlgoClasses.Probability();
		int n = UIRoomManager.colorResName.Count;
		int[] p = new int[n];
		for (int i = 0; i < n; i++) {
			p[i] = UIRoomManager.resWeight[UIRoomManager.colorResName[i]];
		}
		colorPicker.SetProb(p);
	}

	private void Start() {
		////float[] ps = new float[] { 60.7f, 30, 90.5f };
		//int[] ps = new int[] { 1, 5, 5, 30, 500 };
		//AlgoClasses.Probability p = new AlgoClasses.Probability();
		//p.SetProb(ps);

		//int[] pp = new int[ps.Length];
		//for (int i = 0; i < 1000000; i++) {
		//	int ii = p.Get();
		//	pp[ii] += 1;
		//}

		//for (int i = 0; i < pp.Length; i++) {
		//	print(i + ": " + pp[i]);
		//}

	}

}
