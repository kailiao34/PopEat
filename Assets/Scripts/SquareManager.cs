using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareManager : MonoBehaviour {
	public LineRenderer L1;
	int I1;
	List<Square> Inseries = new List<Square>() { null };
	bool Startbool = true;
	Square selected, preSelected;
	// For Hex Arrangement
	public GameObject squarePrefab;
	HashSet<GameObject> squares = new HashSet<GameObject>();

	private void Start() {
		StartCoroutine(StartTicker());
	}

	IEnumerator StartTicker() {
		for (int i = 1; i <= UIRoomManager.gData.startSec; i++) {
			yield return new WaitForSeconds(1f);
			if (i != UIRoomManager.gData.startSec) {
				//print(i);
			} else {
				ArrangeHex();
				yield return new WaitForSeconds(UIRoomManager.gData.overSec);
				enabled = false;
				Release();
				//print("GameOver");

				Dictionary<int, int> d = new Dictionary<int, int>();
				foreach (GameObject g in squares) {
					int n;
					int ii = g.GetComponent<Square>().colorIndex;
					if (d.TryGetValue(ii, out n)) {
						d[ii] = n + 1;
					} else {
						d.Add(ii, 1);
					}
				}

				UIRoomManager.client.SendGameResult(d);         // 傳送本地端統計結果給伺服器

				int colorIndex = 0, max = int.MinValue;
				// 計算本地端獲勝者
				foreach (KeyValuePair<int, int> w in d) {
					if (w.Value > max) {
						colorIndex = w.Key;
						max = w.Value;
					}
				}
				//Debug.Log("本地端獲勝者: " + colorIndex + " - " + UIRoomManager.GetResNameFromColor(colorIndex));
				UIGameManager.ins.GameResultUI();
				UIGameManager.ins.SetLocalWinner(UIRoomManager.GetResNameFromColor(colorIndex), colorIndex);

			}
		}
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
			for (int i = 1; i < UIRoomManager.gData.hexLayers; i++) {
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
