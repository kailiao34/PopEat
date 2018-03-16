using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class YongGameManager : Photon.PunBehaviour
{
    public GameObject Content;
    public GameObject playerPrefab;
    public Text roomname;
    public List<RectTransform> GG = new List<RectTransform>();
    public static YongGameManager yongGameManager;
    // Use this for initialization
    void Start()
    {
        yongGameManager = this;
        if (PhotonNetwork.room != null && roomname != null)
        {
            roomname.text = "房間名稱:" + PhotonNetwork.room.name;
        }
        RectTransform gg = PhotonNetwork.Instantiate(this.playerPrefab.name, Content.transform.parent.position, Quaternion.identity, 0).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
            Application.LoadLevel("Hall");
        }
    }

    public void Sort(RectTransform r1)
    {
        RectTransform R1 = r1;
        R1.transform.parent = Content.transform;

        GG.Add(r1);
        for (int i = 0; i < GG.Count; i++)
        {
            if (i == 0)
            {
                GG[i].anchoredPosition = new Vector2(-1, -55);
                R1.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                GG[i].anchoredPosition = new Vector2(-1, -55 + (i * -91));
                R1.localScale = new Vector3(1, 1, 1);
            }

        }

    }

}
