using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class TcpClient : TcpBase {

	public TcpClient(string ipAddr, int port) {
		serverSocket.Connect(ipAddr, port);
		Receive(serverSocket, Received);
	}

	void Received(Socket socket, string data) {
		Debug.Log(data);
	}
}
