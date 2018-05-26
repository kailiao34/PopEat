using System.Collections.Generic;
using System.Net.Sockets;

public class TcpClient : ClientActions {

	public delegate void PlayerListChangedDel(PlayerInfos pi = null, int playerIndex = -1);
	public Del OnMyInfoChanged;
	/// <summary>
	/// pi - 加入的玩家的資訊 (若是玩家退出這個參數為 null)
	/// playerID - 退出的玩家的ID (若是玩家加入這個參數為 -1)
	/// </summary>
	public PlayerListChangedDel OnPlayerListChanged;
	public Ticker.Del OnStartGame;

	const string PlayerInfosCode = "RPI";
	const string GetInfosInRoomCode = "GIIR";
	const string AddNewPlayerCode = "ANP";
	const string PlayerExitCode = "PExit";
	const string ReadyCode = "Ready";
	const string LeaveRoomCode = "LeaveRoom";
	const string GameReadyCode = "GameReady";
	const string StartGameCode = "StartGame";
	const string GameResultCode = "GameResult";

	public TcpClient() {
		methods.Add(GetInfosInRoomCode, RECGetInfosInRoom);		// 只有階段 2 可接收，收到後改為階段 3
		methods.Add(AddNewPlayerCode, RECAddNewPlayer);			// 只有階段 3 可接收
		methods.Add(PlayerExitCode, RECPlayerExit);				// 只有階段 3 可接收
		methods.Add(ReadyCode, RECReady);						// 只有階段 3 可接收
		methods.Add(StartGameCode, RECStartGame);				// 只有階段 3 可接收，切換場景後改為階段 4
		methods.Add(GameResultCode, RECGameResult);             // 只有階段 4 可接收
	}

	#region ============== 傳送函數 ==============
	/// <summary>
	/// 不包括 Ready
	/// </summary>
	public void SendPlayerInfos(PlayerInfos playerInfos) {
		SendCommand(mySocket, PlayerInfosCode, new string[]{
			playerInfos.nickName, playerInfos.foodSelected
		});
	}
	/// <summary>
	/// 傳送 Ready 指令給伺服器，由伺服器判斷這個玩家目前的 ready 狀態，並廣播給同房間的人 (自己也會收到)
	/// </summary>
	public void SendReady() {
		SendCommand(mySocket, ReadyCode);
	}

	public void SendLeaveRoom() {
		SendCommand(mySocket, LeaveRoomCode);
	}

	public void SendGameReady() {
		SendCommand(mySocket, GameReadyCode);
	}
	/// <summary>
	/// 傳入的字典為: 顏色編號,數量
	/// </summary>
	public void SendGameResult(Dictionary<int, int> colorNum) {
		List<string> s = new List<string>();

		foreach (KeyValuePair<int, int> d in colorNum) {
			s.Add(d.Key.ToString());
			s.Add(d.Value.ToString());
		}
		SendCommand(mySocket, GameResultCode, s.ToArray());
	}
	#endregion =======================================

	#region ============== 接收函數 ==============
	/// <summary>
	/// 當傳送 (SendPlayerInfos) 成功進房後會收到，接收在自己進房前已在房內的所有人的資訊
	/// </summary>
	void RECGetInfosInRoom(Socket inSocket, string[] inParams) {
		if (UIRoomManager.curStage != 2) return;            // 只有階段 2 可接收
		UIRoomManager.playersInRoom.Clear();
		UIRoomManager.curStage = 3;                         // 進到階段 3

		foreach (string s in inParams) {
			PlayerInfos pi = ParsePlayerInfo(s);
			if (pi == null) continue;

			if (OnPlayerListChanged != null) OnPlayerListChanged(pi);
		}
	}

	/// <summary>
	/// 有新玩家加入房間時伺服器將發來此通知
	/// </summary>
	void RECAddNewPlayer(Socket inSocket, string[] inParams) {
		if (inParams == null || inParams.Length < 1) return;
		if (UIRoomManager.curStage != 3) return;                    // 只有階段 3 可接收

		PlayerInfos pi = ParsePlayerInfo(inParams[0]);
		if (pi == null) return;

		if (OnPlayerListChanged != null) OnPlayerListChanged(pi);
	}

	/// <summary>
	/// 當有人離開房間時接收到這個指令，收到的參數為對方的 ID
	/// </summary>
	void RECPlayerExit(Socket inSocket, string[] inParams) {
		if (UIRoomManager.curStage != 3) return;                    // 只有階段 3 可接收
		int id;
		try {
			id = int.Parse(inParams[0]);
		} catch { return; }

		for (int i = 0; i < UIRoomManager.playersInRoom.Count; i++) {
			if (UIRoomManager.playersInRoom[i].ID == id) {
				//UIRoomManager.InfosInRoom.RemoveAt(i);
				if (OnPlayerListChanged != null) OnPlayerListChanged(playerIndex: i);
				break;
			}
		}
	}

	/// <summary>
	/// 當房間內有人改變 Ready 狀態時，會接收到伺服器傳來這個指令
	/// 參數裡包括: 那個人的 ID，True 或 False
	/// </summary>
	void RECReady(Socket inSocket, string[] inParams) {
		if (UIRoomManager.curStage != 3) return;                    // 只有階段 3 可接收
		int id;
		bool ready;
		try {
			id = int.Parse(inParams[0]);
			ready = (inParams[1] == "T");
		} catch { return; }

		if (id == UIRoomManager.myInfos.ID) {       // 如果是自己 Ready，回呼時傳回 -1
			UIRoomManager.myInfos.ready = ready;
			Ticker.StartTicker(0, () => { WaitRoomManager.ins.Ready(-1, ready); });
			return;
		}

		for (int i = 0; i < UIRoomManager.playersInRoom.Count; i++) {
			if (UIRoomManager.playersInRoom[i].ID == id) {
				UIRoomManager.playersInRoom[i].ready = ready;
				Ticker.StartTicker(0, () => { WaitRoomManager.ins.Ready(i, ready); });
				return;
			}
		}
	}

	void RECStartGame(Socket inSocket, string[] inParams) {
		if (UIRoomManager.curStage != 3) return;                    // 只有階段 3 可接收
		Ticker.StartTicker(0, OnStartGame);
	}

	void RECGameResult(Socket inSocket, string[] inParams) {
		if (UIRoomManager.curStage != 4) return;                    // 只有階段 4 可接收
		string res = null;
		int colorIndex = 0;
		try { colorIndex = int.Parse(inParams[0]); } catch { return; }
		res = UIRoomManager.GetResNameFromColor(colorIndex);
		if (res == null) return;

		Ticker.StartTicker(0, () => { UIGameManager.ins.SetGlobalWinner(res, colorIndex); });
		ClientLeave(mySocket);
		
		//UnityEngine.Debug.Log("統計結果: " + colorIndex + " - " + res);
	}
	#endregion =======================================
	/// <summary>
	/// 失敗回傳 null
	/// </summary>
	PlayerInfos ParsePlayerInfo(string inParams) {
		try {
			string[] ps = ExtractParams(inParams);

			// 只收到2個參數，是伺服器給自己的 ID 和代表餐廳的 ColorIndex
			if (ps.Length == 2) {
				UIRoomManager.myInfos.ID = int.Parse(ps[0]);
				UIRoomManager.myInfos.colorIndex = int.Parse(ps[1]);
				if (OnMyInfoChanged != null) OnMyInfoChanged();
				return null;
			} else {
				PlayerInfos pi = new PlayerInfos() {
					nickName = ps[0],
					foodSelected = ps[1],
					ready = ps[2] == "T",
					ID = int.Parse(ps[3]),
					colorIndex = int.Parse(ps[4])
				};
				return pi;
			}
		} catch { return null; };
	}
}
