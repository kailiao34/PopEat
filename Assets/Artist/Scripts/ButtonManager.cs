using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour {

	public static ButtonManager ins;
	public Animator UIswitcher;

	public Animator loadingIndicatorAnimator;           //呼叫 Loading 介面

	public Text roomNameText, waitRoomDisplayNameText;
	string currentText;
	[SerializeField]
	GameObject resOK, createRoomOK, enterRoomOK;

	public Button[] buttons;
	public UIRoomManager roomManager;
	bool CreateOrJoinRoom;          // True: Create, False: Join
	[SerializeField]
	Image soundButton;
	[SerializeField]
	Sprite soundOnImage, soundOffImage;

	private void Awake() {
		ins = this;
	}

	private void Start() {
		UIswitcher.SetBool("goToGame", false);

		string m = PlayerPrefs.GetString("Mute");
		if (m == "M") {         // 靜音
			MuteButton();
		}
	}

	public void MuteButton() {
		AudioManager.Mute();
		if (AudioManager.muted) {
			soundButton.sprite = soundOffImage;
			PlayerPrefs.SetString("Mute", "M");
		} else {
			soundButton.sprite = soundOnImage;
			PlayerPrefs.SetString("Mute", "");
		}
	}

	//private void Update() {
	//	if (Input.GetKeyDown(KeyCode.A)) {
	//		buttonEat();
	//	}
	//}
	/// <summary>
	/// EnterRoom or CreateRoom Callback
	/// True: Create Room, False: Enter Room
	/// </summary>
	public void EnterOrCreateRoomOK(bool createOrEnter) {
		createRoomOK.SetActive(createOrEnter);
		enterRoomOK.SetActive(!createOrEnter);
	}

	public void TurnOffRoomOKs() {
		createRoomOK.SetActive(false);
		enterRoomOK.SetActive(false);
	}

	public void ResSelecteOK() {
		resOK.SetActive(UIRoomManager.myInfos.foodSelected != "");
	}

	//========呼叫暱稱輸入介面=========
	public void buttonNick() {
		UIswitcher.SetBool("nick", !UIswitcher.GetBool("nick"));
		Sounds.PlayButton();
	}

	//=======呼叫餐廳列表介面==========
	public void buttonEat() {
		if (UIswitcher.GetBool("eat")) {
			UIswitcher.SetBool("eat", false);
			loadingIndicatorAnimator.SetBool("Enabled", false); // used for loading animator
		} else {
			UIswitcher.SetBool("eat", true);
		}
		ResSelecteOK();
		Sounds.PlayButton();
	}

	//=======呼叫進入/創立房間介面==========

	// 雖然共用，但創房 和 進房 還是需要分別 (color)

	public void buttonCer(bool isCreate) {
		if (UIRoomManager.roomWaitForServer) {      // 正在等待伺服器回傳房間狀態
			LogUI.Show("正在等待伺服器回應...");
		} else {
		CreateOrJoinRoom = isCreate;
		ShowCreateOrJoinRoomUI();
		}
		Sounds.PlayButton();
	}

	public void ButtonCreateOrJoinRoomOkay() {
		if (CreateOrJoinRoom) {
			roomManager.CreateRoom(roomNameText.text);
		} else {
			roomManager.JoinRoom(roomNameText.text);
		}
		ShowCreateOrJoinRoomUI();
		Sounds.PlayButton();
	}

	void ShowCreateOrJoinRoomUI() {
		if (UIswitcher.GetBool("cer")) {
			UIswitcher.SetBool("cer", false);
		} else {
			if (CreateOrJoinRoom) {
				UIswitcher.SetInteger("cerF", 1);
			} else {
				UIswitcher.SetInteger("cerF", 0);
			}
			UIswitcher.SetBool("cer", true);
		}
	}

	//=======呼叫等候室介面==========

	public void buttonWait() {
		Button buttonAllGo = buttons[0];

		//need Escape Yes No UI - Posponed
		if (buttonAllGo.isActiveAndEnabled) {
			UIswitcher.SetBool("wait", !UIswitcher.GetBool("wait"));
		}

		waitRoomDisplayNameText.text = UIRoomManager.myInfos.roomName;
		Sounds.PlayButton();
	}

	//=======呼叫 Credits 介面==========
	public void CreditList() {
		UIswitcher.SetBool("credits", !UIswitcher.GetBool("credits"));
		Sounds.PlayButton();
	}

	public void ChangeButtonText(Text buttonText) {
		buttonText.text = currentText;
	}
	public void CurrentText(Text text) {
		currentText = text.text;
	}
}
