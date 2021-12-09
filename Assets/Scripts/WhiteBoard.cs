using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WhiteBoard : MonoBehaviourPunCallbacks
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048, 2048);
    private Material m;

    void Start()
    {
        var r = GetComponent<Renderer>();
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        r.material.mainTexture = texture;
    }

    private void Update()
    {
        ChangeTexture();
        photonView.RPC("ChangeTexture", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ChangeTexture()
    {
        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
    }

}
