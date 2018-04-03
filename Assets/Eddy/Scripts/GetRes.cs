using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GetRes : MonoBehaviour {
	public static GetRes ins;
	public delegate void GetDetailDel(List<Details> resDetails);

	private void Awake() {
		ins = this;
	}

	#region ======================= 外部使用函數 =======================
	public void GetAllRes(double lat, double lng, int radius, GetDetailDel callBackEvent) {
		StartCoroutine(GetInfo(lat, lng, radius, callBackEvent));
	}

	IEnumerator GetInfo(double lat, double lng, int radius, GetDetailDel callBackEvent) {
		WWW www = new WWW("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + lat.ToString() + "," + lng.ToString() + "&radius=" + radius.ToString() +
			"&type=restaurant&language=zh-TW&rankby =distance&key=AIzaSyDiC9HjxWI0dBa9x5hYL9xOmOnWJcFE-zU");
		yield return www;

		string text1 = www.text;

		List<Details> detailsList = new List<Details>();
		int s1 = 0, e1 = 0;

		#region =========================== 取得餐廳列表 ===========================
		while (true) {
			string name, url;

			// 取得餐廳名
            s1 = text1.IndexOf("name", s1) + 9;
            if (s1 <= 9) break;
            e1 = text1.IndexOf("\",", s1);
			name = text1.Substring(s1, e1 - s1);

			// 取得餐廳 ID 的 Url
			s1 = text1.IndexOf("place_id", s1) + 13;
			if (s1 <= 13) break;
			e1 = text1.IndexOf("\",", s1);
			url = "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + text1.Substring(s1, e1 - s1) +
				"&language=zh-TW&key=AIzaSyDiC9HjxWI0dBa9x5hYL9xOmOnWJcFE-zU";

			// 填入字典
			Details d = new Details {
				name = name,
				permanentlyClosed = false,
			};


			#region ======= 取得各餐廳的 Details =======
			www = new WWW(url);
			yield return www;

			string text = www.text;
			int s = 0, e = 0;

			#region ------------- 解析 Json -------------
			// 是否永遠關閉
			if (text.IndexOf("permanently_closed") > 0) {
				d.permanentlyClosed = true;
			}
			// 取得地址
			s = text.IndexOf("formatted_address") + 22;
			if (s > 22) {
				e = text.IndexOf("\",", s);
				d.address = text.Substring(s, e - s);
			}
			// 取得電話
			s = text.IndexOf("formatted_phone_number") + 27;
			if (s > 27) {
				e = text.IndexOf("\",", s);
				d.phoneNum = text.Substring(s, e - s);
			}
			// 取得營業時間
			s = text.IndexOf("weekday_text");
			if (s > 0) {
				int maxEnd = text.IndexOf("},", s);
				while (true) {
					s = text.IndexOf("星期", s);
					if (s <= 0 || s >= maxEnd) break;
					e = text.IndexOf("\"", s);
					d.openingHours.Add(text.Substring(s, e - s));
					s = e;
				}
			}
			// 取得評價分數
			s = text.IndexOf("rating") + 10;
			if (s > 10) {
				e = text.IndexOf(",", s);
				float rate;
				if (float.TryParse(text.Substring(s, e - s), out rate)) {
					d.rating = rate;
				}
			}
			// 取得評價留言
			s = text.IndexOf("reviews");
			if (s > 0) {
				while (true) {
					Reviews review = new Reviews();

					// 留言者暱稱
					s = text.IndexOf("author_name", s) + 16;
					if (s <= 16) break;
					e = text.IndexOf("\"", s);
					review.name = text.Substring(s, e - s);

					// 給予的評價分數
					s = text.IndexOf("rating", s) + 10;
					if (s <= 10) break;
					e = text.IndexOf(",", s);
					int rate;
					if (int.TryParse(text.Substring(s, e - s), out rate)) {
						review.rating = rate;
					}

					// 給予的評價內容
					s = text.IndexOf("text", s) + 9;
					if (s <= 9) break;
					e = text.IndexOf("\"", s);
					review.text = text.Substring(s, e - s);

					// 評價時的時間
					s = text.IndexOf("time", s) + 8;
					if (s <= 8) break;
					e = text.IndexOf("}", s);
					int sec;
					if (int.TryParse(text.Substring(s, e - s), out sec)) {
						review.time = new DateTime(1970, 1, 1).AddSeconds(sec);
					}

					d.reviews.Add(review);
				}
			}
			#endregion -------------------------
			#endregion =================================
			detailsList.Add(d);
		}
		#endregion ==================================================================

		if (callBackEvent != null) callBackEvent(detailsList);
	}

	#endregion ==========================================================
}
