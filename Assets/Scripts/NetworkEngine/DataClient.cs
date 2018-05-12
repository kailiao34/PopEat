
public class DataClient : ClientActions {
	const string RECLocationCode = "LocAndRes";
	string preRes;
	bool sent = false;

	public DataClient() {
		if (FoodLis.preResSelected < 0) {				// 餐廳是使用者自行輸入的
			OnConnectedToServer = SendLocAndRes;
		} else {
			OnConnectedToServer = SendLocation;
		}
	}

	void SendLocation() {
		if (GetRes.getLocSucceed && !sent) {
			SendCommand(mySocket, RECLocationCode, new string[] { GetRes.lat.ToString(), GetRes.lng.ToString() });
			sent = true;
		}
	}

	void SendLocAndRes() {
		if (UIRoomManager.myInfos.foodSelected == preRes) return;

		SendCommand(mySocket, RECLocationCode, new string[] {
			GetRes.lat.ToString(),
			GetRes.lng.ToString(),
			UIRoomManager.myInfos.foodSelected
		});
		preRes = UIRoomManager.myInfos.foodSelected;
	}

}
