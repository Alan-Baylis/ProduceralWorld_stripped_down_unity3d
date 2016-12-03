using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

    public float rsX, rsY, rsZ;

	void Start () {
	
	}
	
	void Update () {
        transform.Rotate(rsX, rsY, rsZ);
	}
}
