using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using HoloToolkit.Unity;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA.Sharing;

public class AnchorStoreService : Singleton<AnchorStoreService>
{

    WorldAnchorStore anchorStore;
    WorldAnchor worldAnchor;
    WorldAnchor rootAnchor;

    GameObject hologramCollection;

    bool storeLoaded;

    /// <summary>
    /// WorldAnchorTransferBatch is the primary object in serializing/deserializing anchors.
    /// </summary>
    WorldAnchorTransferBatch transferBatch;

    /// <summary>
    /// The datablob of the anchor.
    /// </summary>
    List<byte> exportingAnchorBytes = new List<byte>();

    // mock test
    GameObject RootAnchorGO;

    void Start()
    {
        // get WorldAnchors directory
        hologramCollection = GameObject.Find("HologramCollection");
        WorldAnchorStore.GetAsync(StoreLoaded);
        // MOCK
        RootAnchorGO = GameObject.Find("RootAnchor");
        //RootAnchorGO.GetComponent<Anchor>().AnchorStoreReady();
    }

    // sets all found anchors when WAS has loaded
    void StoreLoaded(WorldAnchorStore store)
    {
        Debug.Log("WorldAnchorStore Loaded");
        anchorStore = store;
        storeLoaded = true;

        RootAnchorGO.GetComponent<Anchor>().AnchorStoreReady();
        //foreach (Transform child in hologramCollection.transform)
        //{
        //    var go = child.transform.gameObject;

        //    var thisAnchor = store.Load(go.name, go);
        //    if (!thisAnchor)
        //    {
        //        Debug.Log("No saved anchors have been found");
        //    }
        //    else
        //    {
        //        Debug.Log("Anchor " + thisAnchor.name + " has been placed");
        //    }

        //    // notifies children the store is ready
        //    if (go.GetComponent<Anchor>())
        //    {
        //        Debug.Log("game object with anchor has been found");
        //        go.GetComponent<Anchor>().AnchorStoreReady();
        //    }
        //    ImportRootGameObject();
        //}
    }

    public void SaveAnchorLocation(string anchorName, WorldAnchor anchor)
    {
        Debug.Log("save anchor called");

        // ** attempting to save a key that already exists will fail, not overwrite! **
        // if key id exists in anchor store, delete first, then add new reference
        string[] ids = anchorStore.GetAllIds();
        foreach (string id in ids)
        {
            if (id == anchorName)
            {
                Debug.Log("anchor key already exists...deleting " + id);
                anchorStore.Delete(id);
            }
        }
        //var savedAnchor = anchorStore.Save(anchorName, anchor);
        if (anchorStore.Save(anchorName, anchor))
        {
            Debug.Log("anchor has been successfully saved. Now updating transfer batch");
            ExportRootAnchor(anchorName, anchor);
        }
    }

    private void ExportRootAnchor(string name, WorldAnchor thisAnchor)
    {
        transferBatch = new WorldAnchorTransferBatch();
        transferBatch.AddWorldAnchor(name, thisAnchor);
        WorldAnchorTransferBatch.ExportAsync(transferBatch, OnExportDataAvailable, OnExportComplete);
    }

    private void OnExportDataAvailable(byte[] data)
    {
        Debug.Log("Data ready to be transfered to client");
        //TransferDataToClient(data);
    }

    private void OnExportComplete(SerializationCompletionReason status)
    {
        if (status != SerializationCompletionReason.Succeeded)
        {
            Debug.Log("SendExport Failed");
            //SendExportFailedToClient();
        }
        else
        {
            Debug.Log("SendExport Succeeded");
            //SendExportSucceededToClient();
        }
    }



    // This byte array should have been updated over the network from TransferDataToClient
    private byte[] importedData;
    private int retryCount = 3;

    private void ImportRootGameObject()
    {
        WorldAnchorTransferBatch.ImportAsync(importedData, OnImportComplete);
    }

    private void OnImportComplete(SerializationCompletionReason completionReason, WorldAnchorTransferBatch deserializedTransferBatch)
    {
        if (completionReason != SerializationCompletionReason.Succeeded)
        {
            Debug.Log("Failed to import: " + completionReason.ToString());
            if (retryCount > 0)
            {
                retryCount--;
                WorldAnchorTransferBatch.ImportAsync(importedData, OnImportComplete);
            }
            return;
        }

        rootAnchor = deserializedTransferBatch.LockObject("RootAnchor", RootAnchorGO);
        Debug.Log("RootGame object: " + RootAnchorGO.transform.position);
    }

}
