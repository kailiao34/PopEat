using System.Collections.Generic;
using System;

[Serializable]
public class PlayerInfos {
	public string nickName = "";
	public string roomName = "";
	public string foodSelected = "";
	public bool ready = false;
	public int ID = 0;
	public int colorIndex;
}

public class Details {
	public string name;                                             // 餐廳名
	public string address;                                          // 地址
	public string phoneNum;                                         // 電話
	public List<string> openingHours = new List<string>();          // 營業時間 (字串表示)
	public float rating;                                            // 評價分數
	public List<Reviews> reviews = new List<Reviews>();             // 網友評價
	public bool permanentlyClosed;                                  // 如果 True 則此餐廳已永久停業
    public string opNow;                                              //是否營業中 - BR

	public string placeID;                                  // 用來向Google取得更詳細的餐廳資料 (內部使用)
	public bool visited;									// 用來標記這間餐廳是否己向Google取得過詳細資料，避免重複讀取

}

public class Reviews {
	public string name;
	public int rating;
	public string text;
	public DateTime time = new DateTime(1970, 1, 1);
}
