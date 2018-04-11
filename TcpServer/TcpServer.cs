﻿using System.Collections.Generic;
using System.Net.Sockets;

public class TcpServer : ServerActions {

	Dictionary<Socket, PlayerInfos> infosDict = new Dictionary<Socket, PlayerInfos>();
	Dictionary<string, List<Socket>> inWaitRoom = new Dictionary<string, List<Socket>>();

	int IDCount = 0;

	const string PlayerInfosCode = "RPI";
	const string GetInfosInRoomCode = "GIIR";
	const string AddNewPlayerCode = "ANP";
	const string PlayerExitCode = "PExit";
	const string ReadyCode = "Ready";
	const string LeaveRoomCode = "LeaveRoom";

	public TcpServer() {
		// 註冊指令對應的函數
		methods.Add(PlayerInfosCode, RECPlayerInfos);
		methods.Add(ReadyCode, RECReady);
		methods.Add(LeaveRoomCode, RECLeaveRoom);
	}

	void RECPlayerInfos(Socket inSocket, string[] inParams) {
		// 如果這個用戶已經進來過則不再處理 (除非先呼叫離開房間命令)
		if (infosDict.ContainsKey(inSocket)) return;

		try {
			// 傳來的 Infos 放入字典
			PlayerInfos pi = new PlayerInfos() {
				roomName = socketDict[inSocket],
				ready = false,
				ID = IDCount++,

				nickName = inParams[0],
				foodSelected = inParams[1]
			};

			infosDict[inSocket] = pi;

			if (inWaitRoom.ContainsKey(pi.roomName)) {
				// 回傳已在房間裡的人的資訊給新進的這位 (為節省網路流量，不連自己的資訊一起傳回)
				List<Socket> sList = inWaitRoom[pi.roomName];
				string[] paramsStr = new string[sList.Count+1];
				int ii = 0;
				foreach (Socket s in sList) {
					paramsStr[ii] = InfosStr(infosDict[s]);
					ii++;
				}
				// 最後一個只放這個玩家的ID，若收到參數只有1個的就是 ID
				paramsStr[ii] = GetCmdString(pi.ID.ToString()).ToString();

				SendCommand(inSocket, GetInfosInRoomCode, paramsStr);

				// 回傳給已在房間裡的人這位新進的人的資訊
				SendCommand(sList.ToArray(), AddNewPlayerCode, InfosStr(pi));
			} else {
				inWaitRoom.Add(pi.roomName, new List<Socket>());
				// 只傳送 ID
				SendCommand(inSocket, GetInfosInRoomCode, GetCmdString(pi.ID.ToString()).ToString());
			}

			// 將這個用戶加入等待室
			inWaitRoom[pi.roomName].Add(inSocket);
		} catch { }
	}

	void RECReady(Socket inSocket, string[] inParams) {
		PlayerInfos pi;
		if (infosDict.TryGetValue(inSocket, out pi)) {
			List<Socket> sockets;
			if (inWaitRoom.TryGetValue(pi.roomName, out sockets)) {
				// Ready 狀態傳給其它人 (包括傳送者自己)
				SendCommand(sockets.ToArray(), ReadyCode, new string[] {
					pi.ID.ToString(),
					(pi.ready) ? "F" : "T"
				});
				infosDict[inSocket].ready = !pi.ready;
			}
		}
	}

	void RECLeaveRoom(Socket inSocket, string[] inParams) {
		PlayerInfos pi;

		if (infosDict.TryGetValue(inSocket, out pi)) {
			string room = pi.roomName;
			inWaitRoom[room].Remove(inSocket);
			if (inWaitRoom[room].Count == 0) {
				inWaitRoom.Remove(room);
			} else {
				// 告訴其它人這個用戶已離開
				SendCommand(inWaitRoom[room].ToArray(), PlayerExitCode, pi.ID.ToString());
			}
			infosDict.Remove(inSocket);
		}
	}

	string InfosStr(PlayerInfos p) {
		return GetCmdString(new string[] {
			p.nickName,
			p.foodSelected,
			p.ready ? "T" : "F",
			p.ID.ToString() }).ToString();
	}

	public void PrintRooms() {
		foreach (KeyValuePair<string, HashSet<Socket>> p in roomDict) {
			System.Console.WriteLine(p.Key + " --> " + p.Value.Count);
		}
	}

	public void PrintInfos() {
		foreach (KeyValuePair<Socket, PlayerInfos> p in infosDict) {
			System.Console.WriteLine("NickName --->" + p.Value.nickName);
			System.Console.WriteLine("foodSelected --->" + p.Value.foodSelected);
			System.Console.WriteLine("roomName --->" + p.Value.roomName);
			System.Console.WriteLine("ready --->" + p.Value.ready);
			System.Console.WriteLine("=====================================");
		}
	}

	protected override void OnDisconnected(Socket socket) {
		base.OnDisconnected(socket);

		RECLeaveRoom(socket, null);
	}

	T[] SetToArray<T>(HashSet<T> set) {
		if (set == null) return new T[0];

		T[] array = new T[set.Count];

		int ii = 0;
		foreach (T s in set) {
			array[ii] = s;
			ii++;
		}
		return array;
	}
}
