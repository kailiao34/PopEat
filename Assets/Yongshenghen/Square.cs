using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
    public int Numbering;
    public Renderer thisRenderer;
    public bool Clickbool;
    public Color[] c1;
    Color colorA;
    static public Color colorAss;
    // Use this for initialization
    void Start()
    {
        //thisRenderer = GetComponent<Renderer>();
        //thisRenderer.material=new  Material(Shader.Find("Standard"));
        //GetComponent<MeshRenderer>().material.color
        colorA = c1[Random.Range(0, c1.Length)];
        colorAss = colorA;

        if (GetComponent<MeshRenderer>() == true)
        {
            GetComponent<MeshRenderer>().material.color = colorAss;
        }
        else if (GetComponent<Image>() == true)
        {
            //Debug.Log("image returned");
            GetComponent<Image>().color = colorAss;
        }
        else Debug.Log("error no mesh or iamge renderer");
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
