using System.Collections.Generic;
using System.Net.Sockets;

class Room {
	public List<Socket> sockets = new List<Socket>();
	// 這個房間已經有幾個人按 Ready，如果 = 人數則表示所有人都 Ready了
	public int readyCount = 0;
	// 遊戲結束後，每個餐廳的得分記在這裡
	public Dictionary<string, int> resScore = new Dictionary<string, int>();

	public Room(string res) {
		resScore.Add(res, 0);
	}
}

public class TcpServer : ServerActions {

	Dictionary<Socket, PlayerInfos> infosDict = new Dictionary<Socket, PlayerInfos>();
	Dictionary<string, Room> inWaitRoom = new Dictionary<string, Room>();



	int IDCount = 0;

	const string PlayerInfosCode = "RPI";
	const string GetInfosInRoomCode = "GIIR";
	const string AddNewPlayerCode = "ANP";
	const string PlayerExitCode = "PExit";
	const string ReadyCode = "Ready";
	const string LeaveRoomCode = "LeaveRoom";
	const string GameReadyCode = "GameReady";
	const string StartGameCode = "StartGame";
	const string GameResultCode = "GameResult";

	public TcpServer() {
		// 註冊指令對應的函數
		methods.Add(PlayerInfosCode, RECPlayerInfos);
		methods.Add(ReadyCode, RECReady);
		methods.Add(LeaveRoomCode, RECLeaveRoom);
		methods.Add(GameReadyCode, RECGameReady);
	}

	void RECPlayerInfos(Socket inSocket, string[] inParams) {
		// 如果這個用戶已經進來過則不再處理 (除非先呼叫離開房間命令)
		if (infosDict.ContainsKey(inSocket)) return;

		try {
			// 傳來的 Infos 放入字典
			PlayerInfos pi = new PlayerInfos() {
				roomName = socketDict[inSocket],
				ready = false,
				ID = IDCount++,

				nickName = inParams[0],
				foodSelected = inParams[1]
			};

			infosDict[inSocket] = pi;

			if (inWaitRoom.ContainsKey(pi.roomName)) {
				// 回傳已在房間裡的人的資訊給新進的這位 (為節省網路流量，不連自己的資訊一起傳回)
				List<Socket> sList = inWaitRoom[pi.roomName].sockets;
				string[] paramsStr = new string[sList.Count + 1];
				int ii = 0;
				foreach (Socket s in sList) {
					paramsStr[ii] = InfosStr(infosDict[s]);
					ii++;
				}
				// 最後一個只放這個玩家的ID，若收到參數只有1個的就是 ID
				paramsStr[ii] = GetCmdString(pi.ID.ToString()).ToString();

				SendCommand(inSocket, GetInfosInRoomCode, paramsStr);

				// 回傳給已在房間裡的人這位新進的人的資訊
				SendCommand(sList.ToArray(), AddNewPlayerCode, InfosStr(pi));
			} else {
				inWaitRoom.Add(pi.roomName, new Room(pi.roomName));
				// 只傳送 ID
				SendCommand(inSocket, GetInfosInRoomCode, GetCmdString(pi.ID.ToString()).ToString());
			}

			// 將這個用戶加入等待室
			inWaitRoom[pi.roomName].sockets.Add(inSocket);
		} catch { }
	}

	void RECReady(Socket inSocket, string[] inParams) {
		PlayerInfos pi;
		if (infosDict.TryGetValue(inSocket, out pi)) {
			Room room;
			if (inWaitRoom.TryGetValue(pi.roomName, out room)) {
				// Ready 狀態傳給其它人 (包括傳送者自己)
				SendCommand(room.sockets.ToArray(), ReadyCode, new string[] {
					pi.ID.ToString(),
					(pi.ready) ? "F" : "T"
				});
				bool b = !pi.ready;
				pi.ready = b;
				if (b) {
					room.readyCount++;
				} else {
					room.readyCount--;
				}
			}
		}
	}

	void RECLeaveRoom(Socket inSocket, string[] inParams) {
		PlayerInfos pi;

		if (infosDict.TryGetValue(inSocket, out pi)) {
			Room room;
			if (inWaitRoom.TryGetValue(pi.roomName, out room)) {
				room.sockets.Remove(inSocket);
				if (room.sockets.Count == 0) {
					inWaitRoom.Remove(pi.roomName);
				} else {
					// 告訴其它人這個用戶已離開
					SendCommand(room.sockets.ToArray(), PlayerExitCode, pi.ID.ToString());
				}

				if (pi.ready) room.readyCount--;
			}
			infosDict.Remove(inSocket);
			ClientLeaveRoom(inSocket);
		}
	}

	void RECGameReady(Socket inSocket, string[] inParams) {
		Room room;
		try { room = inWaitRoom[infosDict[inSocket].roomName]; } catch { return; }

		if (room.readyCount == room.sockets.Count) {
			// 廣播開始遊戲
			SendCommand(room.sockets.ToArray(), StartGameCode);
		}
	}

	string InfosStr(PlayerInfos p) {
		return GetCmdString(new string[] {
			p.nickName,
			p.foodSelected,
			p.ready ? "T" : "F",
			p.ID.ToString() }).ToString();
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

		RECLeaveRoom(socket, null);
	}

	T[] SetToArray<T>(HashSet<T> set) {
		if (set == null) return new T[0];

		T[] array = new T[set.Count];

		int ii = 0;
		foreach (T s in set) {
			array[ii] = s;
			ii++;
		}
		return array;
	}
}
