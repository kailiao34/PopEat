using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour {

	public static void PlayButton() {
		AudioManager.PlayOneShot(Audios.Button2);
	}

	public static void PlayExit() {
		AudioManager.PlayOneShot(Audios.Msg2);
	}

	public static void PlayHex() {
		if (Random.Range(0, 2) == 0) {
			AudioManager.PlayOneShot(Audios.Hex2);
		} else {
			AudioManager.PlayOneShot(Audios.Hex3);
		}
	}

	public static void PlayHexMul() {
		AudioManager.PlayOneShot(Audios.HexMul, 25);
	}
}
