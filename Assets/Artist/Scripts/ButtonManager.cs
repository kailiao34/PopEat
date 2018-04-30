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

    public static Animator errorUI1; //呼叫 錯誤訊息 介面
    public Animator errorUI;

    public static Animator loadingIndicatorAnimator1; //呼叫 Loading 介面
    public Animator loadingIndicatorAnimator;

    public Text roomNameText, waitRoomDisplayNameText;
//=======
	string currentText;
	public GameObject resOK, createRoomOK, enterRoomOK;
//>>>>>>> 1114a1a7147f193846bb5505d0b03d508c517292

	public Button[] buttons;
	//public Sprite[] spriteImages;
	public Text errorText; // 用來顯示 錯誤訊息的 UI Text;
	public UIRoomManager roomManager;
	bool CreateOrJoinRoom;			// True: Create, False: Join

	private void Awake() {
		UIswitcher1 = UIswitcher;
        errorUI1 = errorUI;
        loadingIndicatorAnimator1 = loadingIndicatorAnimator;
        ins = this;
	}

    private void Start()
    {
        UIswitcher1.SetBool("goToGame", false);
    }

	//private void Update() {
	//	if (Input.GetKeyDown(KeyCode.A)) {
	//		buttonEat();
	//	}
	//}
	/// <summary>
	/// True: Create Room, False: Enter Room
	/// </summary>
	public void EnterRoomCallback(bool createOrEnter) {
		createRoomOK.SetActive(createOrEnter);
		enterRoomOK.SetActive(!createOrEnter);
	}

	//========呼叫暱稱輸入介面=========
	public void buttonNick() {


		if (UIswitcher1.GetBool("nick") == false) {
			UIswitcher1.SetBool("nick", true);
		}
        else if (UIswitcher1.GetBool("nick") == true) {
			UIswitcher1.SetBool("nick", false);
		}
        else print("這不該發生! Nick");
	}

	//=======呼叫餐廳列表介面==========
	public void buttonEat() {
		if (UIswitcher1.GetBool("eat")) {
			UIswitcher1.SetBool("eat", false);
            loadingIndicatorAnimator1.SetBool("Enabled", false); // used for loading animator
            //print("eat disabled");
        }
        else {
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
			//print("Create");
			roomManager.CreateRoom(roomNameText.text);
		}
        else {
			//print("Join");
			roomManager.JoinRoom(roomNameText.text);
		}
		ShowCreateOrJoinRoomUI();
	}

	void ShowCreateOrJoinRoomUI() {
		if (UIswitcher1.GetBool("cer")) {
			UIswitcher1.SetBool("cer", false);
		}
        else {
			if (CreateOrJoinRoom) {
				UIswitcher1.SetInteger("cerF", 1);
				//print("Create");
			}
            else {
				//print("Join");
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

		if (UIswitcher1.GetBool("wait") == false && buttonAllGo.isActiveAndEnabled) {
			UIswitcher1.SetBool("wait", true);
		}
        else if (UIswitcher1.GetBool("wait") == true && buttonEscape.isActiveAndEnabled) {
			UIswitcher1.SetBool("wait", false);
		}
        else print("這不該發生! Wait");

		waitRoomDisplayNameText.text = UIRoomManager.myInfos.roomName;
	}

	//=======呼叫錯誤提示介面==========
	public void ErrorMsg() {
		if (errorUI1.GetBool("error") == false) {
			errorUI1.SetBool("error", true);
			//errorText.text = currentText;         //錯誤訊息由這邊置換
		}
        else if (errorUI1.GetBool("error") == true) {
            errorUI1.SetBool("error", false);
			errorText.text = "";                        //關閉時順便清除錯誤訊息
		}
        else print("這不該發生! error");
	}

	//=======呼叫 Credits 介面==========
	public void CreditList() {
		if (UIswitcher1.GetBool("credits") == false) {
			UIswitcher1.SetBool("credits", true);
		}
        else if (UIswitcher1.GetBool("credits") == true) {
			UIswitcher1.SetBool("credits", false);
		}
        else print("這不該發生! credits");
	}
    
	public void ChangeButtonText(Text buttonText) {
		buttonText.text = currentText;
	}
	public void CurrentText(Text text) {
		currentText = text.text;
	}
}
