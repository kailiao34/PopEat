using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
	public int colorIndex;
	public bool Clickbool;
	[SerializeField]
	MeshRenderer ren;

    void Start()
    {
		colorIndex = Random.Range(0, UIRoomManager.resWeight.Count);
		Color colorA = UIRoomManager.colorList[colorIndex];
		if (ren != null)
        {
			ren.material.color = colorA;
		}
        else Debug.Log("error no mesh or iamge renderer");
    }

	
}
