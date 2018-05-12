using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program {
	static void Main(string[] args) {

		DataServer server = new DataServer();
		server.StartServer("118.168.178.10", 8057);

	}
}
