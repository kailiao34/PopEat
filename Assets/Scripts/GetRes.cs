using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using System.Text;

public class GetRes : MonoBehaviour {

	[Header("Google Place Key")]
	public string googlePlaceKey = "AIzaSyDiC9HjxWI0dBa9x5hYL9xOmOnWJcFE-zU";

	public static GetRes ins;
	public delegate void GetDetailsDel(List<Details> resDetails);
	public delegate void GetResNamesDel(List<string> resNames);
	public delegate void GetResDetailDel(Details details);

	Dictionary<string, Details> resDetailDict = new Dictionary<string, Details>();

	private void Awake() {
		ins = this;
	}

	#region ======================= 外部使用函數 =======================
	//public void GetAllResAndDetails(double lat, double lng, int radius, GetDetailsDel callBackEvent) {
	//	//StartCoroutine(GetInfo(lat, lng, radius, callBackEvent));
	//}

	public void GetResNames(double lat, double lng, int radius, GetResNamesDel callBackEvent) {
		StartCoroutine(GetNames(lat, lng, radius, callBackEvent));
	}

	public void GetResDetail(string resName, GetResDetailDel callBackEvent) {
		StartCoroutine(GetDetail(resName, callBackEvent));
	}
	#endregion ==========================================================

	IEnumerator GetNames(double lat, double lng, int radius, GetResNamesDel callBackEvent) {

		StringBuilder url = new StringBuilder();
		url.Append("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=")
			.Append(lat.ToString()).Append(",").Append(lng.ToString()).Append("&radius=").Append(radius.ToString())
			.Append("&type=restaurant&language=zh-TW&rankby =distance&key=").Append(googlePlaceKey);

		WWW www = new WWW(url.ToString());
		yield return www;

		JArray resultsArray = (JArray)JObject.Parse(www.text)["results"];
		List<string> names = new List<string>();

		#region =========================== 取得餐廳列表 ===========================
		for (int i = 0; i < resultsArray.Count; i++) {
			JObject result = (JObject)resultsArray[i];
			string name = result.Value<string>("name");             // 取得餐廳名

			Details d;
			if (!resDetailDict.TryGetValue(name, out d)) {
				d = new Details();
			}

			d.name = name;
			d.permanentlyClosed = false;
			d.placeID = result.Value<string>("place_id");
			d.visited = false;
			names.Add(name);
			resDetailDict[name] = d;
		}
		#endregion ==================================================================

		if (callBackEvent != null) callBackEvent(names);
	}

	IEnumerator GetDetail(string resName, GetResDetailDel callBackEvent) {

		Details d = null;

		if (resDetailDict.TryGetValue(resName, out d)) {
			if (!d.visited) {                       // 如果這間餐廳還沒有向Google要過資料
				StringBuilder url = new StringBuilder();
				url.Append("https://maps.googleapis.com/maps/api/place/details/json?placeid=")
					.Append(d.placeID).Append("&language=zh-TW&key=").Append(googlePlaceKey);

				#region ======= 取得餐廳的 Details =======
				WWW www = new WWW(url.ToString());
				yield return www;

				JObject responce = JObject.Parse(www.text);

				#region ------------- 解析 Json -------------
				// 是否永遠關閉
				if (responce["result"]["permanently_closed"] != null) {
					d.permanentlyClosed = true;
				}
				// 取得地址
				d.address = responce["result"].Value<string>("formatted_address");
				if (d.address.Length > 0) {
					StringBuilder sb = new StringBuilder();
					sb.Append(d.address[0]);
					//讓數字和中文字之間添加空白，避免一組數字被從中切開(ex. 631 -> 6 換行 31)。
					for (int j = 1; j < d.address.Length; j++) {
						char c = d.address[j];
						char pc = d.address[j - 1];
						bool isNumber = char.IsNumber(c);
						bool wasNumber = char.IsNumber(pc);
						if (isNumber != wasNumber) {
							sb.Append(" ");
						}
						sb.Append(c);
					}
					d.address = sb.ToString();
				}

				// 取得電話
				d.phoneNum = responce["result"].Value<string>("formatted_phone_number");
				// 取得營業時間
				if (responce["result"]["opening_hours"] != null && responce["result"]["opening_hours"]["weekday_text"] != null) {
					JArray weekdayArray = (JArray)responce["result"]["opening_hours"]["weekday_text"];
					for (int j = 0; j < weekdayArray.Count; j++) {
						d.openingHours.Add(weekdayArray[j].Value<string>());
					}
				}
				// 取得是否營業中
				if (responce["result"]["opening_hours"] != null && responce["result"]["opening_hours"]["open_now"] != null) {
					if (responce["result"]["opening_hours"]["open_now"].Value<bool>()) {
						d.openNow = 0;
					} else d.openNow = 1;
				} else d.openNow = 2;

				// 取得評價分數
				d.rating = responce["result"].Value<float>("rating");
				// 取得評價留言
				JArray reviewArray = (JArray)responce["result"]["reviews"];
				if (reviewArray != null) {
					for (int j = 0; j < reviewArray.Count; j++) {
						Reviews review = new Reviews();
						JObject reviewJObject = (JObject)reviewArray[j];
						review.name = reviewJObject.Value<string>("author_name");
						review.rating = reviewJObject.Value<int>("rating");
						review.text = reviewJObject.Value<string>("text") + "\n";
						review.time = new DateTime(1970, 1, 1).AddSeconds(reviewJObject.Value<int>("time"));
						d.reviews.Add(review);
					}
				}
				#endregion -------------------------
				#endregion =================================
				d.visited = true;
			}
		}

		if (callBackEvent != null) callBackEvent(d);
	}
}
