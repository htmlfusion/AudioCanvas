﻿using UnityEngine;
using System.Collections;

using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;

public class Anchor : MonoBehaviour
{

    GameObject appManager;
    AnchorStoreService anchorService;

    bool anchorStoreLoaded = false;
    bool isPlacing = false;

    // Use this for initialization
    void Start()
    {
        appManager = GameObject.Find("AppManager");
        anchorService = appManager.GetComponent<AnchorStoreService>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AnchorStoreReady()
    {
        Debug.Log("Anchor store is ready has been called inside Anchor script");
        anchorStoreLoaded = true;
    }

    public void RemoveAnchor()
    {
        Debug.Log("Remove Anchor Called");
        if (gameObject.GetComponent<WorldAnchor>())
        {
            Debug.Log("removing world anchor on " + gameObject.name);
            DestroyImmediate(gameObject.GetComponent<WorldAnchor>());
        }
    }

    public void SetAnchor()
    {
        if (!gameObject.GetComponent<WorldAnchor>())
        {
            Debug.Log("adding world anchor on " + gameObject.name);
            var anchor = gameObject.AddComponent<WorldAnchor>();

            if (anchorStoreLoaded)
            {
                anchorService.SaveAnchorLocation(gameObject.name, anchor);
            }
            else
            {
                Debug.Log("The WorldAnchorStore has not loaded yet. Anchor not saved!");
            }
        }
    }

    void OnSelect()
    {
        isPlacing = !isPlacing;

        if (isPlacing)
        {
            RemoveAnchor();
        }
        else
        {
            SetAnchor();
        }
    }
}
