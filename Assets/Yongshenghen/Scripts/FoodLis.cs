using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodLis : MonoBehaviour
{
	public static List<Details> resList;
	public static Text[] resInfos;

	public Text[] resInfosUI;
	public ScrollRect reviewListUI;

    public InputField FoodInputField;
    public GameObject FoodListUI_ScrollView_Content, FoodListButtonParentobject;
    public GameObject foodListButton;
    public FoodListButton EnterFoodListButton;
    public Animator EnterFoodListButtonAnimator;
    public List<RectTransform> foodListRectTransform;
    public List<FoodListButton> FoodListButtonscript;
    public RectTransform eatCanvas;

    public static Animator UIswitcher1;
    public Animator UIswitcher;

	private void Awake() {
		resInfos = resInfosUI;
        UIswitcher1 = UIswitcher;
    }

	private void Start()
	{
        GetRes.ins.GetAllRes(24.99579212, 121.48876185, 500, GetResNames);
        //GetRes.ins.GetAllRes(25.105493, 121.530091, 500, GetResNames);
        //GetRes.ins.GetAllRes(25.033773, 121.564358, 500, GetResNames);
    }

	void GetResNames(List<Details> resDetails)
	{
		resList = resDetails;
		UISort();
    }
	
    public void UISort()
    {
        for (int i = 0; i < resList.Count; i++) {
			GameObject G1 = Instantiate(foodListButton, gameObject.transform.position, 
			Quaternion.identity, FoodListButtonParentobject.transform);
			RectTransform r = G1.GetComponent<RectTransform>();
			FoodListButton f = G1.GetComponent<FoodListButton>();
            //float lSpac = FoodListButtonParentobject.GetComponent<VerticalLayoutGroup>().spacing;

            f.UItext.text = resList[i].name;
			f.resIndex = i;

			/*if (i == 0) {
				r.anchoredPosition = new Vector2(7.47F, -140F);
				//r.localScale = new Vector3(2, 2, 1);

			}
			if (i >= 1) {
				r.anchoredPosition = new Vector2(foodListRectTransform[0].anchoredPosition.x, foodListRectTransform[0].anchoredPosition.y + (i * -120));
				//r.localScale = new Vector3(2, 2, 1);

			}
			if (i >= 11) {
				RectTransform r1 = FoodListUI_ScrollView_Content.GetComponent<RectTransform>();
				r1.sizeDelta = new Vector2(483 , lSpac*i);
			}*/
            
			foodListRectTransform.Add(r);
			FoodListButtonscript.Add(f);
		}
    }

    public void aa1()
    {
        Debug.Log("正在輸入");
        EnterFoodListButtonAnimator.SetBool("switch", true);
        if (FoodInputField.text == "")
        {
            EnterFoodListButtonAnimator.SetBool("switch", false);
            Debug.Log("目前沒有文字");
        }
        EnterFoodListButton.UItext.text = FoodInputField.text;
    }
}
