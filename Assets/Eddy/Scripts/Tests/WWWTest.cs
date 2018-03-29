using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWWTest : MonoBehaviour {
    //public FoodLis foodLis;
    void Start() {
		System.Text.StringBuilder b = new System.Text.StringBuilder();
		b.Append(" ");
		b.Append(3).Append(" ");
		b.Append(5.45f).Append(" sdf");

		print(b.ToString());

		//System.DateTime d = new System.DateTime(1995,6,7,8,9,1);
		//print(d);

		//System.DateTime d1 = new System.DateTime(d.Ticks);
		//print(d1);

		//GetRes.ins.GetAllRes(24.99579212, 121.48876185, 500, PrintDetails);
		//GetRes.ins.Get(24.99579212, 121.48876185, 500, PrintNames);
	}

	//void PrintNames(string[] resNames) {
	//	foreach (string s in resNames) {
	//		GetRes.ins.GetResDetails(s, PrintDetails);
 //       }
 //   }

	void PrintDetails(List<Details> d) {

		foreach (Details dd in d) {
			print(dd.name);
			//foodLis.list.Add(dd.name);

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
			print("=======================");
			//foodLis.Getlist();
		}
	}
}
