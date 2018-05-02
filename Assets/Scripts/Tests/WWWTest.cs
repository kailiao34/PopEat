using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWWTest : MonoBehaviour {
    void Start() {
		//GetRes.ins.GetAllRes(24.99579212, 121.48876185, 500, PrintDetails);
	}

	public static void PrintDetails(List<Details> d) {

		int index = 0;

		foreach (Details dd in d) {
			index++;
			print("============================== (" + index + ") ==============================");
			print("餐廳名 --> " + dd.name);

			print("地址 --> " + dd.address);
			print("電話 --> " + dd.phoneNum);
			print("====== ↓↓↓ 以下是營業時間 (字串型式) ======");
			foreach (string s in dd.openingHours) {
				print(s);
			}
			print("餐廳評價(是數字，到時可以用星號表示之類) --> " + dd.rating);
			print("====== 以下是各個評價留言 (每個留言有4個資訊: 留言者名、給予的評價、留言、留言時間 ======");
			foreach (Reviews r in dd.reviews) {
				print("-------- 評價 --------");
				print(r.name);
				print(r.rating);
				print(r.text);
				print(r.time);
			}

			print("------------------------- 評價完 --------------------------");
			print("餐廳是否已永久關閉 --> " + dd.permanentlyClosed);
			print("===================================================================");
		}
	}
}
