using UnityEngine.UI;
using UnityEngine;

public class WaitRoomPlayer : MonoBehaviour {

	public Image image;
	public Text nickNameText, resText;
	public GameObject readyUI;

	public void FeedColor(int colorIndex) {
		if (image != null) {
			image.color = UIRoomManager.gData.colors[colorIndex];
		}
	}
	
}
