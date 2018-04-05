using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

public class NetworkBehaviour : TcpBase {

	public enum RoomStatus {
		// 成功訊息 (0 ~ 99)
		Created = 0, Joined = 1,
		// 失敗訊息 ( >= 100)
		NoRoomName = 100, RoomExists = 101, RoomNotExists = 102, ParamsError = 103
	}

	// 指令表
	protected const string CreateOrJoinRoomCode = "NBCOJR";
	protected const string CreateRoomCode = "NBCR";
	protected const string JoinRoomCode = "NBJR";
	protected const string ReciveRoomStatusCode = "NBRS";

	protected Dictionary<string, Methods> methods = new Dictionary<string, Methods>();

	/// <summary>
	/// 傳入字串，第一個空格前為指令代號，第二個空格前為用來分割參數的字串 
	/// (最多只取 4 個字元，不可使用"{NUL")，其後為要輸入的參數
	/// </summary>
	protected void Command(Socket socket, string str) {

		int i1 = str.IndexOf(' ');
		if (i1 <= 0) return;

		string code = str.Substring(0, i1);             // 取得指令代號
		if (!methods.ContainsKey(code)) return;         // 如果指令代號沒有被註冊則離開

		i1++;
		string[] inParams = new string[0];

		if (i1 < str.Length) {

			string split;
			int i2;

			int d = str.Length - i1;
			if (d > 9) d = 9;               // 最多只取 4 個字元 + [||-}，共 9 個

			i2 = str.IndexOf("[||-}", i1, d);

			if (i2 <= i1) {                             // 找不到要使用的切割字串 (不切割)
				split = null;
				i2 = i1;
			} else {                                    // 取得切割字串
				split = str.Substring(i1, i2 - i1);
				if (split == "{NUL") split = null;
				i2 += 5;
			}
			if (split == null) {
				inParams = new string[] { str.Substring(i2) };
			} else {
				inParams = str.Substring(i2).Split(new string[] { split }
			, System.StringSplitOptions.None);          // 以空格分開字串
			}

			//System.Console.WriteLine("Code--->" + code + "---END");
			//System.Console.WriteLine("Split--->" + split + "---END");
			//foreach (string s in inParams) {
			//	System.Console.WriteLine(s + "---END");
			//}
			//System.Console.WriteLine("=====================");
			//System.Console.WriteLine("");
		}

		methods[code](socket, inParams);                            // 呼叫指令代號對應的 Function
	}
	/// <summary>
	/// separator 最多只取 4 個字元
	/// </summary>
	protected void SendCommand(Socket socket, string cmd, string separator, string[] paramsStr) {

		StringBuilder str = new StringBuilder();
		string sep;

		str.Append(cmd).Append(' ');
		if (separator == null || separator.Length <= 0) {
			str.Append("{NUL").Append("[||-}");
			sep = "";
		} else {
			if (separator.Length > 4) {
				sep = separator.Substring(0, 4);
			} else {
				sep = separator;
			}
			str.Append(sep).Append("[||-}");
		}
		if (paramsStr != null) {
			int n = paramsStr.Length;
			for (int i = 0; i < n; i++) {
				if (paramsStr[i] == null || paramsStr[i].Length <= 0) continue;
				str.Append(paramsStr[i]);
				if (i < n - 1) str.Append(sep);
			}
		}
		//System.Console.WriteLine(str.ToString());
		//Command(socket, str.ToString());
		Send(socket, str.ToString());
	}

	/// <summary>
	/// 只需要傳一個參數時用這一個
	/// </summary>
	protected void SendCommand(Socket socket, string cmd, string paramsStr) {
		StringBuilder str = new StringBuilder();

		str.Append(cmd).Append(" {NUL[||-}");
		if (paramsStr != null) str.Append(paramsStr);
		Send(socket, str.ToString());
	}

	
}
