using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using System.Text;

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
	#endregion ==========================================================

	IEnumerator GetInfo(double lat, double lng, int radius, GetDetailDel callBackEvent) {
		WWW www = new WWW("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + lat.ToString() + "," + lng.ToString() + "&radius=" + radius.ToString() +
			"&type=restaurant&language=zh-TW&rankby =distance&key=AIzaSyDiC9HjxWI0dBa9x5hYL9xOmOnWJcFE-zU");
		yield return www;

		List<Details> detailsList = new List<Details>();
        JObject responce1 = JObject.Parse(www.text);
        JArray resultsArray = (JArray)responce1["results"];

        #region =========================== 取得餐廳列表 ===========================
        for (int i = 0; i < resultsArray.Count; i++) {
			string name, url;

            JObject result = (JObject)resultsArray[i];
            
            // 取得餐廳名
            name = result.Value<string>("name");
            // 取得餐廳 ID 的 Url
            url = "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + result.Value<string>("place_id") +
                "&language=zh-TW&key=AIzaSyDiC9HjxWI0dBa9x5hYL9xOmOnWJcFE-zU";
            // 填入字典
            Details d = new Details {
				name = name,
				permanentlyClosed = false,
			};

            #region ======= 取得各餐廳的 Details =======
            www = new WWW(url);
			yield return www;

            JObject responce = JObject.Parse(www.text);

			#region ------------- 解析 Json -------------
			// 是否永遠關閉
			if (responce["result"]["permanently_closed"] != null) {
				d.permanentlyClosed = true;
			}
            // 取得地址
            d.address = responce["result"].Value<string>("formatted_address");
            if (d.address.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(d.address[0]);
                //讓數字和中文字之間添加空白，避免一組數字被從中切開(ex. 631 -> 6 換行 31)。
                for (int j = 1; j < d.address.Length; j++)
                {
                    char c = d.address[j];
                    char pc = d.address[j-1];
                    bool isNumber = char.IsNumber(c);
                    bool wasNumber = char.IsNumber(pc);
                    if (isNumber != wasNumber)
                    {
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
                for (int j = 0; j < weekdayArray.Count; j++)
                {
                    d.openingHours.Add(weekdayArray[j].Value<string>());
                }
            }
            // 取得是否營業中
            if (responce["result"]["opening_hours"] != null && responce["result"]["opening_hours"]["open_now"] != null) {
                if (responce["result"]["opening_hours"]["open_now"].Value<bool>())
                {
                    d.opNow = "營業中";
                }
                else d.opNow = "已打烊";
            }
            else d.opNow = "無營業時段資訊";

            // 取得評價分數
            d.rating = responce["result"].Value<float>("rating");
            // 取得評價留言
            JArray reviewArray = (JArray)responce["result"]["reviews"];
			if (reviewArray != null) {
                for (int j = 0; j < reviewArray.Count; j++)
                {
					Reviews review = new Reviews();
                    JObject reviewJObject = (JObject)reviewArray[j];
                    review.name = reviewJObject.Value<string>("author_name");
                    review.rating = reviewJObject.Value<int>("rating");
                    review.text = reviewJObject.Value<string>("text")+"\n";
                    review.time = new DateTime(1970, 1, 1).AddSeconds(reviewJObject.Value<int>("time"));
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

}
