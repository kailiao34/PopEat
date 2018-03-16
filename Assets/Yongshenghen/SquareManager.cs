using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareManager : MonoBehaviour
{

    public LineRenderer L1;
    public int I1;
    public List<Square> Inseries;
    public bool Startbool;

    // Use this for initialization
    void Start()
    {

    }
    Ray ray;
    RaycastHit hit;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Startbool)
            {
                eliminateSquare();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Inseries.Count != 0 && Inseries[0] != null)
            {
                for (int i = 0; i < Inseries.Count; i++)
                {
                    Destroy(Inseries[i].gameObject);
                }

            }
            Inseries.Clear();
            Inseries.Add(null);
            Startbool = true;

            L1.positionCount = 0;
            I1 = 0;
        }
    }
    public void eliminateSquare()
    {

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (hit.collider.gameObject.tag == "Square")
            {
                Colourdetermination();
            }
            else if (hit.collider.gameObject.tag == "floor")
            {
                if (Inseries[0] != null)
                {
                    for (int i = 0; i < Inseries.Count; i++)
                    {
                        Destroy(Inseries[i].gameObject);
                    }
                    Inseries.Clear();
                    Inseries.Add(null);
                    L1.positionCount = 0;
                    I1 = 0;
                    Startbool = false;
                }
            }
        }
        Debug.DrawLine(ray.origin, hit.point, Color.green);
    }
    public void Colourdetermination()
    {
        if (Inseries[0] == null)
        {
            L1.positionCount += 1;
            I1 += 1;
            Inseries[0] = hit.collider.gameObject.GetComponent<Square>();
            L1.SetPosition(0, new Vector3( hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y+1f, hit.collider.gameObject.transform.position.z));
        }
        if (Inseries[0] != null)
        {
            Square judgment = hit.collider.gameObject.GetComponent<Square>();
            if (judgment.Clickbool == false&& judgment!= Inseries[0])
            {
                if (judgment.thisRenderer.material.color != Inseries[0].thisRenderer.material.color)
                {
                    for (int i = 0; i < Inseries.Count; i++)
                    {
                        Destroy(Inseries[i].gameObject);
                    }
                    Inseries.Clear();
                    Inseries.Add(null);
                    L1.positionCount = 0;
                    I1 = 0;
                    Startbool = false;

                }
                else if (judgment.thisRenderer.material.color == Inseries[0].thisRenderer.material.color)
                {
                    Inseries.Add(judgment);
                    L1.positionCount += 1;
                    L1.SetPosition(Inseries.Count-1, new Vector3(judgment.gameObject.transform.position.x, judgment.gameObject.transform.position.y+1f, judgment.gameObject.transform.position.z));
                    judgment.Clickbool = true;
                }

            }
        }
    }
}
