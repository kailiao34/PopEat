//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MoveCamera : MonoBehaviour {

//	public Transform[] ts;
//	bool[] lims = new bool[4];

//	Camera cam;

	
//	float maxCanSize;

//	public delegate void Moving(bool isMoving);
//	public static Moving movingCallback;
//	bool moving = false;

//	int moveOrZoon;                // 0 ~ +3: 平移, 0 ~ -3: 縮放

//	LateQueue<float> queue = new LateQueue<float>(4);
//	LateQueue<Vector2> oldPos0 = new LateQueue<Vector2>(2);
//	LateQueue<Vector2> oldPos1 = new LateQueue<Vector2>(2);

//	private void Start() {
//		cam = GetComponent<Camera>();
//		maxCanSize = cam.orthographicSize;
//		if (Generic.gData.minCanSize > maxCanSize) Generic.gData.minCanSize = maxCanSize;

		
//	}

//	void Update() {

//		if (Input.touchCount >= 2) {
//			Touch t0 = Input.GetTouch(0);
//			Touch t1 = Input.GetTouch(1);
//			float dist = DistSqr(t0.position, t1.position);

//			if ((t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved)) {
//				float oldDist = queue.In(dist);

//				if (queue.gotOne) {
//					float delta = dist - oldDist;
//					bool p = false;

//					// 判斷是平移還是縮放
//					if (delta > -4500 && delta < 4500) {                                        // 平移
//						if (moveOrZoon < 3) moveOrZoon++;
//						p = true;
//					} else {                                                                    // 縮放
//						if (moveOrZoon > -3) moveOrZoon--;
//					}

//					if (moveOrZoon > 0) {                               // 正在平移狀態
//						PlanerMove(t0.position, t1.position);
//					} else if (moveOrZoon < 0) {                        // 正在縮放狀態
//						Zoom(delta);
//					} else {                                            // 中立
//						if (p) PlanerMove(t0.position, t1.position);
//						else Zoom(delta);
//					}
//				}
//			}
//			if (!moving) {
//				if (movingCallback != null) movingCallback(true);
//				moving = true;
//			}
//		} else {
//			if (moving) {
//				if (movingCallback != null) movingCallback(false);
//				queue.Reset(4);
//				oldPos0.Reset(2);
//				oldPos1.Reset(2);
//				moveOrZoon = 0;
//				moving = false;
//			}

//		}
//		//PlanerMove();
//		//Zoom();

//	}

//	void Zoom(float delta) {
//		if (delta > 0) {                // 拉近
//			if (cam.orthographicSize >= Generic.gData.minCanSize) cam.orthographicSize -= Generic.gData.canZoomSpeed;
//		} else {                        // 拉遠
//			if (cam.orthographicSize <= maxCanSize) cam.orthographicSize += Generic.gData.canZoomSpeed;
//		}
//	}

//	void PlanerMove(Vector2 t0Pos, Vector2 t1Pos) {
//		Vector2 v0 = oldPos0.In(t0Pos);
//		Vector2 v1 = oldPos1.In(t1Pos);
//		if (oldPos0.gotOne && oldPos1.gotOne) {
//			Vector3 v = V2toV3((t0Pos - v0) + (t1Pos - v1));        // 與上一個點相對的移動方向 (上下是Z，左右是X)

//			// 限制移動方向
//			Vector3[] p = new Vector3[4];
//			for (int i = 0; i < 4; i++) {
//				p[i] = cam.WorldToViewportPoint(ts[i].position);
//			}
//			if ((p[0].y <= 1 && v.z > 0) ||                            // 到達上方極限
//				(p[1].y >= 0 && v.z < 0)) {                            // 到達下方極限
//				v.z = 0;
//			}
//			if ((p[2].x >= 0 && v.x < 0) ||                            // 到達左方極限
//				(p[3].x <= 1 && v.x > 0)) {                            // 到達右方極限
//				v.x = 0;
//			}
//			transform.position += v * Time.deltaTime * Generic.gData.canMoveSpeed;
//		}
		
//	}

//	void PlanerMove() {
//		Vector3[] p = new Vector3[4];
//		for (int i = 0; i < 4; i++) {
//			p[i] = cam.WorldToViewportPoint(ts[i].position);
//		}

//		Vector3 v = new Vector3();
//		if (Input.GetKey(KeyCode.W)) {
//			v += Vector3.forward;
//		}
//		if (Input.GetKey(KeyCode.S)) {
//			v -= Vector3.forward;
//		}
//		if (Input.GetKey(KeyCode.A)) {
//			v -= Vector3.right;
//		}
//		if (Input.GetKey(KeyCode.D)) {
//			v += Vector3.right;
//		}

//		if ((p[0].y <= 1 && v.z > 0) ||                            // 到達上方極限
//			(p[1].y >= 0 && v.z < 0)) {                            // 到達下方極限
//			v.z = 0;
//		}
//		if ((p[2].x >= 0 && v.x < 0) ||                            // 到達左方極限
//			(p[3].x <= 1 && v.x > 0)) {                            // 到達右方極限
//			v.x = 0;
//		}
//		transform.position += v * Time.deltaTime * Generic.gData.canMoveSpeed;
//	}

//	void Zoom() {
//		if (Input.GetKey(KeyCode.Z)) {
//			if (cam.orthographicSize <= maxCanSize) cam.orthographicSize += Generic.gData.canZoomSpeed;
//		}
//		if (Input.GetKey(KeyCode.X)) {
//			if (cam.orthographicSize >= Generic.gData.minCanSize) cam.orthographicSize -= Generic.gData.canZoomSpeed;
//		}
//	}

//	float DistSqr(Vector2 v1, Vector2 v2) {
//		float a = v1.x - v2.x;
//		float b = v1.y - v2.y;
//		return a * a + b * b;
//	}

//	Vector3 V2toV3(Vector2 v) {
//		return new Vector3(v.x, v.y, 0);
//	}

//	/// <summary>
//	/// 放入 n 個後才能開始取出來的機器，n 個前取出來的都是 default(T)
//	/// 如果 n = 3，想像有一貫通的水管，內有兩個空格，塞入兩個時還取不出來，塞入第三個後最先塞入的會掉出來
//	/// Hint: 用 gotOne 確認取出來的不是 default
//	/// Hint: 所以當 n = 1 時，就放入什麼立即出來一樣的東西
//	/// </summary>
//	class LateQueue<T> {
//		int late = 1;
//		Queue<T> queue = new Queue<T>();
//		public bool gotOne;

//		public LateQueue(int n) {
//			if (n > 1) late = n;
//		}

//		public T In(T value) {
//			queue.Enqueue(value);
//			if (queue.Count >= late) {
//				gotOne = true;                  // 成功取出
//				return queue.Dequeue();
//			} else {                                        // 還不能取
//				return default(T);
//			}
//		}

//		public void Reset(int n) {
//			queue.Clear();
//			late = n;
//			gotOne = false;
//		}
//	}
//}
