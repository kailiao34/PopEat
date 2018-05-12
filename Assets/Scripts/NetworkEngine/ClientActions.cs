using System.Net.Sockets;
using System.Threading;

public class ClientActions : NetworkBehaviour {

	public bool isConnected;
	public delegate void RoomCallBack(RoomStatus status);
	public RoomCallBack OnJoinedRoom;
	public Del OnConnectedToServer;

	// 指令表
	protected const string CreateOrJoinRoomCode = "NBCOJR";
	protected const string CreateRoomCode = "NBCR";
	protected const string JoinRoomCode = "NBJR";
	protected const string ReciveRoomStatusCode = "NBRS";

	public void ConnectToServer(string ipAddr, int port) {
		new Thread(()=> { Connect(ipAddr, port); }).Start();

		// 註冊指令對應的函數
		methods.Add(ReciveRoomStatusCode, RECRoomStatus);
	}

	void Connect(string ipAddr, int port) {
		try {
			mySocket.Connect(ipAddr, port);
		} catch {
			LogUI.Show("伺服器未開啟");
			UIRoomManager.roomWaitForServer = false;
			return;
		}
		Receive(mySocket);
		isConnected = true;
		if (OnConnectedToServer != null) OnConnectedToServer();
	}

	void RECRoomStatus(Socket inSocket, string[] inParams) {
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

	public void CreateOrJoinRoom(string roomName) {
		SendCommand(mySocket, CreateOrJoinRoomCode, roomName);
	}

	public void CreateRoom(string roomName) {
		SendCommand(mySocket, CreateRoomCode, roomName);
	}

	public void JoinRoom(string roomName) {
		SendCommand(mySocket, JoinRoomCode, roomName);
	}

	protected override void OnDisconnected(Socket socket) {
		base.OnDisconnected(socket);
		//UnityEngine.Debug.Log("Server Offline");
		if (socket == mySocket) isConnected = false;
	}

}
