using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareManager : MonoBehaviour {
	[SerializeField]
	public LineRenderer linePrefab;
	// For Hex Arrangement
	[SerializeField]
	GameObject squarePrefab, hexFXPrefab;
	[SerializeField]
	UnityEngine.UI.Text tickerText;
	HashSet<GameObject> squares = new HashSet<GameObject>();
	//bool stop = false;

	Camera cam;
	Queue<LineRenderer> lineRecycling = new Queue<LineRenderer>();
	List<LineRenderer> lineList = new List<LineRenderer>();
	List<List<Square>> hexList = new List<List<Square>>();
	List<bool> beginList = new List<bool>();
	List<Square> curSelected = new List<Square>();

	private void Start() {
		UIRoomManager.curStage = 4;
		StartCoroutine(StartTicker());
		//MoveCamera.movingCallback = MoveSwitch;
		cam = Camera.main;
	}

	//void MoveSwitch(bool dontMove) {
	//	if (stop) return;
	//	if (dontMove) {
	//		Release();
	//		enabled = false;
	//	} else {
	//		enabled = true;
	//	}
	//}

	IEnumerator StartTicker() {
		for (int i = Generic.gData.startSec; i > 0; i--) {  // 教學動畫倒數
			yield return new WaitForSeconds(1f);
		}

		ArrangeHex();
		for (int i = Generic.gData.overSec; i >= 0; i--) {     // 遊戲時間倒數
			tickerText.text = /*"剩餘時間: " +*/ i.ToString();
			yield return new WaitForSeconds(1f);
		}
		#region ================== 時間到後的工作 ==================
		enabled = false;                                        // 不能再消六角
																//stop = true;
		for (int i = 0; i < hexList.Count; i++) {                // 消除正在連的
			Release(i);
		}
		//print("GameOver");

		Dictionary<int, int> d = new Dictionary<int, int>();
		foreach (GameObject g in squares) {                     // 計算各顏色剩餘數量
			int n;
			int ii = g.GetComponent<Square>().colorIndex;
			if (d.TryGetValue(ii, out n)) {
				d[ii] = n + 1;
			} else {
				d.Add(ii, 1);
			}
		}

		UIRoomManager.client.SendGameResult(d);                 // 傳送本地端統計結果給伺服器

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
		#endregion ========================================================
	}

	void Update() {
		for (int i = 0; i < Input.touchCount; i++) {
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began) {
				NewLine(Input.touchCount);
				beginList[i] = true;
			}

			if (touch.phase == TouchPhase.Ended) {          // 放開，消除方塊
				Release(i);
				RemoveLine(i);
			} else {                                        // 按住，連連看
				if (beginList[i]) Click(Input.mousePosition, i);
			}
		}

#if UNITY_EDITOR
		// 滑鼠測試用
		if (Input.GetMouseButtonDown(0)) {
			NewLine(1);
			beginList[0] = true;
		}
		if (Input.GetMouseButton(0)) {           // 按住，連連看
			if (beginList[0]) Click(Input.mousePosition, 0);
		}
		if (Input.GetMouseButtonUp(0)) {         // 放開，消除方塊
			Release(0);
			RemoveLine(0);
		}
#endif
	}

	void Click(Vector2 screenPoint, int index) {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(screenPoint);
		if (Physics.Raycast(ray, out hit, 100.0f)) {
			Square hex = hit.collider.gameObject.GetComponent<Square>();
			if (hex != null) {                                  // 點到六角
				if (hex == curSelected[index]) return;
				// 點到已被選過的或不同顏色的
				if (hex.Clickbool || (curSelected[index] != null && hex.colorIndex != curSelected[index].colorIndex)) {
					Release(index);
					return;
				}
				// LineRenderer
				int c = lineList[index].positionCount;
				lineList[index].positionCount = c + 1;
				lineList[index].SetPosition(c, hex.transform.position);
				hex.Clickbool = true;
				// Hex
				hexList[index].Add(hex);
				curSelected[index] = hex;
			} else {                                            // 點到地板
				Release(index);
			}
		}
	}

	void NewLine(int count) {
		for (int i = lineList.Count; i < count; i++) {
			LineRenderer l;
			if (lineRecycling.Count == 0) {
				l = Instantiate(linePrefab);
			} else {
				l = lineRecycling.Dequeue();
			}
			lineList.Add(l);
			hexList.Add(new List<Square>());
			beginList.Add(true);
			curSelected.Add(null);
		}
	}

	void RemoveLine(int index) {
		lineRecycling.Enqueue(lineList[index]);

		lineList.RemoveAt(index);
		hexList.RemoveAt(index);
		beginList.RemoveAt(index);
		curSelected.RemoveAt(index);
	}

	void Release(int index) {

		// 播放音效
		if (hexList[index].Count == 1) {
			Sounds.PlayHex();
		} else if (hexList[index].Count > 1) {
			Sounds.PlayHexMul();
		}

		if (hexList[index].Count > 0) {
			beginList[index] = false;
		}

		// 消除選到的六角
		for (int i = 0; i < hexList[index].Count; i++) {
			squares.Remove(hexList[index][i].gameObject);
			Instantiate(hexFXPrefab, hexList[index][i].transform.position, Quaternion.identity);      // 爆破效果

			Destroy(hexList[index][i].gameObject);
		}

		hexList[index].Clear();
		// 關掉這條 LineRenderer
		lineList[index].positionCount = 0;
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
