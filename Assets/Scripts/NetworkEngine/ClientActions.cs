using System.Net.Sockets;

public class ClientActions : NetworkBehaviour {

	public bool isConnected;
	public delegate void RoomCallBack(RoomStatus status);
	public RoomCallBack OnJoinedRoom;
	
	public void ConnectToServer(string ipAddr, int port) {
		mySocket.Connect(ipAddr, port);
		Receive(mySocket);
		isConnected = true;

		// 註冊指令對應的函數
		methods.Add(ReciveRoomStatusCode, RECRoomStatus);
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
		SendCommand(mySocket, CreateRoomCode, roomName );
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
