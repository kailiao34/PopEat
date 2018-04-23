using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour {
	public int colorIndex;
	public bool Clickbool;
	[SerializeField]
	MeshRenderer ren;

	void Start() {
		colorIndex = GameManager.colorPicker.Get();
		if (ren != null) {
			ren.material.color = UIRoomManager.colorList[colorIndex];
		}
	}


}
