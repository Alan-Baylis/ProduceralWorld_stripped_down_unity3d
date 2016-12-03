using UnityEngine;
using System.Collections;

public class DataManager : MonoBehaviour {

    public static DataManager instance;
    public Vector3 playerPos;
    public float distanceWalked;
    public float distanceCenter;
    public float temperature;
    public float time;
    private int count = 0;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

	void Start () {
	    
	}
	
	void Update () {
        time++;
        distanceCenter = Vector2.Distance(Vector2.zero, new Vector2(playerPos.x, playerPos.z));
        temperature = GenObject.terrainGenerator.GetTemperatureWorldSpace(playerPos.x, playerPos.z);

        count++;
        if(count > 120)
        {
            PlayerPrefs.SetFloat("dist_walked", distanceWalked);
            PlayerPrefs.SetFloat("dist_map", distanceCenter);
            PlayerPrefs.SetFloat("playerx", playerPos.x);
            PlayerPrefs.SetFloat("playerz", playerPos.z);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Application.LoadLevel("Menu");
        }
    }
}
