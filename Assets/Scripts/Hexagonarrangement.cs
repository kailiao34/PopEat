using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagonarrangement : MonoBehaviour {
	public int size;
	public GameObject center, squarePrefab;
	List<GameObject> squares = new List<GameObject>();
	int s = 0;

	void Start() {
		for (int j = 0; j < 6; j++) {
			GameObject tt1 = Instantiate(center, new Vector3(0, 0, 0), Quaternion.identity);
			tt1.gameObject.name = "j=====>" + j;
			for (int i = 0; i < size; i++) {
				GameObject gg = Instantiate(squarePrefab, new Vector3(0, 0, 0), squarePrefab.transform.rotation);
				s++;
				gg.transform.position = new Vector3(0, 0, i * 0.84f);
				gg.transform.parent = tt1.transform;
				gg.gameObject.name = "i=====>" + i;
				if (i >= 1) {
					for (int k = 1; k < i + 1; k++) {
						GameObject gg2 = Instantiate(squarePrefab, new Vector3(0, 0, 0), squarePrefab.transform.rotation);
						s++;
						gg2.transform.position = new Vector3(gg.transform.position.x + k * 0.752f, 0, gg.transform.position.z - k * 0.422f);
						gg2.transform.parent = tt1.transform;
						gg2.gameObject.name = "k=====>" + k;
					}
				}
				if (j > 0 && i == 0) {
					Destroy(gg.gameObject);
					s--;
				}
				if (i > 0) {
					s--;
					Destroy(gg.gameObject);
				}
			}
			if (j >= 1) {
				tt1.transform.eulerAngles = new Vector3(0, j * 60, 0);
			}
		}
		print("六角數量: " + s);
	}
}
