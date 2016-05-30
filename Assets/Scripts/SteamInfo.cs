using UnityEngine;
using System.Collections;
using Steamworks;

public class SteamInfo : MonoBehaviour {

    public string SteamName;
    public CSteamID SteamID;
    public ulong m_SteamID;
    public Texture2D m_Image;
    
	// Use this for initialization
	void Start () {
        if (SteamManager.Initialized)
        {
            SteamName = SteamFriends.GetPersonaName();
            SteamID = SteamUser.GetSteamID();
            m_SteamID = SteamID.m_SteamID;
            //m_Image = FindAvatar(SteamID);
            Debug.Log(SteamName);
        }
	}
}
