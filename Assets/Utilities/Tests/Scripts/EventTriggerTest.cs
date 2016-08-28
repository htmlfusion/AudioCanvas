using UnityEngine;
using System.Collections;

public class EventTriggerTest : MonoBehaviour {

	void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            EventManager.TriggerEvent("test");
        }
    }
}
