using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {

	public Transform model;
	public float maxX, maxY;                            // 邊界
	public static HexGrid ins;
	public	List<Vector3> centers = new List<Vector3>();        // 各六邊形的中心點

	private void Awake() {
		ins = this;
	}

	public void GenerateHex() {

		#region ==================================== 排列演算法 ====================================

		Vector3 ext = model.GetComponentInChildren<MeshRenderer>().bounds.extents;
		ext.y = 0;
		Vector3 curLoc = ext;
		float x = Mathf.Sqrt(ext.x * ext.x - ext.z * ext.z);
		Vector3 ver = new Vector3(0, 0, ext.z);
		Vector3 hor = new Vector3(ext.x + x, 0, 0);
		Vector3 ver2 = ver * 2;

		maxX -= ext.x;
		maxY -= ext.z * 2;

		bool odd = true;            // 是否為奇數列
		while (curLoc.x < maxX) {
			centers.Add(curLoc);

			Vector3 loc = curLoc;
			while (loc.z < maxY) {
				loc += ver2;
				centers.Add(loc);
			}

			curLoc += hor;
			if (odd) {
				curLoc += ver;
				odd = false;
			} else {
				curLoc -= ver;
				odd = true;
			}
		}
		#endregion ====================================================================================

		Transform parent = new GameObject("Hexagons").transform;
		foreach (Vector3 c in centers) {
            Instantiate(model, c, Quaternion.identity, parent);// parent
        }
	}
}
