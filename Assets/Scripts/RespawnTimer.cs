using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RespawnTimer : MonoBehaviour {

    public float timeRemaining = 0;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0)
            gameObject.GetComponent<Text>().enabled = false;
        //truncate time remaining to two decimal places
        GetComponent<Text>().text = "Respawn in " + timeRemaining.ToString("F2") + " seconds";
	}
    public void UpdateTime(float time)
    {
        gameObject.GetComponent<Text>().enabled = true;
        timeRemaining = time;
    }
}
