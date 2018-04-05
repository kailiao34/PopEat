﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

class Program {

	static TcpServer server;

	static void Main(string[] args) {
		new Thread(UserInput).Start();                  // 接收指令的 Thread

		server = new TcpServer();
		server.StartServer("127.0.0.1", 8056);
		server.ReceiveResCallback = ReceiveRes;

		//string s = "RRI  ";
		//int i1 = s.IndexOf(' ') +1;
		//Console.WriteLine(i1);

		//int i2 = s.IndexOf(' ', i1);
		//Console.WriteLine(i2);

		//System.DateTime t = new System.DateTime(long.Parse("636398971690000000"));
		//System.Console.WriteLine(t.ToString());

	}

	static void ReceiveRes(Socket socket, List<Details> dList) {
		System.Console.WriteLine("Received: " + dList.Count);
		server.SendResInfos(socket, dList);
	}


	static void UserInput() {
		while (true) {
			if (Console.ReadLine() == "r") {
				server.PrintRooms();
			}
		}
	}
}
