using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

    public const int MAX_OBJECT_COUNT = 1500, REFRESH_RATE = 5, SECTOR_SIZE = 600, SPAWN_RATE = 20, OBJECTS_PER_SECTOR = 120;
    public static ObjectManager instance;

    public int objShow = 0;
    private int objCount = 0;
    private int timer = 0;
    private List<GeneratedObject> objects;
    public float px, pz;
    public Sector[,] sectors;
    private bool first = true;

    public ObjectManager()
    {
        if (instance == null)
        {
            instance = this;
            objects = new List<GeneratedObject>();
            sectors = new Sector[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    sectors[i, j] = new Sector(i - 1, j - 1, 0);

            Debug.Log("ObjectManager Singleton init done");
        }
    }
    
	void Start () {
	}

	void Update () {

        objShow = objCount;
        int secx = (int)Mathf.Floor(px / SECTOR_SIZE);
        int secz = (int)Mathf.Floor(pz / SECTOR_SIZE);

        if (sectors[1,1].x != secx || sectors[1,1].z != secz)
        {
            Sector[,] prev = new Sector[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    prev[i, j] = new Sector(sectors[i, j].x, sectors[i, j].z, 0);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    sectors[i, j].x = i - 1 + secx;
                    sectors[i, j].z = j - 1 + secz;
                    sectors[i, j].todo = 0;
                    //Debug.Log("Sec: " + i + "," + j + " = " + sectors[i, j].x + "," + sectors[i, j].z);
                    bool existed = false;
                    for (int x = 0; x < 3; x++)
                        for(int y = 0; y < 3; y++)
                        {
                            if(sectors[i, j].x == prev[x, y].x && sectors[i, j].z == prev[x, y].z)
                            {
                                existed = true;
                                break;
                            }
                        }
                    if (!existed || first)
                        sectors[i, j].todo = OBJECTS_PER_SECTOR;
                }
            first = false;
        }
        
        timer++;
        if (timer < REFRESH_RATE)
            return;
        timer = 0;

        for(int i = 0; i < 3; i++)
            for(int j = 0; j < 3; j++)
            {
                if(sectors[i, j].todo >= SPAWN_RATE)
                {
                    sectors[i, j].todo -= SPAWN_RATE;
                    for(int k = 0; k < SPAWN_RATE; k++)
                    {
                        float x = (Random.value * SECTOR_SIZE) + (sectors[i, j].x * SECTOR_SIZE) + (SECTOR_SIZE / 2);
                        float z = (Random.value * SECTOR_SIZE) + (sectors[i, j].z * SECTOR_SIZE) - (SECTOR_SIZE / 2);
                        float temp = GenObject.terrainGenerator.GetTemperatureWorldSpace(x, z);
                        
                        if (temp < 0.19f)
                        {
                            float r = Random.value;
                            if (r < 0.9f)
                                AddObject(GenObject.instance.MakeStone(new Vector3(x, -1, z)));
                            else
                                AddObject(GenObject.instance.MakeRock(new Vector3(x, -1, z)));
                        }
                        else if (temp < 0.21f)
                        {
                            float r = Random.value;
                            if (r < 0.7f)
                                AddObject(GenObject.instance.MakeStone(new Vector3(x, -1, z)));
                            else if(r < 0.9f)
                                AddObject(GenObject.instance.MakeRock(new Vector3(x, -1, z)));
                            else
                                AddObject(GenObject.instance.MakeGrass(new Vector3(x, -1, z)));
                        }
                        else if (temp < 0.39f)
                        {
                            float r = Random.value;
                            if(r < 0.5f)
                                AddObject(GenObject.instance.MakeStone(new Vector3(x, -1, z)));
                            else if(r < 0.9f)
                                AddObject(GenObject.instance.MakeGrass(new Vector3(x, -1, z)));
                            else
                                AddObject(GenObject.instance.MakeRock(new Vector3(x, -1, z)));
                        }
                        else if(temp < 0.41f)
                        {
                            float r = Random.value;
                            if (r < 0.5f)
                                AddObject(GenObject.instance.MakeStone(new Vector3(x, -1, z)));
                            else if (r < 0.9f)
                                AddObject(GenObject.instance.MakeGrass(new Vector3(x, -1, z)));
                            else
                                AddObject(GenObject.instance.MakeTree(new Vector3(x, -1, z)));
                        }
                        else if(temp < 0.59f)
                        {
                            float r = Random.value;
                            if(r < 0.5f)
                                AddObject(GenObject.instance.MakeGrass(new Vector3(x, -1, z)));
                            else if(r < 0.75f)
                                AddObject(GenObject.instance.MakeTree(new Vector3(x, -1, z)));
                            else
                                AddObject(GenObject.instance.MakeStruik(new Vector3(x, -1, z)));
                        }
                        else if(temp < 0.61f)
                        {
                            float r = Random.value;
                            if (r < 0.5f)
                                AddObject(GenObject.instance.MakeGrass(new Vector3(x, -1, z)));
                            else if (r < 0.7f)
                                AddObject(GenObject.instance.MakeTree(new Vector3(x, -1, z)));
                            else
                                AddObject(GenObject.instance.MakeStone(new Vector3(x, -1, z)));
                        }
                        else if(temp < 0.79f)
                        {
                            float r = Random.value;
                            if(r < 0.4f)
                                AddObject(GenObject.instance.MakeRock(new Vector3(x, -1, z)));
                            else if(r < 0.6f)
                                AddObject(GenObject.instance.MakeStone(new Vector3(x, -1, z)));
                            else if(r < 0.8)
                                AddObject(GenObject.instance.MakeGrass(new Vector3(x, -1, z)));
                            else
                                AddObject(GenObject.instance.MakeCactus(new Vector3(x, -1, z)));
                        }
                        else if(temp < 0.81f)
                        {
                            float r = Random.value;
                            if (r < 0.5f)
                                AddObject(GenObject.instance.MakeRock(new Vector3(x, -1, z)));
                            else if(r < 0.7f)
                                AddObject(GenObject.instance.MakeStone(new Vector3(x, -1, z)));
                            else
                                AddObject(GenObject.instance.MakeCactus(new Vector3(x, -1, z)));
                        }
                        else if(temp > 0.81f)
                        {
                            float r = Random.value;
                            if(r < 0.5f)
                                AddObject(GenObject.instance.MakeRock(new Vector3(x, -1, z)));
                            else
                                AddObject(GenObject.instance.MakeCactus(new Vector3(x, -1, z)));
                        }
                    }
                }
            }

        for(int i = 0; i < objCount; i++)
        {
            float dx = Mathf.Abs(px - objects[i].gameObject.transform.position.x);
            float dz = Mathf.Abs(pz - objects[i].gameObject.transform.position.z);

            if ((dx * dx) + (dz * dz) > SECTOR_SIZE * SECTOR_SIZE * 4)
            {
                objects[i].Kill();
                DestroyObj(objects[i]);
            }
        }
    }

    private void AddObject(GeneratedObject obj)
    {
        if(objCount < MAX_OBJECT_COUNT)
        {
            objects.Add(obj);
            objCount++;
        }
    }
    public bool IsFull()
    {
        return objCount >= MAX_OBJECT_COUNT;
    }
    public void ReportPlayerTransform(float x, float z)
    {
        px = x;
        pz = z;
    }
    private void DestroyObj(GeneratedObject go)
    {
        objects.Remove(go);
        Destroy(go.gameObject);
        objCount--;
    }
}

public class Sector
{
    public int x, z;
    public int todo;

    public Sector(int x, int z, int todo)
    {
        this.x = x;
        this.z = z;
        this.todo = todo;
    }
}
