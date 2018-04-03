using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;

public class TcpClient : RoomCommands {

	public TcpClient(string ipAddr, int port) {
		mySocket.Connect("127.0.0.1", 8000);
		Receive(mySocket, Command);
	}

	public void SSSS(string s) {
		Send(mySocket, s);
	}

	public void CreateOrJoinRoom(string roomName) {
		SendCommand(mySocket, CreateOrJoinRoomCode, new string[] { roomName });
	}

	public void CreateRoom(string roomName) {
		SendCommand(mySocket, CreateRoomCode, new string[] { roomName });
	}

	public void JoinRoom(string roomName) {
		SendCommand(mySocket, JoinRoomCode, new string[] { roomName });
	}

	public void SendRes(List<Details> dList) {
		SendResInfos(mySocket, dList);
	}
	
}
