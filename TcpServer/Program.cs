using System;
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

		//string[] ss = str.Split(new char[] { ' ' }, -1);
		//foreach (string s in ss) {
		//	Console.WriteLine(s);
		//}

		//string s = "RRI  ";
		//int i1 = s.IndexOf(' ') +1;
		//Console.WriteLine(i1);

		//int i2 = s.IndexOf(' ', i1);
		//Console.WriteLine(i2);

		//System.DateTime t = new System.DateTime(long.Parse("636398971690000000"));
		//System.Console.WriteLine(t.ToString());

	}

	static void UserInput() {
		while (true) {
			string s = Console.ReadLine();

			if (s == "r") {
				server.PrintRooms();
			} else if (s == "i") {
				server.PrintInfos();
			}
		}
	}
}
