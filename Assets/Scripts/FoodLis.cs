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
    Animator loadingIndicatorAnimator;

    //public static Animator UIswitcher1;
    //public Animator UIswitcher;

	private void Awake() {
		resInfos = resInfosUI;
        loadingIndicatorAnimator = ButtonManager.loadingIndicatorAnimator1;
        //UIswitcher1 = UIswitcher;
    }

	private void Start()
	{
        loadingIndicatorAnimator.SetBool("Enabled", true); // used for loading animator
        GetRes.ins.GetAllRes(24.99579212, 121.48876185, 500, GetResNames);
    }

    void GetResNames(List<Details> resDetails)
	{
		resList = resDetails;
		UISort();
        loadingIndicatorAnimator.SetBool("Enabled", false); // used for loading animator
    }
	
    public void UISort()
    {
        for (int i = 0; i < resList.Count; i++) {
			GameObject G1 = Instantiate(foodListButton, gameObject.transform.position, 
			Quaternion.identity, FoodListButtonParentobject.transform);
			RectTransform r = G1.GetComponent<RectTransform>();
			FoodListButton f = G1.GetComponent<FoodListButton>();

            f.UItext.text = resList[i].name;
			f.resIndex = i;
            
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
