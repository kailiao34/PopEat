using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class ServerActions : NetworkBehaviour {
	Thread acceptThread;

	/// <summary>
	/// 用房名找 Socket 的字典
	/// </summary>
	protected Dictionary<string, HashSet<Socket>> roomDict = new Dictionary<string, HashSet<Socket>>();
	/// <summary>
	/// 用 Socket 找房名的字典
	/// </summary>
	protected Dictionary<Socket, string> socketDict = new Dictionary<Socket, string>();
	
	public void StartServer(string ipAddr, int port, int maxClient = 9999) {
		QuitEvent += Realse;

		//伺服器本身的IP和Port
		mySocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddr), port));
		mySocket.Listen(maxClient);//最多一次接受多少人連線

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
			Receive(clientSocket, Command);        // 開啟接收資料線程
			System.Console.WriteLine("Accepted");
		}
	}

	protected override void OnDisconnected(Socket socket) {
		if (!socketDict.ContainsKey(socket)) return;
		string roomName = socketDict[socket];
		if (roomDict.ContainsKey(roomName)) {
			roomDict[roomName].Remove(socket);
			if (roomDict[roomName].Count == 0) roomDict.Remove(roomName);
		}
		socketDict.Remove(socket);

		System.Console.WriteLine("Client Leave");
	}

	private void Realse() {
		if (acceptThread != null) acceptThread.Abort();
	}

	#region ================ 接收函數 ================
	void RECCreateOrJoinRoom(Socket inSocket, string[] inParams) {
		if (inParams == null || inParams.Length < 1) {              // 沒有輸入房名
			SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.NoRoomName).ToString());
			return;
		}

		if (!roomDict.ContainsKey(inParams[0])) {                   // 房間不存在的話創建一個房間
			roomDict.Add(inParams[0], new HashSet<Socket>());
			SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.Created).ToString());
		} else {
			SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.Joined).ToString());
		}
		AddToRoom(inParams[0], inSocket);                        // 加入此房間
	}

	void RECCreateRoom(Socket inSocket, string[] inParams) {
		if (inParams == null || inParams.Length < 1) {              // 沒有輸入房名
			SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.NoRoomName).ToString());
			return;
		}

		if (roomDict.ContainsKey(inParams[0])) {                   // 房間已存在，回傳錯誤
			SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.RoomExists).ToString());
		} else {                                                    // 房間不存在，成功創建				
			roomDict.Add(inParams[0], new HashSet<Socket>());
			AddToRoom(inParams[0], inSocket);                        // 加入此房間
			SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.Created).ToString());
		}
	}

	void RECJoinRoom(Socket inSocket, string[] inParams) {
		if (inParams == null || inParams.Length < 1) {              // 沒有輸入房名
			SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.NoRoomName).ToString());
			return;
		}

		if (roomDict.ContainsKey(inParams[0])) {                   // 房間存在，成功加入
			AddToRoom(inParams[0], inSocket);                    // 加入此房間
			SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.Joined).ToString());
		} else {                                                    // 房間不存在，回傳錯誤		
			SendCommand(inSocket, ReciveRoomStatusCode, ((int)RoomStatus.RoomNotExists).ToString());
		}
	}
	#endregion ========================================

	/// <summary>
	/// 需先自行判斷房間是否存在，若不存在會報錯
	/// </summary>
	void AddToRoom(string roomName, Socket socket) {
		string oldRoom;
		if (socketDict.TryGetValue(socket, out oldRoom)) {              // 這個連線者已經在其它房間
			roomDict[oldRoom].Remove(socket);
			if (roomDict[oldRoom].Count == 0) roomDict.Remove(oldRoom);
		}
		roomDict[roomName].Add(socket);                    // 加入此房間
		socketDict[socket] = roomName;
	}

	#region ============= 外部呼叫函數 ==============
	public void SendInRoom(string roomName, string data) {
		if (!roomDict.ContainsKey(roomName)) return;

		List<Socket> sockets = new List<Socket>(roomDict[roomName]);
		foreach (Socket s in sockets) {
			if (CheckSocket(s)) {
				s.Send(Encoding.UTF8.GetBytes(data));
			}
		}
	}
	#endregion ======================================

}
