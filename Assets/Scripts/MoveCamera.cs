using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

	public Transform[] ts;
	bool[] lims = new bool[4];

	Camera cam;

	Vector3 right;
	Vector3 forward;
	float oldDist = 0;
	float maxCanSize;

	public delegate void Moving(bool isMoving);
	public static Moving movingCallback;
	bool moving = false;
		
	private void Start() {
		cam = GetComponent<Camera>();
		maxCanSize = cam.orthographicSize;
		if (Generic.gData.minCanSize > maxCanSize) Generic.gData.minCanSize = maxCanSize;

		right = new Vector3(Generic.gData.canMoveSpeed, 0, 0);
		forward = new Vector3(0, 0, Generic.gData.canMoveSpeed);
	}

	void Update() {
		//PlanerMove();

		if (Input.touchCount >= 2) {
			Touch t0 = Input.GetTouch(0);
			Touch t1 = Input.GetTouch(1);
			float dist = DistSqr(t0.position, t1.position);

			if (oldDist > 0 && (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved)) {
				if (dist > oldDist) {				// 拉近
					if (cam.orthographicSize >= Generic.gData.minCanSize) cam.orthographicSize -= Generic.gData.canZoomSpeed;
				} else if (dist < oldDist) {		// 拉遠
						if (cam.orthographicSize <= maxCanSize) cam.orthographicSize += Generic.gData.canZoomSpeed;
				}
				if (moving == false) {
					if (movingCallback != null) movingCallback(true);
					moving = true;
				}
			}
			oldDist = dist;
		} else {
			oldDist = 0;
			if (moving) {
				if (movingCallback != null) movingCallback(false);
				moving = false;
			}
		}
	}

	void PlanerMove() {

		Vector3[] p = new Vector3[4];

		for (int i = 0; i < 4; i++) {
			p[i] = cam.WorldToViewportPoint(ts[i].position);
		}

		if (p[0].y > 1 && Input.GetKey(KeyCode.W)) {
			transform.position += forward * Time.deltaTime;
		}
		if (p[1].y < 0 && Input.GetKey(KeyCode.S)) {
			transform.position -= forward * Time.deltaTime;
		}
		if (p[2].x < 0 && Input.GetKey(KeyCode.A)) {
			transform.position -= right * Time.deltaTime;
		}
		if (p[3].x > 1 && Input.GetKey(KeyCode.D)) {
			transform.position += right * Time.deltaTime;
		}
	}

	//void Zoom() {
	//	if (Input.GetKey(KeyCode.Z)) {
	//		if (cam.orthographicSize <= maxCanSize) cam.orthographicSize += Generic.gData.canZoomSpeed;
	//	}
	//	if (Input.GetKey(KeyCode.X)) {
	//		if (cam.orthographicSize >= Generic.gData.minCanSize) cam.orthographicSize -= Generic.gData.canZoomSpeed;
	//	}
	//}

	float DistSqr(Vector2 v1, Vector2 v2) {
		float a = v1.x - v2.x;
		float b = v1.y - v2.y;
		return a * a + b * b;
	}
}
