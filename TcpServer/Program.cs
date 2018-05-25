﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

class Program {

	static TcpServer server;

	static void Main(string[] args) {
		//new Thread(UserInput).Start();                  // 接收指令的 Thread

		server = new TcpServer(28);             // 最大的等待玩家結果秒數
												//server.StartServer("114.42.162.52", 8056, 30);
		server.StartServer("10.140.0.2", 8056, 30);
	}

	//static void UserInput() {
	//	while (true) {
	//		string s = Console.ReadLine();

	//		if (s == "r") {
	//			server.PrintRooms();
	//		} else if (s == "i") {
	//			server.PrintInfos();
	//		} else if (s == "c") {
	//			server.PrintUserCount();
	//		}
	//	}
	//}
}
