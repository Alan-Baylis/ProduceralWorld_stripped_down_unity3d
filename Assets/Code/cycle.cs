using UnityEngine;
using System.Collections;

public class cycle : MonoBehaviour {

    public float speed;
    public const float speed_24minPERday = 360f / (24 * 60 * 60);
    private Light light;
    public float time = 0f;

	void Start () {
        light = GetComponent<Light>();
	}
	
	void Update () {
        transform.RotateAround(Vector3.zero, Vector3.right, speed_24minPERday * speed);
        transform.LookAt(Vector3.zero);
        time += speed_24minPERday * speed;
        if (time > 360)
            time = 0;
        if((time > 0 && time < 90) || (time > 270 && time < 360))
            light.enabled = true;
        else
            light.enabled = false;
	}
}
