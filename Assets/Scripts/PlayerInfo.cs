using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Steamworks;

public class PlayerInfo : NetworkBehaviour {
    [SyncVar(hook = "UserNameUpdated")]
    public string UserName;
    [SyncVar(hook = "ScoreUpdated")]
    public int score;
    [SyncVar]
    public ulong playerSteamLong;
    SteamInfo si;
    ScoreBoardManager sbm;
	// Use this for initialization
	void Start () {
        sbm = GameObject.Find("_LOCALSCRIPTS").GetComponent<ScoreBoardManager>();
        if (!isLocalPlayer)
            return;
        si = GameObject.Find("Network").GetComponent<SteamInfo>();

        CmdSetSteamId(si.m_SteamID);        
        CmdSetUserName(si.SteamName);
    }
	
    [Command]
    void CmdSetUserName(string name)
    {
        UserName = name;
    }
    [Command]
    void CmdSetSteamId(ulong steamid)
    {
        playerSteamLong = steamid;
    }
	// Update is called once per frame
	void Update () {
	
	}

    void UserNameUpdated(string name)
    {
        UserName = name;
        Debug.Log(name + " joined the game!");
        sbm.UpdatePlayers();
    }
    void ScoreUpdated(int val)
    {
        score = val;
        sbm.UpdateScores();
    }
}
