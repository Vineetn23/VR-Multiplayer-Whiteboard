using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public GameObject LocalXRRigGO;
    public GameObject MainAvatarGO;


    public GameObject AvatarHeadGO;
    public GameObject AvatarBodyGO;

    public GameObject[] AvatarModelPrefabs;

    public TextMeshProUGUI PlayerName_Text;

    void Start()
    {
        if (photonView.IsMine)
        {
            //player is local
            LocalXRRigGO.SetActive(true);

            //Getting Avatar Selection Data so that the correct avatar model can be instantiated.
            object avatarSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            {
                Debug.Log("Avatar Selection Number: " + (int)avatarSelectionNumber);
                photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered,(int)avatarSelectionNumber);
            }

            SetLayerRecursively(AvatarBodyGO,7);
            SetLayerRecursively(AvatarHeadGO,6);

            TeleportationArea[] teleportationAreas = GameObject.FindObjectsOfType<TeleportationArea>();
            if (teleportationAreas.Length > 0)
            {
                Debug.Log("Found" + teleportationAreas.Length + "teleportation area.");
                foreach(var item in teleportationAreas)
                {
                    item.teleportationProvider = LocalXRRigGO.GetComponent<TeleportationProvider>();
                }
            }
            MainAvatarGO.AddComponent<AudioListener>();

        }
        else
        {
            //player is remote
            LocalXRRigGO.SetActive(false) ;
            SetLayerRecursively(AvatarBodyGO, 0);
            SetLayerRecursively(AvatarHeadGO, 0);
        }

        if(PlayerName_Text != null)
        {
            PlayerName_Text.text = photonView.Owner.NickName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    [PunRPC]
    public void InitializeSelectedAvatarModel(int avatarSelectionNumber)
    {
        GameObject selectedAvatarGameobject = Instantiate(AvatarModelPrefabs[avatarSelectionNumber], LocalXRRigGO.transform);

        AvatarInputConverter avatarInputConverter = LocalXRRigGO.GetComponent<AvatarInputConverter>();
        AvatarHolder avatarHolder = selectedAvatarGameobject.GetComponent<AvatarHolder>();
        SetUpAvatarGameobject(avatarHolder.HeadTransform, avatarInputConverter.AvatarHead);
        SetUpAvatarGameobject(avatarHolder.BodyTransform, avatarInputConverter.AvatarBody);
        SetUpAvatarGameobject(avatarHolder.HandLeftTransform, avatarInputConverter.AvatarHand_Left);
        SetUpAvatarGameobject(avatarHolder.HandRightTransform, avatarInputConverter.AvatarHand_Right);
    }

    void SetUpAvatarGameobject(Transform avatarModelTransform, Transform mainAvatarTransform)
    {
        avatarModelTransform.SetParent(mainAvatarTransform);
        avatarModelTransform.localPosition = Vector3.zero;
        avatarModelTransform.localRotation = Quaternion.identity;
    }

}
