using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour {

    private BIRD_AI state;
    private float speed;
    private float turnspeed;
    private int max, count;
    public AudioClip clip;
    private AudioSource source;

	void Start () {
        state = new BIRD_AI();
        state = BIRD_AI.FLY;
        transform.Rotate(0f, Random.value * 360f, 0f);
        speed = Random.value * 20f + 10f;
        turnspeed = Random.value * 2f + 1f;
        count = 0;
        StartCoroutine(goDown());
        source = GetComponent<AudioSource>();
	}

    IEnumerator goDown()
    {
        yield return new WaitForEndOfFrame();
        RaycastHit hit;
        Physics.Raycast(new Ray(transform.position, transform.up * -1), out hit, 2200);
        transform.position -= new Vector3(0, hit.distance - 100, 0);
    }

    void Update () {
        if (state == BIRD_AI.FLY)
            FLY();
        else if (state == BIRD_AI.LEFT)
            LEFT();
        else if (state == BIRD_AI.RIGHT)
            RIGHT();
        else if (state == BIRD_AI.SCREAM)
            SCREAM();
        if(count % 2 == 0)
        {
            float x = transform.position.x;
            float z = transform.position.z;
            transform.position = new Vector3(x, GenObject.terrainGenerator.GetHeight(x, z) + 50, z);
        }
    }

    private void FLY()
    {
        if(count == 0)
        {
            max = (int)(Random.value * 240 + 60);
        }
        if(count >= max)
        {
            count = 0;
            float r = Random.value;
            if (r < 0.33f)
                state = BIRD_AI.LEFT;
            else if (r < 0.66f)
                state = BIRD_AI.RIGHT;
            else
                state = BIRD_AI.SCREAM;
        }
        count++;
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void LEFT()
    {
        if (count == 0)
        {
            max = (int)(Random.value * 120 + 30);
        }
        if (count >= max)
        {
            count = 0;
            float r = Random.value;
            if (r < 0.5f)
                state = BIRD_AI.FLY;
            else
                state = BIRD_AI.RIGHT;
        }
        count++;
        transform.position += transform.forward * speed * 0.666f * Time.deltaTime;
        transform.Rotate(0, turnspeed, 0);
    }
    private void RIGHT()
    {
        if (count == 0)
        {
            max = (int)(Random.value * 120 + 30);
        }
        if (count >= max)
        {
            count = 0;
            float r = Random.value;
            if (r < 0.5f)
                state = BIRD_AI.LEFT;
            else
                state = BIRD_AI.FLY;
        }
        count++;
        transform.position += transform.forward * speed * 0.666f * Time.deltaTime;
        transform.Rotate(0, -turnspeed, 0);
    }
    private void SCREAM()
    {
        count = 0;
        transform.position += transform.forward * speed * Time.deltaTime;
        source.PlayOneShot(clip);
        state = BIRD_AI.FLY;
    }
}

public enum BIRD_AI
{
    FLY,
    LEFT,
    RIGHT,
    SCREAM
}