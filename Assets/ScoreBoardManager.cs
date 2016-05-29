using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScoreBoardManager : MonoBehaviour {

    public GameObject ScoreBoardContent;
    public GameObject ScoreBoardEntry;
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
            Text[] textDisplays = go.GetComponentsInChildren<Text>();
            foreach (Text text in textDisplays)
            {
                if(text.gameObject.name == "PlayerNameText")
                {
                    //update player name in scoreboard
                    text.text = player.GetComponent<PlayerInfo>().UserName;
                    break;
                }
                if(text.gameObject.name == "PlayerScoreText")
                {
                    //update player score
                    break;
                }
                if(text.gameObject.name == "PlayerPingText")
                {
                    //update player ping
                    break;
                }
            }
        }
    }

}
