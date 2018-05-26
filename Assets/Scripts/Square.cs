using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour {
	public int colorIndex;
	public bool Clickbool;
	[SerializeField]
	MeshRenderer ren;

	void Start() {
		colorIndex = UIRoomManager.colorPicker.Get();

		// ****************** Test ******************
#if UNITY_EDITOR
		if (colorIndex == -1) {
			colorIndex = Random.Range(0, 3);
		}
#endif
		// ******************************************

		if (ren != null) {
			ren.material.color = Generic.gData.colors[colorIndex];
		}
	}


}
