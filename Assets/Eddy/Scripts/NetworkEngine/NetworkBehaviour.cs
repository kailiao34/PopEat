using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

public class NetworkBehaviour : TcpBase {

	public enum RoomStatus {
		// 成功訊息 (0 ~ 99)
		Created = 0, Joined = 1,
		// 失敗訊息 ( >= 100)
		NoRoomName = 100, RoomExists = 101, RoomNotExists = 102, ParamsError = 103
	}

	public delegate void RoomCallBack(RoomStatus status);
	public RoomCallBack OnJoinedRoom;
	protected Dictionary<string, Methods> methods = new Dictionary<string, Methods>();
	/// <summary>
	/// 用房名找 Socket 的字典
	/// </summary>
	private Dictionary<string, HashSet<Socket>> roomDict = new Dictionary<string, HashSet<Socket>>();
	/// <summary>
	/// 用 Socket 找房名的字典
	/// </summary>
	private Dictionary<Socket, string> socketDict = new Dictionary<Socket, string>();

	protected const string CreateOrJoinRoomCode = "NBCOJR";
	protected const string CreateRoomCode = "NBCR";
	protected const string JoinRoomCode = "NBJR";
	protected const string ReciveRoomStatusCode = "NBRS";

	// 各指令代號
	public NetworkBehaviour() {
		methods.Add(CreateOrJoinRoomCode, CreateOrJoinRoom);
		methods.Add(CreateRoomCode, CreateRoom);
		methods.Add(JoinRoomCode, JoinRoom);
		methods.Add(ReciveRoomStatusCode, ReciveRoomStatus);
	}

	/// <summary>
	/// 傳入字串，第一個空格前為指令代號，第二個空格前為用來分割參數的字串(最多只取4個字元)，
	/// 其後為要輸入的參數
	/// </summary>
	protected void Command(Socket socket, string str) {

		int i1 = str.IndexOf(' ');
		if (i1 <= 0) return;

		string code = str.Substring(0, i1);             // 取得指令代號
		if (!methods.ContainsKey(code)) return;         // 如果指令代號沒有被註冊則離開

		i1++;
		string[] inParams = new string[0];

		if (i1 < str.Length) {

			string split;
			int i2;

			i2 = str.IndexOf(' ', i1);

			if (i2 <= i1) {                             // 找不到要使用的切割字串 (不切割)
				split = null;
				i2 = i1;
			} else {                                    // 取得切割字串
				split = str.Substring(i1, i2 - i1);
				i2++;
			}
			if (split == null) {
				inParams = new string[] { str.Substring(i2) };
			} else {
				inParams = str.Substring(i2).Split(new string[] { split }
			, System.StringSplitOptions.RemoveEmptyEntries);          // 以空格分開字串
			}

			//System.Console.WriteLine("Code--->" + code + "---END");
			//System.Console.WriteLine("Split--->" + split + "---END");
			//foreach (string s in inParams) {
			//	System.Console.WriteLine(s + "---END");
			//}
			//System.Console.WriteLine("=====================");
			//System.Console.WriteLine("");
		}

		methods[code](socket, inParams);                            // 呼叫指令代號對應的 Function
	}

	public void SendCommand(Socket socket, string cmd,
		string[] paramsStr = null, string separator = null) {

		StringBuilder str = new StringBuilder();
		string sep;

		str.Append(cmd).Append(' ');
		if (separator == null || separator.Length <= 0) {
			sep = "";
		} else {
			if (separator.Length > 4) {
				sep = separator.Substring(0, 4);
			} else {
				sep = separator;
			}
			str.Append(sep).Append(' ');
		}
		if (paramsStr != null) {
			int n = paramsStr.Length;
			for (int i = 0; i < n; i++) {
				if (paramsStr[i] == null || paramsStr[i].Length <= 0) continue;
				str.Append(paramsStr[i]);
				if (i < n - 1) str.Append(sep);
			}
		}
		//Command(null, str.ToString());
		Send(socket, str.ToString());
	}

	#region ================ 接收函數 ================
	void CreateOrJoinRoom(Socket inSocket, string[] inParams) {
		if (inParams == null || inParams.Length < 1) {              // 沒有輸入房名
			SendCommand(inSocket, ReciveRoomStatusCode, new string[] { ((int)RoomStatus.NoRoomName).ToString() });
			return;
		}

		if (!roomDict.ContainsKey(inParams[0])) {                   // 房間不存在的話創建一個房間
			roomDict.Add(inParams[0], new HashSet<Socket>());
			SendCommand(inSocket, ReciveRoomStatusCode, new string[] { ((int)RoomStatus.Created).ToString() });
		} else {
			SendCommand(inSocket, ReciveRoomStatusCode, new string[] { ((int)RoomStatus.Joined).ToString() });
		}
		AddToRoom(inParams[0], inSocket);                        // 加入此房間
	}

	void CreateRoom(Socket inSocket, string[] inParams) {
		if (inParams == null || inParams.Length < 1) {              // 沒有輸入房名
			SendCommand(inSocket, ReciveRoomStatusCode, new string[] { ((int)RoomStatus.NoRoomName).ToString() });
			return;
		}

		if (roomDict.ContainsKey(inParams[0])) {                   // 房間已存在，回傳錯誤
			SendCommand(inSocket, ReciveRoomStatusCode, new string[] { ((int)RoomStatus.RoomExists).ToString() });
		} else {                                                    // 房間不存在，成功創建				
			roomDict.Add(inParams[0], new HashSet<Socket>());
			AddToRoom(inParams[0], inSocket);                        // 加入此房間
			SendCommand(inSocket, ReciveRoomStatusCode, new string[] { ((int)RoomStatus.Created).ToString() });
		}
	}

	void JoinRoom(Socket inSocket, string[] inParams) {
		if (inParams == null || inParams.Length < 1) {              // 沒有輸入房名
			SendCommand(inSocket, ReciveRoomStatusCode, new string[] { ((int)RoomStatus.NoRoomName).ToString() });
			return;
		}

		if (roomDict.ContainsKey(inParams[0])) {                   // 房間存在，成功加入
			AddToRoom(inParams[0], inSocket);                    // 加入此房間
			SendCommand(inSocket, ReciveRoomStatusCode, new string[] { ((int)RoomStatus.Joined).ToString() });
		} else {                                                    // 房間不存在，回傳錯誤		
			SendCommand(inSocket, ReciveRoomStatusCode, new string[] { ((int)RoomStatus.RoomNotExists).ToString() });
		}
	}

	void ReciveRoomStatus(Socket inSocket, string[] inParams) {
		if (OnJoinedRoom == null) return;

		if (inParams == null || inParams.Length < 1) {              // 沒有收到 RoomState
			OnJoinedRoom(RoomStatus.ParamsError);
			return;
		}
		try {
			OnJoinedRoom((RoomStatus)int.Parse(inParams[0]));
		} catch {
			OnJoinedRoom(RoomStatus.ParamsError);
		}
	}
	#endregion ========================================

	void AddToRoom(string roomName, Socket socket) {
		roomDict[roomName].Add(socket);                    // 加入此房間
		socketDict[socket] = roomName;
	}

	protected override void OnDisconnected(Socket socket) {
		if (!socketDict.ContainsKey(socket)) return;
		string roomName = socketDict[socket];
		if (roomDict.ContainsKey(roomName)) {
			roomDict[roomName].Remove(socket);
			if (roomDict[roomName].Count == 0) roomDict.Remove(roomName);
		}
		socketDict.Remove(socket);
	}

	#region ============= 外部呼叫函數 ==============
	public void SendInRoom(string roomName, string data) {
		if (!roomDict.ContainsKey(roomName)) return;

		List<Socket> sockets = new List<Socket>(roomDict[roomName]);
		foreach (Socket s in sockets) {
			if (CheckSocket(s)) {
				s.Send(Encoding.ASCII.GetBytes(data));
			}
		}
	}

	#endregion ======================================
}
