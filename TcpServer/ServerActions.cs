using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;

/// <summary>
/// 此腳本才開始有房間的概念
/// </summary>
public class ServerActions : NetworkBehaviour {
	protected int maxClient;        // 房間最大人數

	Thread acceptThread;
	/// <summary>
	/// 用房名找 Socket 的字典
	/// </summary>
	protected Dictionary<string, HashSet<Socket>> roomSocketDict = new Dictionary<string, HashSet<Socket>>();
	/// <summary>
	/// 用 Socket 找房名的字典
	/// </summary>
	protected Dictionary<Socket, string> socketRoomDict = new Dictionary<Socket, string>();

	// 指令表
	protected const string CreateOrJoinRoomCode = "NBCOJR";
	protected const string CreateRoomCode = "NBCR";
	protected const string JoinRoomCode = "NBJR";
	protected const string ReciveRoomStatusCode = "NBRS";

	public void StartServer(string ipAddr, int port, int maxClient = 9999) {
		this.maxClient = maxClient;
		try {
			//伺服器本身的IP和Port
			mySocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddr), port));
			mySocket.Listen(9999);//最多一次接受多少人連線

			acceptThread = new Thread(Accept);
			acceptThread.Start();

			// 註冊指令對應的函數
			methods.Add(CreateOrJoinRoomCode, RECCreateOrJoinRoom);
			methods.Add(CreateRoomCode, RECCreateRoom);
			methods.Add(JoinRoomCode, RECJoinRoom);
		} catch (Exception ex) { LogError("StartServer: " + ex.Message); }
	}

	private void Accept() {
		while (isRunning) {
			try {
				Socket clientSocket = mySocket.Accept();    // 接收到連線請求
				Receive(clientSocket);        // 開啟接收資料線程
			} catch (Exception ex) { LogError("Accept: " + ex.Message); }
			//LogMessage("Accepted", clientSocket);
			//Console.WriteLine("Accepted");
		}
	}

	protected override void OnDisconnected(Socket socket) {
		ClientLeaveRoom(socket);
		//Console.WriteLine("Client Leave");
		//LogMessage("Client Leave", socket);
	}

	protected void ClientLeaveRoom(Socket socket) {
		try {
			if (!socketRoomDict.ContainsKey(socket)) return;
			string roomName = socketRoomDict[socket];
			if (roomSocketDict.ContainsKey(roomName)) {
				roomSocketDict[roomName].Remove(socket);
				if (roomSocketDict[roomName].Count == 0) roomSocketDict.Remove(roomName);
			}
			socketRoomDict.Remove(socket);
		} catch (Exception ex) { LogError("ClientLeaveRoom: " + ex.Message); }
	}

	public override void ApplicationQuit() {
		base.ApplicationQuit();
		try {
			if (acceptThread != null) acceptThread.Abort();
		} catch (Exception ex) { LogError("ApplicationQuit: " + ex.Message); }
	}

	#region ================ 接收函數 ================
	void RECCreateOrJoinRoom(Socket inSocket, string[] inParams) {
		try {
			if (inParams == null || inParams.Length < 1) {              // 沒有輸入房名
				SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.NoRoomName).ToString());
				return;
			}

			RoomStatus status;

			if (!roomSocketDict.ContainsKey(inParams[0])) {                   // 房間不存在的話創建一個房間
				roomSocketDict.Add(inParams[0], new HashSet<Socket>());
				status = RoomStatus.Created;
			} else {
				status = RoomStatus.Joined;
			}
			AddToRoom(inParams[0], inSocket, status);                        // 加入此房間
		} catch (Exception ex) { LogError("RECCreateOrJoinRoom: " + ex.Message); }
	}

	void RECCreateRoom(Socket inSocket, string[] inParams) {
		try {
			if (inParams == null || inParams.Length < 1) {              // 沒有輸入房名
				SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.NoRoomName).ToString());
				return;
			}

			if (roomSocketDict.ContainsKey(inParams[0])) {                   // 房間已存在，回傳錯誤
				SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.RoomExists).ToString());
			} else {                                                    // 房間不存在，成功創建				
				roomSocketDict.Add(inParams[0], new HashSet<Socket>());
				AddToRoom(inParams[0], inSocket, RoomStatus.Created);                        // 加入此房間
			}
		} catch (Exception ex) { LogError("RECCreateRoom: " + ex.Message); }
	}

	void RECJoinRoom(Socket inSocket, string[] inParams) {
		try {
			if (inParams == null || inParams.Length < 1) {              // 沒有輸入房名
				SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.NoRoomName).ToString());
				return;
			}

			if (roomSocketDict.ContainsKey(inParams[0])) {                   // 房間存在，成功加入
				AddToRoom(inParams[0], inSocket, RoomStatus.Joined);                    // 加入此房間
			} else {                                                    // 房間不存在，回傳錯誤		
				SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.RoomNotExists).ToString());
			}
		} catch (Exception ex) {
			LogError("RECJoinRoom: " + ex.Message);
		}
	}
	#endregion ========================================

	/// <summary>
	/// 需先自行判斷房間是否存在，若不存在會報錯
	/// </summary>
	protected virtual void AddToRoom(string roomName, Socket socket, RoomStatus roomStatus) {
		if (!roomSocketDict.ContainsKey(roomName)) return;

		if (roomSocketDict[roomName].Count >= maxClient) {      // 房間已滿
			SendCommand(socket, ReciveRoomStatusCode, ((int)RoomStatus.RoomFulled).ToString());
			return;
		}

		string oldRoom;
		if (socketRoomDict.TryGetValue(socket, out oldRoom)) {              // 這個連線者已經在其它房間
			if (oldRoom != roomName) {
				roomSocketDict[oldRoom].Remove(socket);                         // 將他換過來
				if (roomSocketDict[oldRoom].Count == 0) roomSocketDict.Remove(oldRoom);
			}
		}
		roomSocketDict[roomName].Add(socket);                    // 加入此房間
		socketRoomDict[socket] = roomName;
		SendCommand(socket, ReciveRoomStatusCode, ((int)roomStatus).ToString());    // 回傳房間狀態
	}
}
