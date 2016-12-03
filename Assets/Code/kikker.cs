using UnityEngine;
using System.Collections;

public class kikker : MonoBehaviour {

    private KIKKER_AI state;
    private float speed;
    private float turnspeed;
    private int max, count;
    private float spingcount = 0f, springheight = 0f;

    void Start () {
        state = new KIKKER_AI();
        state = KIKKER_AI.LOOP;
        transform.Rotate(0f, Random.value * 360f, 0f);
        speed = Random.value * 1f + 3f;
        turnspeed = Random.value * 1f + 0.5f;
        count = 0;

        float x = transform.position.x;
        float z = transform.position.z;
        transform.position = new Vector3(x, GenObject.terrainGenerator.GetHeight(x, z) + 50, z);
    }
	
	void Update () {
        if (state == KIKKER_AI.LOOP)
            LOOP();
        else if (state == KIKKER_AI.LEFT)
            LEFT();
        else if (state == KIKKER_AI.RIGHT)
            RIGHT();
        else if (state == KIKKER_AI.SPRING)
            SPRING();

        float x = transform.position.x;
        float z = transform.position.z;
        transform.position = new Vector3(x, GenObject.terrainGenerator.GetHeight(x, z) + 0.3f + springheight, z);
    }

    private void LOOP()
    {
        if (count == 0)
        {
            max = (int)(Random.value * 240 + 60);
        }
        if (count >= max)
        {
            count = 0;
            float r = Random.value;
            if (r < 0.33f)
                state = KIKKER_AI.LEFT;
            else if (r < 0.66f)
                state = KIKKER_AI.RIGHT;
            else
                state = KIKKER_AI.SPRING;
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
                state = KIKKER_AI.LOOP;
            else
                state = KIKKER_AI.RIGHT;
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
                state = KIKKER_AI.LEFT;
            else
                state = KIKKER_AI.LOOP;
        }
        count++;
        transform.position += transform.forward * speed * 0.666f * Time.deltaTime;
        transform.Rotate(0, -turnspeed, 0);
    }
    private void SPRING()
    {
        if (count == 0)
            max = 30;
        if (count >= max)
        {
            springheight = 0f;
            spingcount = 0f;
            count = 0;
            float r = Random.value;
            if (r < 0.5f)
                state = KIKKER_AI.LEFT;
            else
                state = KIKKER_AI.LOOP;
        }
        count++;
        transform.position += transform.forward * speed * Time.deltaTime;
        spingcount += 1f/30f;
        springheight = (float)System.Math.Sin(spingcount * Mathf.PI * 0.25f) * 6;
    }
}

public enum KIKKER_AI
{
    LOOP,
    LEFT,
    RIGHT,
    SPRING
}
