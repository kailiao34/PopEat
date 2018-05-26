using UnityEngine.UI;
using UnityEngine;

public class LogUI : MonoBehaviour {
	static Text text;
	static Animator anim;
	static int hash = Animator.StringToHash("error");

	private void Awake() {
		text = GetComponentInChildren<Text>();
		anim = GetComponent<Animator>();
	}

	public static void Show(string msg, float sec = 1) {
		Ticker.StartTicker(0, ()=> {
			text.text = msg;
			anim.SetBool(hash, true);
		});
		Ticker.StartTicker(sec, Hide);
		Ticker.StartTicker(0, Sounds.PlayExit);
	}

	public static void Hide() {
		anim.SetBool(hash, false);
	}
}
