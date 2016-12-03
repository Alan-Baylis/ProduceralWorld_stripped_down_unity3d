using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public string func;
    public bool hasText;
    private Text text;
    public GameObject loading_text;

	void Start () {
        if (hasText)
            text = GetComponent<Text>();
	}
	
	void Update () {
        if(func == "dist_walk")
        {
            text.text = "Distance walked: " + DataManager.instance.distanceWalked + " M";
        }
        else if(func == "dist_map")
        {
            text.text = "Distance to spawn: " + DataManager.instance.distanceCenter + " M";
        }
        else if(func == "temp")
        {
            text.text = "Temperature: " + (DataManager.instance.temperature * 60f - 10f) + " C";
        }
	}

    public void __LoadGame()
    {
        if (loading_text != null)
            loading_text.active = true;
        Application.LoadLevel("DierenTest");
    }

    public void __Save()
    {
        PlayerPrefs.SetFloat("dist_walked", DataManager.instance.distanceWalked);
        PlayerPrefs.SetFloat("dist_map", DataManager.instance.distanceCenter);
        PlayerPrefs.SetFloat("playerx", DataManager.instance.playerPos.x);
        PlayerPrefs.SetFloat("playerz", DataManager.instance.playerPos.z);
    }

    public void __Load()
    {

    }
}
