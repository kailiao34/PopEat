using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWWTest : MonoBehaviour {

	void Start() {
		GetRes.ins.Get(24.99579212, 121.48876185, 500, PrintNames);
	}

	void PrintNames(string[] resNames) {
		foreach (string s in resNames) {
			GetRes.ins.GetResDetails(s, PrintDetails);
		}
	}

	void PrintDetails(Details dd) {
		print(dd.name);
		print(dd.address);
		print(dd.phoneNum);
		foreach (string s in dd.openingHours) {
			print(s);
		}
		print(dd.rating);
		print("--------- reviews ----------");
		foreach (Reviews r in dd.reviews) {
			print(r.name);
			print(r.rating);
			print(r.text);
			print(r.time);
		}
		print("----------------------------");
		print(dd.permanentlyClosed);
		print(dd.requested);
		print("=======================");
	}
	
}
