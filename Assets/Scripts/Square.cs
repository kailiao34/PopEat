using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour {
	public int colorIndex;
	public bool Clickbool;
	[SerializeField]
	MeshRenderer ren;

	void Start() {
		// ****************** Test ******************
		//colorIndex = SquareColorTest.p.Get();
		//ren.material.color = SquareColorTest.colors[colorIndex];
		// ******************************************

		colorIndex = UIRoomManager.colorPicker.Get();
		if (ren != null) {
			ren.material.color = UIRoomManager.gData.colors[colorIndex];
		}
	}


}
