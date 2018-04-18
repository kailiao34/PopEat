using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLocationTest : MonoBehaviour
{

    string msg;
    //public GetSurroundingRes SurroundingRes;

    //   private void OnGUI() {
    //	GUI.Label(new Rect(5, Screen.height - 60, 1000, 20), msg);
    //}


    IEnumerator Start()
    {

        msg = "1111";

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        msg = "2222";

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            msg = "Timed out";
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            msg = "Unable to determine device location";
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            msg = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp;
            //StartCoroutine(SurroundingRes.Get("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=+" + Input.location.lastData.latitude + "," + Input.location.lastData.longitude + "&radius=500&type=restaurant&key=AIzaSyDiC9HjxWI0dBa9x5hYL9xOmOnWJcFE-zU"));
        }
        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }

}
