using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareManager : MonoBehaviour {
	public LineRenderer L1;
	public int I1;
	public List<Square> Inseries;
	public bool Startbool;
	Square preSelected;
	Square selected;

	void Update() {
		if (Input.GetMouseButton(0))            // 按住，連連看
		{
			if (Startbool) eliminateSquare();
		}
		if (Input.GetMouseButtonUp(0))          // 放開，消除方塊
		{
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
				Destroy(Inseries[i].gameObject);
			}
			Inseries.Clear();
			Inseries.Add(null);
			L1.positionCount = 0;
			I1 = 0;
			Startbool = false;
		}
	}
}
