using System.Collections.Generic;
using System.Net.Sockets;

public class TcpClient : ClientActions {

	public delegate void ReceiveResDel(Socket socket, List<Details> dList);
	public ReceiveResDel ReceiveResCallback;

	const string PlayerInfosCode = "RPI";
	const string GetInfosInRoomCode = "GIIR";
	const string AddNewPlayerCode = "ANP";

	public TcpClient() {
		methods.Add(GetInfosInRoomCode, RECGetInfosInRoom);
		methods.Add(AddNewPlayerCode, RECAddNewPlayer);
	}

	/// <summary>
	/// 不包括 Ready
	/// </summary>
	public void SendPlayerInfos(PlayerInfos playerInfos) {
		SendCommand(mySocket, PlayerInfosCode, new string[]{
			playerInfos.nickName, playerInfos.foodSelected
		});
	}

	void RECGetInfosInRoom(Socket inSocket, string[] inParams) {
		UnityEngine.Debug.Log("===== RECGetInfosInRoom =====");
		foreach (string s in inParams) {
			UnityEngine.Debug.Log(s);
		}
	}

	void RECAddNewPlayer(Socket inSocket, string[] inParams) {
		UnityEngine.Debug.Log("===== RECAddNewPlayer =====");
		foreach (string s in inParams) {
			UnityEngine.Debug.Log(s);
		}
	}
}
