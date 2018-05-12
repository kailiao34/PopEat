using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program {
	static void Main(string[] args) {

		DataServer server = new DataServer();
		server.StartServer("36.226.39.243", 8057);

	}
}
