using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;

public class TcpBase {

	public delegate void ReceiveCallBack(Socket socket, string data);
	protected delegate void QuitDel();
	protected event QuitDel QuitEvent;
	protected bool isRunning = true;
	protected Socket serverSocket;

	Dictionary<Socket, Thread> threadsDict = new Dictionary<Socket, Thread>();

	public TcpBase() {
		//開始連線，設定使用網路、串流、TCP
		serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//new server socket object
		QuitEvent = Realse;
	}

	public void Receive(Socket clientSocket, ReceiveCallBack callBack) {
		Thread threadReceive = new Thread(() => { ReceiveThread(clientSocket, callBack); });
		threadReceive.Start();

		threadsDict.Add(clientSocket, threadReceive);
	}

	public void Send(Socket clientSocket, string data) {
		if (CheckSocket(clientSocket)) {
			clientSocket.Send(Encoding.ASCII.GetBytes(data));
		}
	}

	public void SendToAll(string data) {
		List<Socket> sockets = new List<Socket>(threadsDict.Keys);
		foreach (Socket s in sockets) {
			if (CheckSocket(s)) {
				s.Send(Encoding.ASCII.GetBytes(data));
			}
		}
	}

	public void RealseAll() {
		if (QuitEvent != null) QuitEvent();
	}
	
	/// <summary>
	/// 如果該 Socket 已經斷開連線，則從字典中將它刪除
	/// </summary>
	bool CheckSocket(Socket socket) {
		if (socket == null) return false;
		if (!socket.Connected) {
			if (threadsDict.ContainsKey(socket)) {
				threadsDict[socket].Abort();      // 停止 Receive Thread
				threadsDict.Remove(socket);       // 從字典移除
			}
			return false;
		}
		return true;
	}

	private void ReceiveThread(Socket clientSocket, ReceiveCallBack callBack) {
		byte[] bytes = new byte[256];

		while (clientSocket.Connected && isRunning) {
			clientSocket.Receive(bytes);

			if (callBack != null) callBack(clientSocket, Encoding.ASCII.GetString(bytes));
		}
	}

	private void Realse() {
		isRunning = false;
		// 關閉所有 Client 連線
		foreach (KeyValuePair<Socket, Thread> d in threadsDict) {
			try { d.Value.Abort(); } catch { }
			try { d.Key.Close(); } catch { }
		}
		// 關閉 Server Socket
		if (serverSocket != null) serverSocket.Close();
	}
}
