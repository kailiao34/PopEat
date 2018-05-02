using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodLis : MonoBehaviour {
	public static List<string> resList;

	public static Text[] resInfoText;
	public Text[] resInfosUI;

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
	Animator loadingIndicatorAnimator;
	// 餐廳按鈕正常的顏色
	public static ColorBlock resNormalCB;
	//餐廳按鈕被按下時改成這個顏色
	public static ColorBlock resHCB;
	// 記錄上一個被選到的餐廳按鈕
	public static int preResSelected;
	static List<Button> resButtons;

	private void Awake() {
		resInfoText = resInfosUI;
		loadingIndicatorAnimator = ButtonManager.loadingIndicatorAnimator1;

		resNormalCB = resButtonPrefab.GetComponent<Button>().colors;
		resHCB = resNormalCB;
		resHCB.normalColor = UIRoomManager.gData.resHighlighted;
		resHCB.highlightedColor = UIRoomManager.gData.resHighlighted;

	}

	private void Start() {
		loadingIndicatorAnimator.SetBool("Enabled", true); // used for loading animator

		if (resList == null || resList.Count == 0) {
			GetRes.ins.GetResNames(24.99579212, 121.48876185, UIRoomManager.gData.radius, GetResNames);
		} else {
			UISort();
			HighlightButton(preResSelected);
		}
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
		resButtons[preResSelected].colors = resNormalCB;			// 上一個選到的按鈕顏色改回正常
		resButtons[buttonIndex].colors = resHCB;                    // 新選到的改顏色
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
