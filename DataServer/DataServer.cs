using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;

public class DataServer : NetworkBehaviour {

	Thread acceptThread;
	string locDataFileName = "UserLocations.txt";
	string resDataFileName = "UserInputRes.txt";
	FileStream locFile, resFile;

	const string RECLocationCode = "LocAndRes";

	public DataServer() {
		locFile = File.Open(locDataFileName, FileMode.Append);
		resFile = File.Open(resDataFileName, FileMode.Append);
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
		if (inParams.Length < 2 || inParams.Length > 3) return;

		StringBuilder s = new StringBuilder();
		for (int i = 0; i < inParams.Length; i++) {
			s.Append(inParams[i]).Append(',');
		}

		LogMessage(s, (inParams.Length == 2) ? locFile : resFile);
	}


}
