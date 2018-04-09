using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
    public ColorList colorList; // BR - 使用 ColorList Asset 來取得顏色 (scriptable Object)，這樣只需要存取和修改一個顏色表。

    public int Numbering;
    public Renderer thisRenderer;
    public bool Clickbool;
    //public Color[] c1;
    Color colorA;

    void Start()
    {
        //為了讓 Square.cs 能同時用在 六角 polygons 和 六角 UI 二種不同的 materials 而改寫。
        colorA = colorList.colors[Random.Range(0, colorList.colors.Count)];
        if (GetComponent<MeshRenderer>() == true)
        {
            GetComponent<MeshRenderer>().material.color = colorA;
        }
        else if (GetComponent<Image>() == true)
        {
            //Debug.Log("image returned");
            GetComponent<Image>().color = colorA;
        }
        else Debug.Log("error no mesh or iamge renderer");
    }
}
