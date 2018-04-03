using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public static List<Details> resInfoList = new List<Details>();
	public static PlayerInfos playerInfos = new PlayerInfos();
	TcpClient tc;

	public string s;

	void Start() {
		try {
			tc = new TcpClient("127.0.0.1", 8000);
		} catch {
			print("伺服器未開啟");
			return;
		}
		tc.OnJoinedRoom = JoinedRoom;
		tc.ReceiveResCallback = ReceiveRes;

		//tc.SSSS(s);

		//tc.CreateOrJoinRoom("abc");

		//tc.SendRes(SendTest.DetailsDataTest());

		GetRes.ins.GetAllRes(24.99579212, 121.48876185, 500, GetResFromGoogle);
	}

	void GetResFromGoogle(List<Details> resDetails) {
		Debug.Log("Google: " + resDetails.Count);
		tc.SendRes(resDetails);
		//WWWTest.PrintDetails(resNames, resDetails);
	}

	static void ReceiveRes(Socket socket, List<Details> dList) {
		Debug.Log("Received222: " + dList.Count);
		WWWTest.PrintDetails(dList);

	}

	void JoinedRoom(TcpClient.RoomStatus status) {
		print(status);
	}

	private void OnApplicationQuit() {
		if (tc != null) tc.RealseAll();
	}
}
