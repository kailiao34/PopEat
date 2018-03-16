using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YongPlayerUI : MonoBehaviour {
    PlayerManager _target;
    public Text PlayerNameText;
    // Use this for initialization
    void Start ()
    {
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetTarget(PlayerManager target)
    {

        if (target == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        // Cache references for efficiency because we are going to reuse them.
        _target = target;
     


        CharacterController _characterController = _target.GetComponent<CharacterController>();

        // Get data from the Player that won't change during the lifetime of this Component
        if (PlayerNameText != null)
        {
            PlayerNameText.text = "暱稱:" + _target.photonView.owner.NickName;//+"所選擇的食物:"+ HallManager.hallManager.food;
        }
    }
}
