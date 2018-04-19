using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Text;

/// <summary>
/// 此腳本負責最基礎的 TCP/IP 功能
/// 1. 負責開線程接收各 Socket 傳來的訊息，儲存這些 Socket 通道用以回覆訊息
/// 2. TCP 底層的字串傳送
/// 3. 處理玩家斷線後的資源清理
/// 4. 錯誤 Log
/// 
/// 使用方式:	1. 接收到連線後呼叫 Receive 持續接收訊息
///				2. 覆寫 DataReceived 以處理每一次收到的資料
///				2. Send 方法用以傳送訊息
///				3. 繼承這個 Class 者可在 ApplicationQuit 覆寫(要呼叫base)結束應用程式時需釋放的資源
///				4. 應用程式關閉前呼叫 ApplicationQuit 以關閉所有通道
///				5. 需要 Log 錯誤時呼叫 LogMessage
/// </summary>
public class TcpBase {

	//public delegate void ReceiveCallBack(Socket socket, string data);
	public delegate void Methods(Socket inSocket, string[] inParams);
	public delegate void Del();
	protected bool isRunning = true;
	protected Socket mySocket;

	Dictionary<Socket, Thread> threadsDict = new Dictionary<Socket, Thread>();

	public TcpBase() {
		//開始連線，設定使用網路、串流、TCP
		mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//new server socket object
	}

	protected void Receive(Socket clientSocket) {
		Thread threadReceive = new Thread(() => { ReceiveThread(clientSocket); });
		threadReceive.Start();

		threadsDict.Add(clientSocket, threadReceive);
	}

	protected void Send(Socket clientSocket, string data) {
		if (CheckSocket(clientSocket)) {
			clientSocket.Send(Encoding.UTF8.GetBytes(data));
		}
	}

	protected void Send(Socket[] sockets, string data) {
		foreach (Socket s in sockets) {
			if (CheckSocket(s)) {
				s.Send(Encoding.UTF8.GetBytes(data));
			}
		}
	}

	public void LogMessage(string msg, Socket socket = null, string fileName = "ErrorLog.txt") {

	}
	/// <summary>
	/// 如果該 Socket 已經斷開連線，則從字典中將它刪除
	/// </summary>
	protected bool CheckSocket(Socket socket) {
		if (socket == null) return false;
		if (!socket.Connected) {
			ClientLeave(socket);
			return false;
		}
		return true;
	}

	private void ReceiveThread(Socket clientSocket) {
		byte[] bytes = new byte[clientSocket.ReceiveBufferSize];
		int size;

		while (clientSocket.Connected && isRunning) {
			try {
				size = clientSocket.Receive(bytes);
				if (size <= 0) {
					ClientLeave(clientSocket);
					return;
				}
			} catch {
				ClientLeave(clientSocket);
				return;
			}
			DataReceived(clientSocket, Encoding.UTF8.GetString(bytes, 0, size));
		}
	}

	void ClientLeave(Socket socket) {
		if (threadsDict.ContainsKey(socket)) {
			OnDisconnected(socket);             // 呼叫斷線事件

			Thread t = threadsDict[socket];
			threadsDict.Remove(socket);       // 從字典移除
			t.Abort();                      // 停止 Receive Thread
		}
	}

	protected virtual void DataReceived(Socket socket, string data) {}

	protected virtual void OnDisconnected(Socket socket) { }
	/// <summary>
	/// 給用戶在應用程式結束時呼叫，用來釋放所有資源
	/// </summary>
	public virtual void ApplicationQuit() {
		isRunning = false;
		// 關閉所有 Client 連線
		foreach (KeyValuePair<Socket, Thread> d in threadsDict) {
			try { d.Value.Abort(); } catch { }
			try { d.Key.Close(); } catch { }
		}
		// 關閉 Server Socket
		if (mySocket != null) mySocket.Close();
	}
}
