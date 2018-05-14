using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;

public class DataServer : NetworkBehaviour {

	Thread acceptThread;

	const string RECLocationCode = "LocAndRes";

	public DataServer() {
		methods.Add(RECLocationCode, RECLocAndRes);
	}

	public void StartServer(string ipAddr, int port) {
		try {
			//伺服器本身的IP和Port
			mySocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddr), port));
			mySocket.Listen(9999);//最多一次接受多少人連線

			acceptThread = new Thread(Accept);
			acceptThread.Start();

		} catch (Exception ex) { LogError("StartServer: " + ex.Message); }
	}

	private void Accept() {
		while (isRunning) {
			try {
				Socket clientSocket = mySocket.Accept();    // 接收到連線請求
				Receive(clientSocket);        // 開啟接收資料線程
			} catch (Exception ex) { LogError("Accept: " + ex.Message); }
			//LogMessage("Accepted", clientSocket);
			Console.WriteLine("Accepted");
		}
	}
	/// <summary>
	/// 接收位置座標和自行輸入的餐廳
	/// </summary>
	void RECLocAndRes(Socket inSocket, string[] inParams) {
		if (inParams.Length == 2) {
			StringBuilder s = new StringBuilder();
			s.Append(inParams[0]).Append(',').Append(inParams[1]);
			LogMessage(s, "UserLocations.txt");
		}
		if (inParams.Length == 3) {
			StringBuilder s = new StringBuilder();
			s.Append(inParams[0]).Append(',').Append(inParams[1]).Append(',').Append(inParams[2]);
			LogMessage(s, "UserInputRes.txt");
		}
	}

	protected override void OnDisconnected(Socket socket) {
		Console.WriteLine("Client Leave");
	}
}
