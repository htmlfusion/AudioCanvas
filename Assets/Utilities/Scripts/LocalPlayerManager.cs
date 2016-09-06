using UnityEngine;
using HoloToolkit.Unity;

/// <summary>
/// Manages local player state.
/// </summary>
public class LocalPlayerManager : Singleton<LocalPlayerManager>
{
    /// <summary>
    /// The selected avatar index for the player.
    /// </summary>
    public int AvatarIndex { get; private set; }

    /// <summary>
    /// Changes the user's avatar and lets everyone know.
    /// </summary>
    /// <param name="AvatarIndex"></param>
    public void SetUserAvatar(int AvatarIndex)
    {
        this.AvatarIndex = AvatarIndex;

        // Let everyone else know who we are.
        SendUserAvatar();
    }

    /// <summary>
    /// Broadcasts the user's avatar to other players.
    /// </summary>
    public void SendUserAvatar()
    {
        Debug.Log("Send user avatar called in LocalPlayerManager");
        CustomMessages.Instance.SendUserAvatar(AvatarIndex);
    }

    // Send the user's position each frame.
    void Update()
    {
        if (ImportExportAnchorManager.Instance.AnchorEstablished)
        {
            Debug.Log("reached the if of importexport.anchorEstablished");
            // Grab the current head transform and broadcast it to all the other users in the session
            Transform headTransform = Camera.main.transform;

            // Transform the head position and rotation into local space
            Vector3 headPosition = this.transform.InverseTransformPoint(headTransform.position);
            Quaternion headRotation = Quaternion.Inverse(this.transform.rotation) * headTransform.rotation;
            CustomMessages.Instance.SendHeadTransform(headPosition, headRotation, 0x1);
        }
    }
}