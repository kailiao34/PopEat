using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HallManager : MonoBehaviour
{

    public static HallManager hallManager;

    public RoomUI roomUI;
    public FoodLis foodLisUI;
    public StoreInformationtUI storeInformationtUI;
    public InputField FoodInputField;
    public string food;
    void Start()
    {
        hallManager = this;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (storeInformationtUI.gameObject.activeSelf==true)
            {
                FoodInputField.text = null;
                foodLisUI.gameObject.SetActive(true);
                storeInformationtUI.gameObject.SetActive(false);
            }
            if (roomUI.gameObject.activeSelf == true)
            {
                FoodInputField.text = null;
                foodLisUI.gameObject.SetActive(true);
                roomUI.gameObject.SetActive(false);
            }
        }
    }
    public void StartFoodInputField()
    {

    }
    public void StopFoodInputField()
    {

    }
}
