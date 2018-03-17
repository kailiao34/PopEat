using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public int Numbering;
    public Renderer thisRenderer;
    public bool Clickbool;
    public Color[] c1;
    // Use this for initialization
    void Start()
    {
        //thisRenderer = GetComponent<Renderer>();
        //thisRenderer.material=new  Material(Shader.Find("Standard"));
		//GetComponent<MeshRenderer>().material.color

		GetComponent<MeshRenderer>().material.color = c1[Random.Range(0, c1.Length)];
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
