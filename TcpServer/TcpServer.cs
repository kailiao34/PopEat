using System.Collections.Generic;
using System.Net.Sockets;

public class TcpServer : ServerActions {

	public Dictionary<Socket, PlayerInfos> infosDict = new Dictionary<Socket, PlayerInfos>();
	public Dictionary<string, List<Socket>> inWaitRoom = new Dictionary<string, List<Socket>>();

	const string PlayerInfosCode = "RPI";
	const string GetInfosInRoomCode = "GIIR";
	const string AddNewPlayerCode = "ANP";

	public TcpServer() {
		// 註冊指令對應的函數
		methods.Add(PlayerInfosCode, RECPlayerInfos);
	}

	void RECPlayerInfos(Socket inSocket, string[] inParams) {
		try {
			// 傳來的 Infos 放入字典
			PlayerInfos pi;
			if (!infosDict.TryGetValue(inSocket, out pi)) {
				pi = new PlayerInfos() {
					roomName = socketDict[inSocket],
					ready = false
				};
			}

			pi.nickName = inParams[0];
			pi.foodSelected = inParams[1];
			infosDict[inSocket] = pi;

			// 回傳已在房間裡的人的資訊給新進的這位
			List<Socket> sList = inWaitRoom[pi.roomName];
			string[] paramsStr = new string[sList.Count];
			for (int i=0; i<sList.Count; i++) {
				paramsStr[i] = InfosStr(infosDict[sList[i]]);
			}
			SendCommand(inSocket, GetInfosInRoomCode, paramsStr, "````");

			// 回傳給已在房間裡的人這位新進的人的資訊
			Send(inWaitRoom[pi.roomName].ToArray(), SendCommand(null, AddNewPlayerCode, InfosStr(pi)));

			// 將這個用戶加入等待室
			if (!inWaitRoom.ContainsKey(pi.roomName)) inWaitRoom.Add(pi.roomName, new List<Socket>());
			inWaitRoom[pi.roomName].Add(inSocket);
		} catch { }
	}

	string InfosStr(PlayerInfos p) {
		System.Text.StringBuilder str = new System.Text.StringBuilder();
		str.Append(p.nickName).Append("{+-}").Append(p.foodSelected).Append("{+-}").
			Append(p.ready).Append("{+-}");
		return str.ToString();
	}

	public void PrintRooms() {
		foreach (KeyValuePair<string, HashSet<Socket>> p in roomDict) {
			System.Console.WriteLine(p.Key + " --> " + p.Value.Count);
		}
	}

	public void PrintInfos() {
		foreach (KeyValuePair<Socket, PlayerInfos> p in infosDict) {
			System.Console.WriteLine("NickName --->" + p.Value.nickName);
			System.Console.WriteLine("foodSelected --->" + p.Value.foodSelected);
			System.Console.WriteLine("roomName --->" + p.Value.roomName);
			System.Console.WriteLine("ready --->" + p.Value.ready);
			System.Console.WriteLine("=====================================");
		}
	}

	protected override void OnDisconnected(Socket socket) {
		base.OnDisconnected(socket);
		infosDict.Remove(socket);
	}
}
