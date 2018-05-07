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

	private void Awake() {
		ins = this;
	}

	private void Start() {
		UIswitcher.SetBool("goToGame", false);
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
	}

	//=======呼叫 Credits 介面==========
	public void CreditList() {
		UIswitcher.SetBool("credits", !UIswitcher.GetBool("credits"));
	}

	public void ChangeButtonText(Text buttonText) {
		buttonText.text = currentText;
	}
	public void CurrentText(Text text) {
		currentText = text.text;
	}
}
