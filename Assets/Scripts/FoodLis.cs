using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodLis : MonoBehaviour {
	public static List<string> resList;
	public static FoodLis ins;

	public static Text[] resInfoText;
	public Text[] resInfosUI;
	[SerializeField]
	Button InputFoodButtonIns;
	static Button InputFoodButton;

	[SerializeField]
	InputField FoodInputField;
	[SerializeField]
	Transform buttonsParent;
	[SerializeField]
	GameObject resButtonPrefab;
	[SerializeField]
	FoodListButton EnterFoodListButton;
	[SerializeField]
	Animator EnterFoodListButtonAnimator;
	[SerializeField]
	Animator loadingIndicatorAnimator;

	// 餐廳按鈕正常的顏色
	public static ColorBlock resNormalCB;
	//餐廳按鈕被按下時改成這個顏色
	public static ColorBlock resHCB;
	// 記錄上一個被選到的餐廳按鈕
	public static int preResSelected;			// 這個值如果是 -1 代表是自行輸入的餐廳
	static List<Button> resButtons;

	private void Awake() {
		ins = this;
		resInfoText = resInfosUI;

		resNormalCB = resButtonPrefab.GetComponent<Button>().colors;
		resHCB = resNormalCB;
		resHCB.normalColor = Generic.gData.resHighlighted;
		resHCB.highlightedColor = Generic.gData.resHighlighted;
		InputFoodButton = InputFoodButtonIns;

	}

	private void Start() {
		if (resList == null || resList.Count == 0) {
			GetResNames();
		} else {
			UISort();
			if (preResSelected < 0) {
				EnterFoodListButtonAnimator.SetBool("switch", true);
				InputFoodButton.GetComponent<FoodListButton>().UItext.text = UIRoomManager.myInfos.foodSelected;
				preResSelected = 0;
				HighlightButton(-1);
			} else {
				int pre = preResSelected;
				preResSelected = -2;
				HighlightButton(pre);
			}
		}
	}

	public void GetResNames() {
		loadingIndicatorAnimator.SetBool("Enabled", true); // used for loading animator
		GetRes.ins.GetResNames(Generic.gData.radius, GetResNames);
	}

	void GetResNames(List<string> resNames) {
		resList = resNames;
		UISort();
	}

	void UISort() {
		resButtons = new List<Button>();

		loadingIndicatorAnimator.SetBool("Enabled", false); // used for loading animator
		for (int i = 0; i < resList.Count; i++) {
			GameObject newButton = Instantiate(resButtonPrefab, buttonsParent);
			FoodListButton f = newButton.GetComponent<FoodListButton>();

			f.UItext.text = resList[i];
			f.buttonIndex = i;
			resButtons.Add(newButton.GetComponent<Button>());
		}
	}

	public static void HighlightButton(int buttonIndex) {
		if (buttonIndex == preResSelected) return;

		if (preResSelected < 0) {                                   // 上一個選到的按鈕顏色改回正常
			InputFoodButton.colors = resNormalCB;
		} else {
			resButtons[preResSelected].colors = resNormalCB;
		}

		if (buttonIndex < 0) {                                  // 新選到的改顏色
			InputFoodButton.colors = resHCB;                        // 選到是使用者自行輸入的
		} else {
			resButtons[buttonIndex].colors = resHCB;
		}
		preResSelected = buttonIndex;
	}

	private void OnEnable() {
		OnValueChanged();
	}

	public void OnValueChanged() {
		EnterFoodListButtonAnimator.SetBool("switch", FoodInputField.text != "");
		EnterFoodListButton.UItext.text = FoodInputField.text;
	}
}
