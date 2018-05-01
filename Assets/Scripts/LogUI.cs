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

	public static void Show(string msg) {
		text.text = msg;
		anim.SetBool(hash, true);
	}

	public static void Hide() {
		anim.SetBool(hash, false);
	}
}
