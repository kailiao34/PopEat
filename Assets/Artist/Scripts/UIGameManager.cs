using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIGameManager : MonoBehaviour {

    public static UIGameManager ins;
    public static Animator UIgame1; //呼叫 Game Result 介面
    public Animator UIgame;

    public static Animator errorUI1; //呼叫 錯誤訊息 介面
    public Animator errorUI;

    public static Animator loadingUI1; //呼叫 Loading 介面
    public Animator loadingUI;

    public Button goToMenu;
    public Text errorText; //用來顯示 錯誤訊息的 UI Text;
    public Button error;
    public Button errorOverlay;

	[SerializeField]
	Text winnerLocalText, winnerGlobalText;
	[SerializeField]
	Image winnerLocalHex, winnerGlobalHex;

    private void Awake()  {
        UIgame1 = UIgame;
        errorUI1 = errorUI;
        loadingUI1 = loadingUI;
        ins = this;
    }

	public void SetLocalWinner(string resName, int colorIndex) {
		winnerLocalText.text = resName;
		winnerLocalHex.color = UIRoomManager.colorList[colorIndex];
	}

	public void SetGlobalWinner(string resName, int colorIndex) {
		winnerGlobalText.text = resName;
		winnerGlobalHex.color = UIRoomManager.colorList[colorIndex];
	}

    //======呼叫遊戲結果介面=======
    public void GameResultUI() {
		

        if (UIgame1.GetBool("result") == false)
        {
            UIgame1.SetBool("result", true);
        }
        else if (UIgame1.GetBool("result") == true)
        {
            UIgame1.SetBool("result", false);
            Invoke("SwitchScene", 1);            //用在 回主選單 button 上
        }
    }

    void SwitchScene()
    {
        SceneManager.LoadScene("Hall");
    }

    //======呼叫 Loading 顯示=======
    public void LoadingUI() {

    }


    //======呼叫錯誤訊息介面=======
    public void ErrorMsgGame()
    {
        if (errorUI1.GetBool("error") == false)
        {
            errorUI1.SetBool("error", true);
            //errorText.text = "想寫啥就換成啥";         //錯誤訊息由這邊置換
        }
        else if (errorUI1.GetBool("error") == true)
        {
            errorUI1.SetBool("error", false);
            errorText.text = "";                        //關閉時順便清除錯誤訊息
        }
        else print("這不該發生! error");
    }
}
