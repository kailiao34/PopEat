using System.Net.Sockets;
using System.Threading;

public class ClientActions : NetworkBehaviour {

	public enum ConnectStatus {
		Connecting = 0, Connected = 1, UnIntialized, Failed = 2, Closed = 3
	}

	public ConnectStatus connectStatus = ConnectStatus.UnIntialized;
	public delegate void RoomCallBack(RoomStatus status);
	public RoomCallBack OnJoinedRoom;
	public Del OnConnectedToServer, OnConnectFailed;

	// 指令表
	protected const string CreateOrJoinRoomCode = "NBCOJR";
	protected const string CreateRoomCode = "NBCR";
	protected const string JoinRoomCode = "NBJR";
	protected const string ReciveRoomStatusCode = "NBRS";

	public ClientActions() {
		methods.Add(ReciveRoomStatusCode, RECRoomStatus);	// 註冊指令對應的函數
	}

	public void ConnectToServer(string ipAddr, int port) {
		if (connectStatus <= ConnectStatus.Connected) return;
		connectStatus = ConnectStatus.Connecting;
		new Thread(()=> { Connect(ipAddr, port); }).Start();
	}

	void Connect(string ipAddr, int port) {
		try {
			InitMySocket();
			mySocket.Connect(ipAddr, port);
		} catch (System.Exception e) {
			connectStatus = ConnectStatus.Failed;
			if (OnConnectFailed != null) OnConnectFailed();
			return;
		}
		Receive(mySocket);
		connectStatus = ConnectStatus.Connected;
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
		if (socket == mySocket) connectStatus = ConnectStatus.Closed;
	}

}
