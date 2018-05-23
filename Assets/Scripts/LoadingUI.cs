using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : MonoBehaviour {

	[SerializeField]
	Animator loadingIndicatorAnimator;
	static Animator loadingAnim;
	static bool onOff;

	private void Awake() {
		loadingAnim = loadingIndicatorAnimator;
	}

	private void OnEnable() {
		loadingIndicatorAnimator.SetBool("Enabled", onOff);
	}
	

	public static void Show() {
		onOff = true;
		if (loadingAnim != null && loadingAnim.gameObject.activeInHierarchy) loadingAnim.SetBool("Enabled", onOff);
	}

	public static void Hide() {
		onOff = false;
		if (loadingAnim != null && loadingAnim.gameObject.activeInHierarchy) loadingAnim.SetBool("Enabled", onOff);
	}

}
