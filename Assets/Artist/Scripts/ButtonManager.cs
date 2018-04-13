using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

    public static Animator UIswitcher1;
    public Animator UIswitcher;

    public Button[] buttons;

    private void Awake()
    {
        UIswitcher1 = UIswitcher;
    }

    void Start () {
        
	}
	
	void Update () {
		
	}
    //========呼叫暱稱輸入介面=========
    public void buttonNick()
    {
        if (UIswitcher1.GetBool("nick") == false)
        {
            UIswitcher1.SetBool("nick", true);
        }
        else if (UIswitcher1.GetBool("nick") == true)
        {
            UIswitcher1.SetBool("nick", false);
        }
        else print("這不該發生!");
    }

    //=======呼叫餐廳列表介面==========
    public void buttonEat()
    {
        if (UIswitcher1.GetBool("eat") == false)
        {
            UIswitcher1.SetBool("eat", true);
            //buttons[1].enabled = false;
        }
        else if (UIswitcher1.GetBool("eat") == true)
        {
            UIswitcher1.SetBool("eat", false);
            //buttons[1].enabled = true;
        }
        else print("這不該發生!");
    }

    //=======呼叫進入/創立房間介面==========

    public void buttonCer()
    {
        if (UIswitcher1.GetBool("cer") == false)
        {
            UIswitcher1.SetBool("cer", true);
        }
        else if (UIswitcher1.GetBool("cer") == true)
        {
            UIswitcher1.SetBool("cer", false);
        }
        else print("這不該發生!");
    }

    //=======呼叫等候室介面==========

    public void buttonWait()
    {
        Button buttonAllGo = buttons[0];
        //Button buttonEscape = buttons[5];

        //need Escape Yes No UI

        if (UIswitcher1.GetBool("wait") == false && buttonAllGo.isActiveAndEnabled)
        {
            UIswitcher1.SetBool("wait", true);
        }
        else if (UIswitcher1.GetBool("wait") == true && /*buttonEscape.isActiveAndEnabled*/)
        {
            UIswitcher1.SetBool("wait", false);
        }
        else print("這不該發生!");
    }

    /*public void buttonNick()
    {
        EnterFoodListButtonAnimator.SetBool("switch", true);
        if (FoodInputField.text == "")
        {
            EnterFoodListButtonAnimator.SetBool("switch", false);
            Debug.Log("目前沒有文字");
        }
        EnterFoodListButton.UItext.text = FoodInputField.text;
    }*/
}
