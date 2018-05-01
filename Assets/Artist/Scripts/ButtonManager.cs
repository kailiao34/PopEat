using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour {

	public static ButtonManager ins;
	public static Animator UIswitcher1; //控制主選單介面切換
	public Animator UIswitcher;
	//<<<<<<< HEAD


	public static Animator loadingIndicatorAnimator1; //呼叫 Loading 介面
	public Animator loadingIndicatorAnimator;

	public Text roomNameText, waitRoomDisplayNameText;
	//=======
	string currentText;
	[SerializeField]
	GameObject resOK, createRoomOK, enterRoomOK;
	//>>>>>>> 1114a1a7147f193846bb5505d0b03d508c517292

	public Button[] buttons;
	public UIRoomManager roomManager;
	bool CreateOrJoinRoom;          // True: Create, False: Join

	private void Awake() {
		UIswitcher1 = UIswitcher;
		loadingIndicatorAnimator1 = loadingIndicatorAnimator;
		ins = this;
	}

	private void Start() {
		UIswitcher1.SetBool("goToGame", false);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.A)) {
			buttonEat();
		}
	}
	/// <summary>
	/// True: Create Room, False: Enter Room
	/// </summary>
	public void EnterRoomCallback(bool createOrEnter) {
		createRoomOK.SetActive(createOrEnter);
		enterRoomOK.SetActive(!createOrEnter);
	}

	public void TurnOffRoomOKs() {
		createRoomOK.SetActive(false);
		enterRoomOK.SetActive(false);
	}

	//========呼叫暱稱輸入介面=========
	public void buttonNick() {
		UIswitcher1.SetBool("nick", !UIswitcher1.GetBool("nick"));
	}

	//=======呼叫餐廳列表介面==========
	public void buttonEat() {
		if (UIswitcher1.GetBool("eat")) {
			UIswitcher1.SetBool("eat", false);
			loadingIndicatorAnimator1.SetBool("Enabled", false); // used for loading animator
		} else {
			UIswitcher1.SetBool("eat", true);
		}
		resOK.SetActive(UIRoomManager.myInfos.foodSelected != "");
	}

	//=======呼叫進入/創立房間介面==========

	// 雖然共用，但創房 和 進房 還是需要分別 (color)

	public void buttonCer(bool isCreate) {
		CreateOrJoinRoom = isCreate;
		ShowCreateOrJoinRoomUI();
	}

	public void ButtonCreateOrJoinRoomOkay() {
		if (CreateOrJoinRoom) {
			roomManager.CreateRoom(roomNameText.text);
		} else {
			roomManager.JoinRoom(roomNameText.text);
		}
		ShowCreateOrJoinRoomUI();
	}

	void ShowCreateOrJoinRoomUI() {
		if (UIswitcher1.GetBool("cer")) {
			UIswitcher1.SetBool("cer", false);
		} else {
			if (CreateOrJoinRoom) {
				UIswitcher1.SetInteger("cerF", 1);
			} else {
				UIswitcher1.SetInteger("cerF", 0);
			}
			UIswitcher1.SetBool("cer", true);
		}
	}

	//=======呼叫等候室介面==========

	public void buttonWait() {
		Button buttonAllGo = buttons[0];
		Button buttonEscape = buttons[5];

		//need Escape Yes No UI - Posponed
		if (buttonAllGo.isActiveAndEnabled) {
			UIswitcher1.SetBool("wait", !UIswitcher1.GetBool("wait"));
		}

		waitRoomDisplayNameText.text = UIRoomManager.myInfos.roomName;
	}

	//=======呼叫 Credits 介面==========
	public void CreditList() {
		UIswitcher1.SetBool("credits", !UIswitcher1.GetBool("credits"));
	}

	public void ChangeButtonText(Text buttonText) {
		buttonText.text = currentText;
	}
	public void CurrentText(Text text) {
		currentText = text.text;
	}
}
