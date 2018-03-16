using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSurroundingRes : MonoBehaviour
{
    [HideInInspector]
    public List<string> names;
    public FoodLis foodLis;

    void Awake()
    {
        //StartCoroutine(Get("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=+" + 25.065272 + "," + 121.458542 + "&radius=500&type=restaurant&key=AIzaSyDiC9HjxWI0dBa9x5hYL9xOmOnWJcFE-zU"));
        //foodLis.list = names;
        // foodLis.Getlist();
    }
    void Update()
    {


    }
    public IEnumerator Get(string phth)
    {

        WWW www = new WWW(phth);
        yield return www;

        string text = www.text;
        int s = 0, e = 0;

        while (true)
        {
            s = text.IndexOf("name", s) + 9;
            if (s <= 9) break;
            e = text.IndexOf("\",", s);
            names.Add(text.Substring(s, e - s));

            if (e <= 0) break;

            s = e;
        }
        foodLis.list = names;
        foodLis.Getlist();
    }
}
