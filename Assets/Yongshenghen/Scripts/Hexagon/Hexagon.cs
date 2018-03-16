using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    public void disappear()
    {
        Destroy(gameObject);
    }
    //void OnMouseDrag()
    //{
    //    Vector3 mov3 = Input.mousePosition;
    //    mov3.z = 26.48f;
    //    Vector3 newmov3 = Camera.main.ScreenToWorldPoint(mov3);
    //    transform.position = newmov3;
    //}
}
