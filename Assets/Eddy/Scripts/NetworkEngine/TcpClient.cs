using System.Collections.Generic;
using System.Net.Sockets;

public class TcpClient : ClientActions {

	public delegate void ReceiveResDel(Socket socket, List<Details> dList);
	public ReceiveResDel ReceiveResCallback;

	protected const string ReceiveResInfosCode = "RRI";                   // 接收所有餐廳資訊的指令代號

	public TcpClient() {
		methods.Add(ReceiveResInfosCode, ReceiveResInfos);       // 接收所有餐廳資訊
	}

	const string split = "[,-]", listSplit = "{,-}";
	public void SendResInfos(Socket socket, List<Details> dList) {
		if (dList == null || dList.Count == 0) return;

		string[] param = new string[dList.Count];
		int index = 0;

		foreach (Details d in dList) {
			System.Text.StringBuilder str = new System.Text.StringBuilder();
			str.Append(d.name).Append(split);
			str.Append(d.address).Append(split);
			str.Append(d.phoneNum).Append(split);
			str.Append(d.openingHours.Count).Append(split);
			foreach (string s in d.openingHours) {
				str.Append(s).Append(split);
			}
			str.Append(d.rating).Append(split);
			str.Append(d.reviews.Count).Append(split);
			foreach (Reviews s in d.reviews) {
				str.Append(s.name).Append(split);
				str.Append(s.rating).Append(split);
				str.Append(s.text).Append(split);
				str.Append(s.time.Ticks).Append(split);
			}
			str.Append(d.permanentlyClosed ? "T" : "F");

			param[index++] = str.ToString();
		}

		SendCommand(socket, ReceiveResInfosCode, listSplit, param);
	}

	void ReceiveResInfos(Socket inSocket, string[] inParams) {
		List<Details> dList = new List<Details>();

		try {
			if (inParams.Length < 1) return;


			foreach (string str1 in inParams) {
				Details d = new Details();
				int n, ii;
				string[] paramsStr = str1.Split(new string[] { split }, System.StringSplitOptions.None);

				d.name = paramsStr[0];
				d.address = paramsStr[1];
				d.phoneNum = paramsStr[2];
				n = int.Parse(paramsStr[3]);
				ii = 4;
				for (int i = 0; i < n; i++) {
					d.openingHours.Add(paramsStr[ii++]);
				}
				d.rating = float.Parse(paramsStr[ii++]);
				n = int.Parse(paramsStr[ii++]);
				for (int i = 0; i < n; i++) {
					Reviews r = new Reviews();
					r.name = paramsStr[ii++];
					r.rating = int.Parse(paramsStr[ii++]);
					r.text = paramsStr[ii++];
					r.time = new System.DateTime(long.Parse(paramsStr[ii++]));
					d.reviews.Add(r);
				}
				d.permanentlyClosed = paramsStr[ii++] == "T";

				dList.Add(d);
			}
		} catch { return; }

		//System.Console.WriteLine("dList -->" + dList.Count + " param -->" + inParams.Length);


		if (ReceiveResCallback != null) ReceiveResCallback(inSocket, dList);
	}
}
