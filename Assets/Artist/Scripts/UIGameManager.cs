using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIGameManager : MonoBehaviour {

    public static UIGameManager ins;
    public static Animator UIgame1; //呼叫 Game Result 介面
    public Animator UIgame;

    public static Animator loadingUI1; //呼叫 Loading 介面
    public Animator loadingUI;

    public Animator ExitAppAnimator; //呼叫 退出程式 介面

    public Button goToMenu;
    public Button error;
    public Button errorOverlay;

	[SerializeField]
	Text winnerLocalText, winnerGlobalText;
	[SerializeField]
	Image winnerLocalHex, winnerGlobalHex;

    private void Awake()  {
        UIgame1 = UIgame;
        loadingUI1 = loadingUI;
        ins = this;
    }

	public void SetLocalWinner(string resName, int colorIndex) {
		winnerLocalText.text = resName;
		winnerLocalHex.color = Generic.gData.colors[colorIndex];
	}

	public void SetGlobalWinner(string resName, int colorIndex) {
		winnerGlobalText.text = resName;
		winnerGlobalHex.color = Generic.gData.colors[colorIndex];
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))     //按下 Android ← 呼叫退出退出程式介面
        {
            ExitAppUI();
        }
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

    //======呼叫 Exit APP 介面========
    public void ExitAppUI()
    {
        ExitAppAnimator.SetBool("exitApp", !ExitAppAnimator.GetBool("exitApp"));
    }

    //======退出遊戲指令=======
    public void ExitApp()
    {
        Application.Quit();
        print("exit confirmed");
    }
}
