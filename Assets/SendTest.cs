using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendTest : MonoBehaviour {

	Details d;

	void Start () {
		d = new Details();
		d.name = "name";
		d.address = "address";
		d.phoneNum = "phoneNum";
		d.openingHours = new List<string>() { "h1", "h2", "h3" };
		d.rating = 3.1415f;
		d.reviews = new List<Reviews>() {
			new Reviews() { name = "rName", rating = 1, text = "text1", time = new System.DateTime(1995,6,7,8,9,1) },
			new Reviews() { name = "rName1", rating = 2, text = "text2", time = new System.DateTime(1995,9,10,8,9,1) }
		};
		d.permanentlyClosed = true;

		Send();
	}

	void Send() {
		string str = "";
		str += d.name + " ";
		str += d.address + " ";
		str += d.phoneNum + " ";
		str += d.openingHours.Count;

	}
}
