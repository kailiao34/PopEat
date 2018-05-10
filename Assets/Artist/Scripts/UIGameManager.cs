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
}
