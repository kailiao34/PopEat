using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIRoomManager : MonoBehaviour {
	[SerializeField]
	GlobalData GlocalDataAsset;
	public static GlobalData gData;

	#region ========== Colors Variables ==========
	// 被選中的餐廳名列表，Index 對應 colorList，這個列表的數量決定遊戲中有幾種六角形
	public static List<string> colorResName = new List<string>();
	public static Dictionary<string, int> resWeight = new Dictionary<string, int>();         // 這間餐廳有幾人選
	#endregion ===================================

	public static PlayerInfos myInfos = new PlayerInfos();
	public static List<PlayerInfos> playersInRoom = new List<PlayerInfos>();
	public static TcpClient client;
	public static AlgoClasses.Probability colorPicker = new AlgoClasses.Probability();

	string roomName = "";
	[SerializeField]
	Button goButton;

	public static GameObject UIObject;

	private void Awake() {
		if (gData == null) gData = GlocalDataAsset;
	}

	private void Start() {
		LeaveRoom();

		goButton.interactable = false;

		// ***************** Test *****************
		//roomName = "ABAB";
		//ConnectWithRoomName();
		//client.CreateOrJoinRoom("ABAB");
		////CreateRoom("ABAB");
		//myInfos.nickName = "KAI";
		//myInfos.foodSelected = "肯德鴉";
		//myInfos.foodSelected = "喝";
		// ****************************************
	}

	#region ========= 給UI按鈕使用的函數 =========
	public void CreateRoom(string name) {
		roomName = name;
		if (!ConnectWithRoomName()) return;

		if (roomName != myInfos.roomName) {         // 一樣的房名不再創建名間
			client.CreateRoom(roomName);
		}
	}

	public void JoinRoom(string name) {
		roomName = name;
		if (!ConnectWithRoomName()) return;

		if (roomName != myInfos.roomName) {         // 一樣的房名不再加入
			client.JoinRoom(roomName);
		}
	}

	public void GoButton() {
		if (playersInRoom.Count > 0) {              // 已經進入等待室，不能再按這個按鈕 (實際在UI時，這情況不會發生)
			Debug.LogError("已經在等待室");
			return;
		}
		
		myInfos.ready = false;
		client.SendPlayerInfos(myInfos);    // 傳送自己的 Infos 給 伺服器，伺服器將回傳在房裡的人和自己的ID
		ButtonManager.ins.buttonWait();     // 打開等待室UI
	}

	public void NickNameButton(Text nameText) {
		myInfos.nickName = nameText.text;
	}

	public void ReadyButton() {
		if (client != null && client.isConnected) {
			client.SendReady();
		}
	}

	public void LeaveRoom() {
		if (client != null && client.isConnected) {
			client.SendLeaveRoom();
		}
		playersInRoom.Clear();
		WaitRoomManager.ins.ClearAllPlayer();
		myInfos.roomName = "";
		roomName = "";

		colorResName.Clear();
		resWeight.Clear();
	}

    public void StartGameButton() {
		if (roomName == "") return;
		bool canGO = true;

		if (myInfos.ready) {            // 如果自己 Ready 了再檢查別人
			for (int i = 0; i < playersInRoom.Count; i++) {
				// 有一個人沒有 Ready，不給進入遊戲
				if (!playersInRoom[i].ready) {
					canGO = false;
					break;
				}
			}
		} else {
			canGO = false;
		}

		if (canGO) {
			client.SendGameReady();
		} else {
			Debug.LogError("等待所有玩家都 Ready 才可以 GO");
		}

	}
	#endregion ====================================

	#region ========== Server Call Back Functions ==========
	void JoinRoomCallback(TcpClient.RoomStatus status) {

		if (status == NetworkBehaviour.RoomStatus.Created) {                // 成功創建房間 (CreateRoom)
			//Debug.Log("創建成功");
			Ticker.StartTicker(0, ()=> { ButtonManager.ins.EnterRoomCallback(true); });
			myInfos.roomName = roomName;
			Ticker.StartTicker(0, () => { CheckAllSet(); });

		} else if (status == NetworkBehaviour.RoomStatus.RoomExists) {      // 房間已存在 (CreateRoom)
			Debug.LogError("房間已存在，請重新輸入");

		} else if (status == NetworkBehaviour.RoomStatus.Joined) {          // 成功加入房間 (JoinRoom)
																			//Debug.Log("加入成功");
			Ticker.StartTicker(0, () => { ButtonManager.ins.EnterRoomCallback(false); });
			myInfos.roomName = roomName;
			Ticker.StartTicker(0, () => { CheckAllSet(); });

		} else if (status == NetworkBehaviour.RoomStatus.RoomNotExists) {   // 房間不存在 (JoinRoom)
			Debug.LogError("房間不存在，請重新輸入");

		} else if (status == NetworkBehaviour.RoomStatus.RoomFulled) {
			Debug.LogError("這個房間已滿");

		} else if (status == NetworkBehaviour.RoomStatus.Others) {
			Debug.LogError("這個房間已經開始遊戲");

		} else {                                                            // 未知錯誤
			Debug.LogError("錯誤");
		}

	}

	void MyInfoChanged() {
		Ticker.StartTicker(0, WaitRoomManager.ins.LocalPlayer);
		AddRes(myInfos.foodSelected);
	}
	/// <summary>
	/// pi = null 時代表玩家退出房間，此時 index 代表退出的玩家的 index
	/// </summary>
	void ListChangedCallback(PlayerInfos pi, int index) {
		if (pi != null) {                                       // 玩家加入
			playersInRoom.Add(pi);
			AddRes(pi.foodSelected);
			WaitRoomManager.ins.PlayerJoin(pi);

		} else if (index >= 0 && index < playersInRoom.Count) {  // 玩家退出
			pi = playersInRoom[index];
			int weight;
			if (resWeight.TryGetValue(pi.foodSelected, out weight)) {
				if (weight <= 1) {          // 這間餐廳只有這個要退出的人選到，才從列表刪除
					colorResName.Remove(pi.foodSelected);
					resWeight.Remove(pi.foodSelected);
				} else {
					resWeight[pi.foodSelected] = weight - 1;
				}
			}
			playersInRoom.RemoveAt(index);

			Ticker.StartTicker(0, () => { WaitRoomManager.ins.PlayerLeave(index); });
		}
	}

	void StartGameCallback() {
		// 設定顏色取得機率
		int n = colorResName.Count;
		int[] p = new int[n];
		for (int i = 0; i < n; i++) {
			p[i] = resWeight[colorResName[i]];
		}
		colorPicker.SetProb(p);
		// 載入遊戲場景
		ButtonManager.UIswitcher1.SetBool("goToGame", true);
		Ticker.StartTicker(0, () => { SceneManager.LoadScene("GameMain"); });
	}
	#endregion =============================================
	/// <summary>
	/// 如果尚未連線則嘗試連線，若連線失敗返回 false
	/// </summary>
	bool ConnectWithRoomName() {
		if (roomName == "") {
			Debug.LogError("請輸入房名");
			return false;
		}

		if (client == null || !client.isConnected) {
			myInfos.roomName = "";
			try {
				client = new TcpClient();
				client.ConnectToServer(gData.ServerIP, gData.ServerPort);
				client.OnJoinedRoom = JoinRoomCallback;
				client.OnPlayerListChanged = ListChangedCallback;
				client.OnStartGame = StartGameCallback;
				client.OnMyInfoChanged = MyInfoChanged;
			} catch {
				Debug.LogError("伺服器未開啟");
				return false;
			}
		}
		return true;
	}

	void AddRes(string resName) {
		if (resWeight.ContainsKey(resName)) {       // 如果有人和他選一樣的餐廳
			resWeight[resName] += 1;
		} else {
			colorResName.Add(resName);
			resWeight.Add(resName, 1);
		}
	}
	/// <summary>
	/// 確認 暱稱、餐廳、房間都已輸入好後，開放 Go Button
	/// </summary>
	public void CheckAllSet() {
		if (client == null || !client.isConnected || myInfos.roomName == ""
			|| myInfos.nickName == "" ||
			myInfos.foodSelected == "") {
			return;
		}
		goButton.interactable = true;
	}

	/// <summary>
	/// 傳入代表餐廳的顏色編號，取回餐廳名，錯誤回傳 null
	/// </summary>
	public static string GetResNameFromColor(int colorIndex) {
		if (myInfos.colorIndex == colorIndex) return myInfos.foodSelected;

		foreach (PlayerInfos pi in playersInRoom) {
			if (pi.colorIndex == colorIndex) return pi.foodSelected;
		}
		return null;
	}

	private void OnApplicationQuit() {
		if (client != null) client.ApplicationQuit();
		//print("Quit");
	}
}
