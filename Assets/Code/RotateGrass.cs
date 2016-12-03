using UnityEngine;
using System.Collections;

public class RotateGrass : MonoBehaviour {

    public Transform player;

    private int max = 4, count = 0;

	void Start () {
        count = (int)(Random.value * max);
	}
	
	void Update () {
        count++;
        if (count < max)
            return;

        count = 0;
        Vector3 diff = player.position - transform.position;
        diff.y = 0f;
        transform.rotation = Quaternion.LookRotation(-diff);
	}
}
