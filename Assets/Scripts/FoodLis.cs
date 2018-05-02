using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodLis : MonoBehaviour {
	public static List<string> resList;

	public static Text[] resInfoText;
	public Text[] resInfosUI;

	public ScrollRect reviewListUI;

	public InputField FoodInputField;
	public GameObject FoodListUI_ScrollView_Content, FoodListButtonParentobject;
	public GameObject foodListButton;
	public FoodListButton EnterFoodListButton;
	[SerializeField]
	Animator EnterFoodListButtonAnimator;
	List<RectTransform> foodListRectTransform = new List<RectTransform>();
	List<FoodListButton> FoodListButtonscript = new List<FoodListButton>();
	public RectTransform eatCanvas;
	Animator loadingIndicatorAnimator;
	// 餐廳按鈕正常的顏色
	public static ColorBlock resNormalCB;
	//餐廳按鈕被按下時改成這個顏色
	public static ColorBlock resHCB;
	// 記錄上一個被選到的餐廳按鈕
	public static Button preResSelected;

	private void Awake() {
		resInfoText = resInfosUI;
		loadingIndicatorAnimator = ButtonManager.loadingIndicatorAnimator1;

		resNormalCB = foodListButton.GetComponent<Button>().colors;
		resHCB = resNormalCB;
		resHCB.normalColor = UIRoomManager.gData.resHighlighted;
		resHCB.highlightedColor = UIRoomManager.gData.resHighlighted;

	}

	private void Start() {
		loadingIndicatorAnimator.SetBool("Enabled", true); // used for loading animator
		GetRes.ins.GetResNames(24.99579212, 121.48876185, UIRoomManager.gData.radius, GetResNames);
	}

	void GetResNames(List<string> resNames) {
		resList = resNames;
		UISort();
		loadingIndicatorAnimator.SetBool("Enabled", false); // used for loading animator
	}

	public void UISort() {
		for (int i = 0; i < resList.Count; i++) {
			GameObject G1 = Instantiate(foodListButton, gameObject.transform.position,
			Quaternion.identity, FoodListButtonParentobject.transform);
			RectTransform r = G1.GetComponent<RectTransform>();
			FoodListButton f = G1.GetComponent<FoodListButton>();

			f.UItext.text = resList[i];

			foodListRectTransform.Add(r);
			FoodListButtonscript.Add(f);
		}
	}

	private void OnEnable() {
		OnValueChanged();
	}

	public void OnValueChanged() {
		EnterFoodListButtonAnimator.SetBool("switch", FoodInputField.text != "");
		EnterFoodListButton.UItext.text = FoodInputField.text;
	}
}
