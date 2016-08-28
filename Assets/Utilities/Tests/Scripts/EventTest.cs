using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EventTest : MonoBehaviour {

    private UnityAction testListener;

    void Awake()
    {
        testListener = new UnityAction(TestFunction);
    }

    void OnEnable()
    {
        EventManager.StartListening("test", testListener);
    }

    void OnDisable()
    {
        EventManager.StopListening("test", testListener);
    }

    void Destroy()
    {
        EventManager.StopListening("test", testListener);
    }

    void TestFunction()
    {
        Debug.Log("Test function was called");
    }
}
