using System.Collections.Generic;
using System.Net.Sockets;

public class TcpClient : ClientActions {

	public delegate void ReadyDel(int playerIndex);
	public ReadyDel OnReadyCallback;
	public Del OnPlayerListChanged;

	const string PlayerInfosCode = "RPI";
	const string GetInfosInRoomCode = "GIIR";
	const string AddNewPlayerCode = "ANP";
	const string PlayerExitCode = "PExit";
	const string ReadyCode = "Ready";
	const string LeaveRoomCode = "LeaveRoom";


	public TcpClient() {
		methods.Add(GetInfosInRoomCode, RECGetInfosInRoom);
		methods.Add(AddNewPlayerCode, RECAddNewPlayer);
		methods.Add(PlayerExitCode, RECPlayerExit);
		methods.Add(ReadyCode, RECReady);
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
	#endregion =======================================

	#region ============== 接收函數 ==============
	void RECReady(Socket inSocket, string[] inParams) {
		int id;
		bool ready;
		try {
			id = int.Parse(inParams[0]);
			ready = (inParams[1] == "T");
		} catch { return; }

		for (int i = 0; i < GameManager.InfosInRoom.Count; i++) {
			if (GameManager.InfosInRoom[i].ID == id) {
				GameManager.InfosInRoom[i].ready = ready;
				if (OnReadyCallback != null) OnReadyCallback(i);
				return;
			}
		}
	}
	/// <summary>
	/// 當傳送 (SendPlayerInfos) 成功進房後會收到，接收在自己進房前已在房內的所有人的資訊
	/// </summary>
	void RECGetInfosInRoom(Socket inSocket, string[] inParams) {
		GameManager.InfosInRoom.Clear();

		foreach (string s in inParams) {
			PlayerInfos pi = ParsePlayerInfo(s);
			if (pi == null) continue;

			GameManager.InfosInRoom.Add(pi);
		}

		if (OnPlayerListChanged != null) OnPlayerListChanged();
	}
	/// <summary>
	/// 有新玩家加入房間時伺服器將發來此通知
	/// </summary>
	void RECAddNewPlayer(Socket inSocket, string[] inParams) {
		if (inParams == null || inParams.Length < 1) return;

		PlayerInfos pi = ParsePlayerInfo(inParams[0]);
		if (pi == null) return;

		GameManager.InfosInRoom.Add(pi);
		if (OnPlayerListChanged != null) OnPlayerListChanged();
	}

	void RECPlayerExit(Socket inSocket, string[] inParams) {
		int id;
		try {
			id = int.Parse(inParams[0]);
		} catch { return; }

		for (int i = 0; i < GameManager.InfosInRoom.Count; i++) {
			if (GameManager.InfosInRoom[i].ID == id) {
				GameManager.InfosInRoom.RemoveAt(i);
				break;
			}
		}
		if (OnPlayerListChanged != null) OnPlayerListChanged();
	}
	#endregion =======================================
	/// <summary>
	/// 失敗回傳 null
	/// </summary>
	PlayerInfos ParsePlayerInfo(string inParams) {
		try {
			string[] ps = ExtractParams(inParams);

			// 只收到1個參數，是伺服器給自己的 ID
			if (ps.Length == 1) {
				GameManager.myInfos.ID = int.Parse(ps[0]);
				GameManager.InfosInRoom.Add(GameManager.myInfos);
				return null;
			} else {
				PlayerInfos pi = new PlayerInfos() {
					nickName = ps[0],
					foodSelected = ps[1],
					ready = ps[2] == "T",
					ID = int.Parse(ps[3])
				};
				return pi;
			}
		} catch { return null; };
	}
}
