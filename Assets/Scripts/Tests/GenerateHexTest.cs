using System.Collections.Generic;
using UnityEngine;

public class GenerateHexTest : MonoBehaviour {

	public LineRenderer L1;
	int I1;
	List<Square> Inseries = new List<Square>() { null };
	bool Startbool = true;
	Square selected, preSelected;
	// For Hex Arrangement
	[SerializeField]
	GameObject squarePrefab, hexFXPrefab;
	HashSet<GameObject> squares = new HashSet<GameObject>();

	private void Start() {
		ArrangeHex();
	}

	void Update() {

		if (Input.GetMouseButton(0)) {           // 按住，連連看
			if (Startbool) eliminateSquare();
		}
		if (Input.GetMouseButtonUp(0)) {         // 放開，消除方塊
			Release();
			Startbool = true;
		}
	}
	void eliminateSquare() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 100.0f)) {
			if (hit.collider.gameObject.tag == "Square") {
				selected = hit.collider.gameObject.GetComponent<Square>();
				Colourdetermination();
			} else if (hit.collider.gameObject.tag == "floor") {
				Release();
			}
		}
		//Debug.DrawLine(ray.origin, hit.point, Color.green);
	}
	void Colourdetermination() {
		if (Inseries[0] == null) {
			L1.positionCount += 1;
			I1 += 1;
			Inseries[0] = selected;
			L1.SetPosition(0, new Vector3(selected.transform.position.x, selected.transform.position.y + 1f, selected.transform.position.z));
		} else {
			if (selected != preSelected && selected.Clickbool) {
				Release();
				return;
			}
			if (selected.Clickbool == false && selected != Inseries[0]) {
				if (selected.colorIndex != Inseries[0].colorIndex) {
					Release();
					return;
				} else if (selected.colorIndex == Inseries[0].colorIndex) {
					Inseries.Add(selected);
					L1.positionCount += 1;
					L1.SetPosition(Inseries.Count - 1, new Vector3(selected.transform.position.x, selected.transform.position.y + 1f, selected.transform.position.z));
				}
			}
		}
		selected.Clickbool = true;
		preSelected = selected;
	}

	void Release() {
		if (Inseries[0] != null) {
			for (int i = 0; i < Inseries.Count; i++) {
				squares.Remove(Inseries[i].gameObject);
				Instantiate(hexFXPrefab, Inseries[i].transform.position, Quaternion.identity);      // 爆破效果

				Destroy(Inseries[i].gameObject);
			}
			Inseries.Clear();
			Inseries.Add(null);
			L1.positionCount = 0;
			I1 = 0;
			Startbool = false;
		}
	}

	void ArrangeHex() {
		squares.Add(Instantiate(squarePrefab));

		for (int j = 0; j < 6; j++) {
			GameObject tt1 = new GameObject();
			tt1.gameObject.name = "j=====>" + j;
			for (int i = 1; i < Generic.gData.hexLayers; i++) {
				float z = i * 0.84f;
				for (int k = 1; k < i + 1; k++) {
					GameObject gg2 = Instantiate(squarePrefab);
					squares.Add(gg2);
					gg2.transform.position = new Vector3(k * 0.752f, 0, z - k * 0.422f);
					gg2.transform.parent = tt1.transform;
					gg2.gameObject.name = "k=====>" + k;
				}
			}
			if (j >= 1) {
				tt1.transform.eulerAngles = new Vector3(0, j * 60, 0);
			}
		}
		//print("六角數量: " + squares.Count);
	}
}
