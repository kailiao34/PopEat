using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodLis : MonoBehaviour
{
    public InputField FoodInputField;
    public GameObject FoodListUI_ScrollView_Content, FoodListButtonParentobject;
    public GameObject foodListButton;
    public FoodListButton EnterFoodListButton;
    public Animator EnterFoodListButtonAnimator;
    public List<string> list;
    public List<RectTransform> foodListRectTransform;
    public List<FoodListButton> FoodListButtonscript;
	// Use this for initialization

	private void Start()
	{
		GetRes.ins.Get(24.99579212, 121.48876185, 500, GetResNames);
	}

	void GetResNames(string[] resNames)
	{
		foreach (string s in resNames)
		{
			list.Add(s);
		}

		Getlist();
	}

	public void Getlist()
    {

		for (int i = 0; i < list.Count; i++)
        {
			print(list[i]);
            UISort(i);
        }
    }

    public void UISort(int i)
    {
        GameObject G1 = Instantiate(foodListButton, gameObject.transform.position, Quaternion.identity);
        G1.transform.parent = FoodListUI_ScrollView_Content.transform;
        foodListRectTransform.Add(G1.GetComponent<RectTransform>());
        FoodListButtonscript.Add(G1.GetComponent<FoodListButton>());
        FoodListButtonscript[i].UItext.text = list[i];
        FoodListButtonscript[i].foodLis = this;
        foodListRectTransform[i].transform.parent = FoodListButtonParentobject.transform;
        if (i == 0)
        {
            foodListRectTransform[i].anchoredPosition = new Vector2(7.47F, -230F);
            foodListRectTransform[i].localScale = new Vector3(2, 2, 1);

        }
        if (i >= 1)
        {
            foodListRectTransform[i].anchoredPosition = new Vector2(foodListRectTransform[0].anchoredPosition.x, foodListRectTransform[0].anchoredPosition.y + (i * -120));
            foodListRectTransform[i].localScale = new Vector3(2, 2, 1);

        }
        if (i >= 11)
        {
            RectTransform r1 = FoodListUI_ScrollView_Content.GetComponent<RectTransform>();
            r1.sizeDelta = new Vector2(r1.rect.width, r1.rect.height + (120));
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
    public void aa2()
    {
        //Debug.Log("正在輸入");
    }
}
