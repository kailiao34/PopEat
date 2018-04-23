using System.Collections.Generic;
using System.Net.Sockets;

class Room {
	// 房間內的玩家
	public List<Socket> sockets = new List<Socket>();
	// 這個房間已經有幾個人按 Ready，如果 = 人數則表示所有人都 Ready了
	public int readyCount = 0;
	// 進來的人會分配到一個ID，一直往上累加
	public int IDCount = 0;
	// 若 True 表示這個房間已經開始遊戲
	public bool isPlaying = false;
	// 餐廳名 (Key) 和 ColorList Index (Value) 的字典
	Dictionary<string, ColorNum> resColorDict = new Dictionary<string, ColorNum>();
	Queue<string> colorRecycling = new Queue<string>();

	// 用來儲存數字和選這個餐廳的人數
	class ColorNum {
		public string colorIndex;
		public int num;
	}

	public Room(string resName) {
		resColorDict.Add(resName, new ColorNum() { colorIndex = "0", num = 1 });
	}

	public string AddRes(string resName) {
		string colorIndex;
		ColorNum cn;

		if (resColorDict.TryGetValue(resName, out cn)) {                            // 已有相同的餐廳名
			cn.num += 1;
			colorIndex = cn.colorIndex;
		} else {                                                            // 沒有相同的餐廳名
			if (colorRecycling.Count == 0) {        // 沒有可回收的 ColorIndex
				colorIndex = resColorDict.Count.ToString();
			} else {                                // 有可回收的 ColorIndex
				colorIndex = colorRecycling.Dequeue();
			}

			resColorDict.Add(resName, new ColorNum() {  // 餐廳索引, 1人
				colorIndex = colorIndex, num = 1
			});
		}
		return colorIndex;
	}

	public void MinusRes(string resName) {
		ColorNum cn;
		if (resColorDict.TryGetValue(resName, out cn)) {
			if (cn.num > 1) {
				cn.num -= 1;
			} else {
				colorRecycling.Enqueue(cn.colorIndex);		// 回收 ColorIndex
				resColorDict.Remove(resName);
			}
		}
	}
}

public class TcpServer : ServerActions {

	Dictionary<Socket, PlayerInfos> infosDict = new Dictionary<Socket, PlayerInfos>();
	Dictionary<string, Room> inWaitRoom = new Dictionary<string, Room>();

	const string PlayerInfosCode = "RPI";
	const string GetInfosInRoomCode = "GIIR";
	const string AddNewPlayerCode = "ANP";
	const string PlayerExitCode = "PExit";
	const string ReadyCode = "Ready";
	const string LeaveRoomCode = "LeaveRoom";
	// 從客戶端傳送過來，在這裡判斷，若所有人都 Ready 了才向房內所有人廣播 StartGame
	const string GameReadyCode = "GameReady";
	// 客戶端收到這個指令將進入遊戲場景
	const string StartGameCode = "StartGame";
	const string GameResultCode = "GameResult";

	public TcpServer() {
		// 註冊指令對應的函數
		methods.Add(PlayerInfosCode, RECPlayerInfos);
		methods.Add(ReadyCode, RECReady);
		methods.Add(LeaveRoomCode, RECLeaveRoom);
		methods.Add(GameReadyCode, RECGameReady);
	}
	/// <summary>
	/// 玩家按下 Go 按鈕後會發送 (加入等待室請求)
	/// </summary>
	void RECPlayerInfos(Socket inSocket, string[] inParams) {
		// 如果這個用戶已經進來過則不再處理 (除非先呼叫離開房間命令)
		if (infosDict.ContainsKey(inSocket)) return;

		try {
			string roomName = socketDict[inSocket];
			Room room;

			// 傳來的 Infos 放入字典
			PlayerInfos pi = new PlayerInfos() {
				roomName = roomName,
				ready = false,
				ID = 0,
				resIndex = "0",
				nickName = inParams[0],
				foodSelected = inParams[1]
			};

			if (inWaitRoom.TryGetValue(roomName, out room)) {
				// 回傳已在房間裡的人的資訊給新進的這位 (為節省網路流量，不連自己的資訊一起傳回)
				List<Socket> sList = room.sockets;
				string[] paramsStr = new string[sList.Count + 1];
				int ii = 0;
				foreach (Socket s in sList) {
					paramsStr[ii] = InfosStr(infosDict[s]);
					ii++;
				}

				// 賦與新進的這位 ID 和 餐廳 Index
				pi.ID = room.IDCount;
				pi.resIndex = room.AddRes(pi.foodSelected);

				// 最後一個只放這個玩家的ID，若收到參數只有2個的就是 ID 和 resIndex
				paramsStr[ii] = GetCmdString(new string[] {
					pi.ID.ToString(),
					pi.resIndex}).ToString();
				SendCommand(inSocket, GetInfosInRoomCode, paramsStr);

				// 回傳給已在房間裡的人這位新進的人的資訊
				SendCommand(sList.ToArray(), AddNewPlayerCode, InfosStr(pi));
			} else {                                // 第一位進等待室的
				room = new Room(pi.foodSelected);
				inWaitRoom.Add(roomName, room);
				// 只傳送 ID 和 resIndex
				SendCommand(inSocket, GetInfosInRoomCode, GetCmdString(new string[]{"0", "0"}).ToString());
			}

			infosDict[inSocket] = pi;
			room.IDCount++;

			// 將這個用戶加入等待室
			room.sockets.Add(inSocket);
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
				room.MinusRes(pi.foodSelected);
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
			room.isPlaying = true;
		}
	}

	string InfosStr(PlayerInfos p) {
		return GetCmdString(new string[] {
			p.nickName,
			p.foodSelected,
			p.ready ? "T" : "F",
			p.ID.ToString(),
			p.resIndex
		}).ToString();
	}

	protected override void AddToRoom(string roomName, Socket socket, RoomStatus roomStatus) {
		Room room;
		if (inWaitRoom.TryGetValue(roomName, out room)) {
			if (room.isPlaying) {           // 如果這個房間已經開始遊戲
				SendCommand(socket, ReciveRoomStatusCode, ((int)RoomStatus.Others).ToString());    // 回傳房間狀態
				return;
			}
		}
		base.AddToRoom(roomName, socket, roomStatus);
	}

	protected override void OnDisconnected(Socket socket) {
		base.OnDisconnected(socket);

		RECLeaveRoom(socket, null);
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
			System.Console.WriteLine("ID --->" + p.Value.ID);
			System.Console.WriteLine("resIndex --->" + p.Value.resIndex);
			System.Console.WriteLine("=====================================");
		}
	}
}
