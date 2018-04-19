using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIRoomManager : MonoBehaviour {

	#region ========== Colors Variables ==========
	[SerializeField]
	ColorList colorListAsset;
	public static Color[] colorList;                                // 每一次的顏色順序都會不一樣 (在 Awake 裡打亂)
	// 被選中的餐廳名列表，Index 對應 colorList，這個列表的數量決定遊戲中有幾種六角形
	public static List<string> colorResName = new List<string>();
	public static Dictionary<string,int> resWeight = new Dictionary<string, int>();         // 這間餐廳有幾人選
	#endregion ===================================

	public string ServerIP = "127.0.0.1";
	public int port = 8056;

	public static PlayerInfos myInfos = new PlayerInfos();
	public static List<PlayerInfos> playersInRoom = new List<PlayerInfos>();
	public static TcpClient client;
	[SerializeField]
	string roomName = "";
	// ***************** Test *****************
	public List<PlayerInfos> roomTest;
	// ****************************************
	static bool notInitialized = true;

	private void Awake() {
		colorList = new Color[colorListAsset.colors.Count];
		colorList = Algorithms.GetRandomArray(colorListAsset.colors.ToArray());
	}

	private void Start() {
		if (notInitialized) {
			DontDestroyOnLoad(gameObject);
			notInitialized = false;
		}
		LeaveRoom();
	}


	#region ========= 給UI按鈕使用的函數 =========
	public void CreateRoom() {
		if (!ConnectWithRoomName()) return;

		if (roomName != myInfos.roomName) {         // 一樣的房名不再創建名間
			client.CreateRoom(roomName);
		}
	}

	public void JoinRoom() {
		if (!ConnectWithRoomName()) return;

		if (roomName != myInfos.roomName) {         // 一樣的房名不再加入
			client.JoinRoom(roomName);
		}
	}

	public void GoButton() {
		if (playersInRoom.Count > 0) {				// 已經進入等待室，不能再按這個按鈕 (實際在UI時，這情況不會發生)
			Debug.LogError("已經在等待室");
			return;
		}

		if (client == null || !client.isConnected || myInfos.roomName == "") {
			Debug.LogError("尚未加入房間或建立房間");
			return;
		}
		if (myInfos.nickName == "") {
			Debug.LogError("請輸入暱稱");
			return;
		}
		if (myInfos.foodSelected == "") {
			Debug.LogError("選擇一個餐廳");
			return;
		}

		myInfos.ready = false;
		client.SendPlayerInfos(myInfos);
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
		myInfos = new PlayerInfos();
		myInfos.roomName = "";

		colorResName.Clear();
		resWeight.Clear();
}

	public void StartGameButton() {
		if (playersInRoom.Count <= 0) return;

		for (int i = 0; i < playersInRoom.Count; i++) {
			// 有一個人沒有 Ready，不給進入遊戲
			if (!playersInRoom[i].ready) {
				Debug.LogError("等待所有玩家都 Ready 才可以 GO");
				return;
			}
		}

		client.SendGameReady();
	}
	#endregion ====================================

	#region ========== Server Call Back Functions ==========
	void JoinRoomCallback(TcpClient.RoomStatus status) {

		if (status == NetworkBehaviour.RoomStatus.Created) {                // 成功創建房間 (CreateRoom)
			Debug.Log("創建成功");
			myInfos.roomName = roomName;

		} else if (status == NetworkBehaviour.RoomStatus.RoomExists) {      // 房間已存在 (CreateRoom)
			Debug.LogError("房間已存在，請重新輸入");

		} else if (status == NetworkBehaviour.RoomStatus.Joined) {          // 成功加入房間 (JoinRoom)
			Debug.Log("加入成功");
			myInfos.roomName = roomName;

		} else if (status == NetworkBehaviour.RoomStatus.RoomNotExists) {   // 房間不存在 (JoinRoom)
			Debug.LogError("房間不存在，請重新輸入");

		} else if (status == NetworkBehaviour.RoomStatus.Others) {
			Debug.LogError("這個房間已經開始遊戲");

		} else {                                                            // 未知錯誤
			Debug.LogError("錯誤");
		}

	}

	void ListChangedCallback(PlayerInfos pi, int index) {
		if (pi != null) {										// 玩家加入
			playersInRoom.Add(pi);

			if (resWeight.ContainsKey(pi.foodSelected)) {		// 如果有人和他選一樣的餐廳
				resWeight[pi.foodSelected] += 1;
			} else {
				colorResName.Add(pi.foodSelected);
				resWeight.Add(pi.foodSelected, 1);
			}

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


		}

		// For Test
		roomTest = playersInRoom;
}

	void ReadyCallback(int playerIndex) {
		Debug.Log("Ready: " + playerIndex);
	}

	void StartGameCallback() {
		SceneManager.LoadScene("GameMain");
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
				client.ConnectToServer(ServerIP, port);
				client.OnJoinedRoom = JoinRoomCallback;
				client.OnPlayerListChanged = ListChangedCallback;
				client.OnReadyCallback = ReadyCallback;
				client.OnStartGame = StartGameCallback;
			} catch {
				Debug.LogError("伺服器未開啟");
				return false;
			}
		}
		return true;
	}

	private void OnApplicationQuit() {
		if (client != null) client.ApplicationQuit();
		print("Quit");
	}

	void Update() {
		// 開房
		if (Input.GetKeyDown(KeyCode.D)) {
			CreateRoom();
		}
		// 入房
		if (Input.GetKeyDown(KeyCode.F)) {
			JoinRoom();
		}
		// Go
		if (Input.GetKeyDown(KeyCode.A)) {
			GoButton();
		}
		// Ready
		if (Input.GetKeyDown(KeyCode.S)) {
			ReadyButton();
		}
		// 返回 (離開房間)
		if (Input.GetKeyDown(KeyCode.G)) {
			LeaveRoom();
		}
		// 開始遊戲
		if (Input.GetKeyDown(KeyCode.H)) {
			StartGameButton();
		}

	}
}
