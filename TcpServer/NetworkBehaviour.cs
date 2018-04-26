using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// 在 Tcp 底層上加入接收、傳送指令和參數傳送的功能，指令可以呼叫對應的 Function
/// 使用方法:
///		1. 將從TCP收到的原始字串交給
///		2. 要接收的 Function 和 指令代號的對應註冊到 methods 字典裡
///		3. 用 SendCommand 方法來傳送指令和參數
/// </summary>
public class NetworkBehaviour : TcpBase {

	public enum RoomStatus {
		// 成功訊息 (0 ~ 99)
		Created = 0, Joined = 1,
		// 失敗訊息 ( >= 100)
		NoRoomName = 100, ParamsError = 101, RoomExists = 102, RoomNotExists = 103,
		RoomFulled = 104, Others = 105
	}

	// 指令表
	protected const string CreateOrJoinRoomCode = "NBCOJR";
	protected const string CreateRoomCode = "NBCR";
	protected const string JoinRoomCode = "NBJR";
	protected const string ReciveRoomStatusCode = "NBRS";

	protected Dictionary<string, Methods> methods = new Dictionary<string, Methods>();

	/// <summary>
	/// 傳入字串，第一個空格前為指令代號，第二個空格前為參數數量 (設 n 個)
	/// 接下來每個空格前都是每個參數在這個字串的結束位置 (共有 n 個)
	/// n 個空格後開始是參數
	/// </summary>

	protected override void DataReceived(Socket socket, string data) {
		// 取得指令代號
		int i1 = data.IndexOf(' ');
		if (i1 <= 0) return;
		string code = data.Substring(0, i1);
		if (!methods.ContainsKey(code)) return;         // 如果指令代號沒有被註冊則離開

		string[] inParams = ExtractParams(data.Substring(i1 + 1));
		if (inParams == null) return;                   // 如果參數解析失敗則離開

		methods[code](socket, inParams);                            // 呼叫指令代號對應的 Function
	}

	protected void SendCommand(Socket socket, string cmd, string[] paramsStr = null) {
		if (socket == null) return;
		StringBuilder s = GetCmdString(paramsStr);
		if (s == null) return;
		Send(socket, (s.Insert(0, " ").Insert(0, cmd)).ToString());
	}

	/// <summary>
	/// 只需要傳一個參數時用這一個
	/// </summary>
	protected void SendCommand(Socket socket, string cmd, string paramsStr) {
		if (socket == null) return;
		Send(socket, (GetCmdString(paramsStr).Insert(0, " ").Insert(0, cmd)).ToString());
	}

	protected void SendCommand(Socket[] socket, string cmd, string[] paramsStr = null) {
		if (socket == null) return;
		Send(socket, (GetCmdString(paramsStr).Insert(0, " ").Insert(0, cmd)).ToString());
	}

	protected void SendCommand(Socket[] socket, string cmd, string paramsStr) {
		if (socket == null) return;
		StringBuilder s = GetCmdString(paramsStr);
		if (s == null) return;
		Send(socket, (s.Insert(0, " ").Insert(0, cmd)).ToString());
	}

	/// <summary>
	/// 如果失敗回傳 null
	/// </summary>
	protected StringBuilder GetCmdString(string[] paramsStr) {
		StringBuilder str = new StringBuilder();
		if (paramsStr == null) paramsStr = new string[0];
		int n = paramsStr.Length;

		str.Append(n).Append(' ');
		for (int i = 0; i < n; i++) {
			if (paramsStr[i] == null) return null;
			str.Append(paramsStr[i].Length).Append(' ');
		}

		for (int i = 0; i < n; i++) {
			str.Append(paramsStr[i]);
		}
		return str;
	}

	protected StringBuilder GetCmdString(string paramsStr) {
		StringBuilder str = new StringBuilder();
		if (paramsStr == null || paramsStr.Length == 0) {
			str.Append(0).Append(' ');
		} else {
			str.Append(1).Append(' ').Append(paramsStr.Length).Append(' ').Append(paramsStr);
		}
		return str;
	}

	/// <summary>
	/// 傳入已去掉前面的指令代號 (包括空格) 的字串
	/// 若解析失敗回傳 null
	/// </summary>
	protected string[] ExtractParams(string str) {
		// 取得參數數量
		int pCount;
		try {
			pCount = int.Parse(str.Substring(0, str.IndexOf(' ')));
		} catch { return null; }                         // 如果整個字串找不到空格或不為數字，則離開
		if (pCount < 0) return null;

		List<string> splits = new List<string>(str.Split(new char[] { ' ' }, pCount + 2));
		if (splits.Count != pCount + 2) return null;         // 參數數量和預期的不同

		splits.RemoveAt(0);
		int[] indices = new int[pCount];
		int all = 0;
		try {
			for (int i = 0; i < pCount; i++) {
				indices[i] = int.Parse(splits[i]);
				all += indices[i];
			}
		} catch { return null; }                         // 如果其中一個參數字數不是數字則離開

		string[] inParams = new string[pCount];
		if (splits[pCount].Length != all) return null;             // 預期的字元總數與收到的字數不同，離開

		int prei = 0;
		for (int i = 0; i < pCount; i++) {
			inParams[i] = splits[pCount].Substring(prei, indices[i]);
			prei += indices[i];
		}
		return inParams;
	}
}
