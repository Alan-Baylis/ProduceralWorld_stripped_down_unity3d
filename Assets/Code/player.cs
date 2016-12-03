using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

    private Vector3 prev;

	void Start () {
        StartCoroutine(goDown());
	}
	
    void Update()
    {
        ObjectManager.instance.ReportPlayerTransform(transform.position.x, transform.position.z);
        DataManager.instance.playerPos = transform.position;

        if (transform.position == prev)
            return;
        DataManager.instance.distanceWalked += Mathf.Abs(Vector3.Distance(transform.position, prev));
        prev = transform.position;
    }

	IEnumerator goDown()
    {
        yield return new WaitForEndOfFrame();
        RaycastHit hit;
        Physics.Raycast(new Ray(transform.position, transform.up * -1), out hit, 2200);
        transform.position -= new Vector3(0, hit.distance - 1, 0);
        for(int i = 0; i < 10; i++) GenObject.instance.SpawnBird();
        yield return new WaitForEndOfFrame();
        DataManager.instance.distanceWalked = 0;
    }
}
