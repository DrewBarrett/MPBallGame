using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Steamworks;

public class ScoreBoardManager : MonoBehaviour {

    public GameObject ScoreBoardContent;
    public GameObject ScoreBoardEntry;
    //SteamInfo si;
    GameObject[] Players;
    List<GameObject> Entries = new List<GameObject>();

    public void UpdatePlayers()
    {
        //TODO: Compare players to existing list and only delete changes
        foreach (GameObject entry in Entries)
        {
            Destroy(entry);
        }
        Players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in Players)
        {
            GameObject go = (GameObject)Instantiate(ScoreBoardEntry);
            Entries.Add(go);
            go.transform.SetParent(ScoreBoardContent.transform);
            Image avatar = go.GetComponentInChildren<Image>();
            if(player.GetComponent<PlayerInfo>().playerSteamLong > 0)
            {
                avatar.sprite = Sprite.Create(FindAvatar(player.GetComponent<PlayerInfo>().playerSteamLong), new Rect(0, 0, 32, 32), new Vector2(.5f, .5f));
            }
            
            
            Text[] textDisplays = go.GetComponentsInChildren<Text>();
            foreach (Text text in textDisplays)
            {
                if (text.gameObject.name == "PlayerNameText")
                {
                    //update player name in scoreboard
                    //Debug.Log("Updating player names now!");
                    text.text = player.GetComponent<PlayerInfo>().UserName;
                    continue;
                }
                if (text.gameObject.name == "PlayerScoreText")
                {
                    //update player score
                    //Debug.Log("Updating player scores now!");
                    text.text = player.GetComponent<PlayerInfo>().score.ToString();
                    continue;
                }
                if (text.gameObject.name == "PlayerPingText")
                {
                    //update player ping
                    continue;
                }
            }
        }
    }

    public void Start()
    {
        //si = GameObject.Find("Network").GetComponent<SteamInfo>();
    }

    public void UpdateScores()
    {
        //im fucking lazy
        UpdatePlayers();
    }

    public Texture2D FindAvatar(ulong steamID)
    {
        CSteamID theSteamId = new CSteamID(steamID);
        int avatarid = SteamFriends.GetSmallFriendAvatar(theSteamId);
        //Debug.Log(avatarid);
        uint width;
        uint height;
        SteamUtils.GetImageSize(avatarid, out width, out height);
        //Debug.Log(width + " " + height);
        byte[] pubDest = new byte[width * height * 4];
        bool valid = SteamUtils.GetImageRGBA(avatarid, pubDest, 4 * (int)height * (int)width);
        if (valid)
        {
            Texture2D avatarImage = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
            avatarImage.LoadRawTextureData(pubDest);
            avatarImage.Apply();
            return avatarImage;
        }
        Debug.LogError("Could not find image for: " + steamID + width + height + valid);
        return null;
    }

}
