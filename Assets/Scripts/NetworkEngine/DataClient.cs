
public class DataClient : ClientActions {
	const string RECLocationCode = "LocAndRes";
	public static string preRes = "";

	public DataClient() {
		preRes = UIRoomManager.myInfos.foodSelected;

		if (FoodLis.preResSelected < 0) {               // 餐廳是使用者自行輸入的
			OnConnectedToServer = SendLocAndRes;
		} else {
			OnConnectedToServer = SendLocation;
		}
	}

	void SendLocation() {
		SendCommand(mySocket, RECLocationCode, new string[] { GetRes.lat.ToString(), GetRes.lng.ToString() });
		ApplicationQuit();
	}

	void SendLocAndRes() {
		SendCommand(mySocket, RECLocationCode, new string[] {
			GetRes.lat.ToString(),
			GetRes.lng.ToString(),
			UIRoomManager.myInfos.foodSelected
		});
		ApplicationQuit();
	}

}
