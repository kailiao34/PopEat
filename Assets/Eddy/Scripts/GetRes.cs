using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Details {
	public string name;                                             // 餐廳名
	public string address;                                          // 地址
	public string phoneNum;                                         // 電話
	public List<string> openingHours = new List<string>();          // 營業時間 (字串表示)
	public float rating;                                            // 評價分數
	public List<Reviews> reviews = new List<Reviews>();             // 網友評價
	public bool permanentlyClosed;                                  // 如果 True 則此餐廳已永久停業
	internal bool requested;                                     // 內部使用 (已向 Google 要過資料)
	internal string detailUrl;                                       // 取得資料的網址
																	 //public List<string> photos;
}

public class Reviews {
	public string name;
	public int rating;
	public string text;
	public DateTime time = new DateTime(1970, 1, 1);
}

public class GetRes : MonoBehaviour {
	public static GetRes ins;
	public delegate void GetInfoDel(string[] resNames);
	public delegate void GetDetailDel(Details resDetails);

	protected Dictionary<string, Details> resDict = new Dictionary<string, Details>();

	private void Awake() {
		ins = this;
	}

	#region ======================= 外部使用函數 =======================
	/// <summary>
	/// 取得附近餐廳名
	/// </summary>
	/// <param name="lat">緯度 (台灣是 24.xx 或 25.xx)</param>
	/// <param name="lng">經度 (台灣是 121.xx)</param>
	/// <param name="radius">取得餐廳的半徑範圍</param>
	/// <param name="callBackEvent">從 Google 得到資料後的 Callback 函數</param>
	public void Get(double lat, double lng, int radius, GetInfoDel callBackEvent) {
		StartCoroutine(GetInfo(lat, lng, radius, callBackEvent));
	}

	/// <summary>
	/// 輸入餐廳名，取得這家餐廳的詳細資料，如果輸入的名稱錯誤，Callback 的傳入參數會是 Null
	/// </summary>
	/// <param name="resName">餐廳名</param>
	public void GetResDetails(string resName, GetDetailDel callBackEvent) {
		StartCoroutine(GetDetails(resName, callBackEvent));
	}
	#endregion ==========================================================

	#region ======================= 內部使用函數 =======================
	IEnumerator GetDetails(string resName, GetDetailDel callBackEvent) {
		Details d;

		if (resDict.ContainsKey(resName)) {
			d = resDict[resName];

			// 如果還沒取得過，則向 Google 要資料
			if (!d.requested) {
				WWW www = new WWW(d.detailUrl);
				yield return www;

				string text = www.text;
				int s = 0, e = 0;

				#region ===================== 解析 Json =====================
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
				#endregion ==================================================

				d.requested = true;
				AddToDatabase(d);
			}

		} else {
			d = null;       // 輸入的餐廳名沒有在字典裡
		}
		
		if (callBackEvent != null) callBackEvent(d);

	}

	IEnumerator GetInfo(double lat, double lng, int radius, GetInfoDel callBackEvent) {
		WWW www = new WWW("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + lat.ToString() + "," + lng.ToString() + "&radius=" + radius.ToString() +
			"&type=restaurant&language=zh-TW&rankby =distance&key=AIzaSyDiC9HjxWI0dBa9x5hYL9xOmOnWJcFE-zU");
		yield return www;

		string text = www.text;

		List<string> namesList = new List<string>();
		int s = 0, e = 0;

        while (true) {
			string name, url;

			// 取得餐廳名
            s = text.IndexOf("name", s) + 9;
            if (s <= 9) break;
            e = text.IndexOf("\",", s);
			name = text.Substring(s, e - s);

			// 取得餐廳 ID 的 Url
			s = text.IndexOf("place_id", s) + 13;
			if (s <= 13) break;
			e = text.IndexOf("\",", s);
			url = "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + text.Substring(s, e - s) +
				"&language=zh-TW&key=AIzaSyDiC9HjxWI0dBa9x5hYL9xOmOnWJcFE-zU";

			// 填入字典
			Details detail = new Details {
				name = name,
				requested = false,
				permanentlyClosed = false,
				detailUrl = url
			};
			resDict.Add(name, detail);

			namesList.Add(name);
		}

		if (callBackEvent != null) callBackEvent(namesList.ToArray());
	}

	protected virtual void AddToDatabase(Details detail) { }

	#endregion ==========================================================
}
