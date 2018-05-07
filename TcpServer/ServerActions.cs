using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

/// <summary>
/// 此腳本才開始有房間的概念
/// </summary>
public class ServerActions : NetworkBehaviour {
	protected int maxClient;

	Thread acceptThread;
	/// <summary>
	/// 用房名找 Socket 的字典
	/// </summary>
	protected Dictionary<string, HashSet<Socket>> roomSocketDict = new Dictionary<string, HashSet<Socket>>();
	/// <summary>
	/// 用 Socket 找房名的字典
	/// </summary>
	protected Dictionary<Socket, string> socketRoomDict = new Dictionary<Socket, string>();

	public void StartServer(string ipAddr, int port, int maxClient = 9999) {
		this.maxClient = maxClient;

		//伺服器本身的IP和Port
		mySocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddr), port));
		mySocket.Listen(9999);//最多一次接受多少人連線

		acceptThread = new Thread(Accept);
		acceptThread.Start();

		// 註冊指令對應的函數
		methods.Add(CreateOrJoinRoomCode, RECCreateOrJoinRoom);
		methods.Add(CreateRoomCode, RECCreateRoom);
		methods.Add(JoinRoomCode, RECJoinRoom);
	}

	private void Accept() {
		while (isRunning) {
			Socket clientSocket = mySocket.Accept();    // 接收到連線請求
			Receive(clientSocket);        // 開啟接收資料線程
			System.Console.WriteLine("Accepted");
		}
	}

	protected override void OnDisconnected(Socket socket) {
		ClientLeaveRoom(socket);
		System.Console.WriteLine("Client Leave");
	}

	protected void ClientLeaveRoom(Socket socket) {
		if (!socketRoomDict.ContainsKey(socket)) return;
		string roomName = socketRoomDict[socket];
		if (roomSocketDict.ContainsKey(roomName)) {
			roomSocketDict[roomName].Remove(socket);
			if (roomSocketDict[roomName].Count == 0) roomSocketDict.Remove(roomName);
		}
		socketRoomDict.Remove(socket);
	}

	public override void ApplicationQuit() {
		base.ApplicationQuit();
		if (acceptThread != null) acceptThread.Abort();
	}

	#region ================ 接收函數 ================
	void RECCreateOrJoinRoom(Socket inSocket, string[] inParams) {
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
	}

	void RECCreateRoom(Socket inSocket, string[] inParams) {
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
	}

	void RECJoinRoom(Socket inSocket, string[] inParams) {
		if (inParams == null || inParams.Length < 1) {              // 沒有輸入房名
			SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.NoRoomName).ToString());
			return;
		}

		if (roomSocketDict.ContainsKey(inParams[0])) {                   // 房間存在，成功加入
			AddToRoom(inParams[0], inSocket, RoomStatus.Joined);                    // 加入此房間
		} else {                                                    // 房間不存在，回傳錯誤		
			SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.RoomNotExists).ToString());
		}
	}
	#endregion ========================================

	/// <summary>
	/// 需先自行判斷房間是否存在，若不存在會報錯
	/// </summary>
	protected virtual void AddToRoom(string roomName, Socket socket, RoomStatus roomStatus) {
		if (roomSocketDict[roomName].Count >= maxClient) {		// 房間已滿
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
		SendCommand(socket, ReciveRoomStatusCode, ((int)roomStatus).ToString());	// 回傳房間狀態
	}
}
