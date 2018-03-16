using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Photon.MonoBehaviour
{
    public GameObject _uiGo;
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;
    // Use this for initialization

    public void Awake()
    {
     
    }
    void Start()
    {
        _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        UIarrangement();
    }

    // Update is called once per frame
    void Update()
    {


    }
    public void UIarrangement()
    {
        YongGameManager.yongGameManager.Sort(gameObject.GetComponent<RectTransform>());
    }
}
