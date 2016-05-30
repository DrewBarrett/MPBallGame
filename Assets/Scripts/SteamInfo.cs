using UnityEngine;
using System.Collections;
using Steamworks;

public class SteamInfo : MonoBehaviour {

    public string SteamName;
	// Use this for initialization
	void Start () {
        SteamName = SteamFriends.GetPersonaName();
        Debug.Log(SteamName);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
