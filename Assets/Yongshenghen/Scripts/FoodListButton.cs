using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FoodListButton : MonoBehaviour {
    public Text UItext;
    public FoodLis foodLis;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void select()
    {
        HallManager.hallManager.roomUI.gameObject.SetActive(true);
        HallManager.hallManager.foodLisUI.gameObject.SetActive(false);
        HallManager.hallManager.food = UItext.text;
    }
    public void OpenStoreInformation()
    {
        HallManager.hallManager.foodLisUI.gameObject.SetActive(false);
        HallManager.hallManager.storeInformationtUI.gameObject.SetActive(true);
    }
}
