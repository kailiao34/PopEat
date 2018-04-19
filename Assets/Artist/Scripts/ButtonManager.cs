using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour {

    public static Animator UIswitcher1;
    public Animator UIswitcher;

    public Button[] buttons;
    //public Sprite[] spriteImages;
    public Text errorText; // 用來顯示 錯誤訊息的 UI Text;

    private void Awake()
    {
        UIswitcher1 = UIswitcher;
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
        else print("這不該發生! Nick");
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
        else print("這不該發生! Eat");
    }

    //=======呼叫進入/創立房間介面==========

    // 雖然共用，但創房 和 進房 還是需要分別 (color)

    public void buttonCer()
    {
        Button buttonCreateR = buttons[1];
        Button buttonEnterR = buttons[4];

        if (UIswitcher1.GetBool("cer") == false)
        {
            //BR - EventSystem 用來判定是創房 還是 進房。 cerF 1 = 創房按鈕 / 0 = 進房按鈕;
            if (EventSystem.current.currentSelectedGameObject.name == buttonCreateR.name)
            {
                UIswitcher1.SetInteger("cerF", 1);
                //print("111");
            }
            else if (EventSystem.current.currentSelectedGameObject.name == buttonEnterR.name)
            {
                UIswitcher1.SetInteger("cerF", 0);
                //print("000");
            }
            else print("不是創房也非進房，這不該發生!");

            UIswitcher1.SetBool("cer", true);
        }
        else if (UIswitcher1.GetBool("cer") == true)
        {
            UIswitcher1.SetBool("cer", false);
        }
        else print("這不該發生! Cer");
    }

    //=======呼叫等候室介面==========

    public void buttonWait()
    {
        Button buttonAllGo = buttons[0];
        Button buttonEscape = buttons[5];        

        //need Escape Yes No UI - Posponed

        if (UIswitcher1.GetBool("wait") == false && buttonAllGo.isActiveAndEnabled)
        {
            UIswitcher1.SetBool("wait", true);
        }
        else if (UIswitcher1.GetBool("wait") == true && buttonEscape.isActiveAndEnabled)
        {
            UIswitcher1.SetBool("wait", false);
        }
        else print("這不該發生! Wait");
    }

    //=======呼叫錯誤提示介面==========
    public void ErrorMsg()
    {
        if (UIswitcher1.GetBool("error") == false)
        {
            UIswitcher1.SetBool("error", true);
            errorText.text = "想寫啥就換成啥";         //錯誤訊息由這邊置換
        }
        else if (UIswitcher1.GetBool("error") == true)
        {
            UIswitcher1.SetBool("error", false);
            errorText.text = "";            
        }
        else print("這不該發生! error");
    }

    //=======呼叫 Credits 介面==========
    public void CreditList()
    {
        if (UIswitcher1.GetBool("credits") == false)
        {
            UIswitcher1.SetBool("credits", true);            
        }
        else if (UIswitcher1.GetBool("credits") == true)
        {
            UIswitcher1.SetBool("credits", false);
        }
        else print("這不該發生! credits");
    }
}
