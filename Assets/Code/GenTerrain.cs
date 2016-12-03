using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Collections;
using System;

public class GenTerrain : MonoBehaviour {

    public float temp_debug;
    public int lands;
    public int scale;
    public float biomeScale;
    public float perlinScale0, perlinScale1, perlinScale2, perlinScale3;
    public float relief0, relief1, relief2, relief3;
    public Material mat;
    public int xo, yo;
    public AnimationCurve curve;

    private List<Plane_TG> planes;
    private WorkerThread thread_texture;
    private WorkerThread thread_mesh;
    private Texture2D[] textures_far, textures_close;
    private Texture2D detailTexture;
    public GameObject player;
    private float playerY;
    private Plane_TG standingOn;
    private int prevX, prevY, vakX, vakY;
    private bool running = false;
    private System.Random rand = new System.Random();
    
    private GameObject dummy;
    private BiomeData dummyBiome;
    private int textureCounterFar = 0, textureCounterClose = 0;

    private const int TEXTURESIZE_CLOSE = 256;
    private const int TEXTURESIZE_FAR = 200;
    private static int GLOBAL_VOID_ID = 0;
    private VoidDataBase voidbase;

    public int tasksInQueue = 0;
    public GameObject testplane;

    void Start() {
        if (xo == 0 && yo == 0)
        {
            xo = (int)(UnityEngine.Random.value * 10000000);
            yo = (int)(UnityEngine.Random.value * 10000000);
        }

        dummy = new GameObject();
        dummyBiome = new BiomeData();

        planes = new List<Plane_TG>();
        thread_texture = new WorkerThread();
        //thread_mesh = new WorkerThread();

        voidbase = new VoidDataBase();

        textures_far = new Texture2D[32];
        for(int i = 0; i < 32; i++)
        {
            textures_far[i] = new Texture2D(TEXTURESIZE_FAR, TEXTURESIZE_FAR);
            textures_far[i].filterMode = FilterMode.Bilinear;
            textures_far[i].wrapMode = TextureWrapMode.Clamp;
        }
        textures_close = new Texture2D[9];
        for (int i = 0; i < 9; i++)
        {
            textures_close[i] = new Texture2D(TEXTURESIZE_CLOSE, TEXTURESIZE_CLOSE);
            textures_close[i].filterMode = FilterMode.Trilinear;
            textures_close[i].wrapMode = TextureWrapMode.Clamp;
        }

        detailTexture = new Texture2D(4048, 4048);
        GenDetailTexture_Sand(detailTexture);

        Texture2D testtex = new Texture2D(128, 128);
        for(int x = 0; x < 128; x++)
        {
            for(int y = 0; y < 128; y++)
            {
                float val = GetTemperature(x * 0.02f, y * 0.02f);
                testtex.SetPixel(x, y, new Color(val, val, val));
            }
        }
        testtex.Apply();
        testplane.GetComponent<Renderer>().material.mainTexture = testtex;

        player = GameObject.FindWithTag("Player");

        int vakX = (int)Mathf.Floor(player.transform.position.x / scale);
        int vakY = (int)Mathf.Floor(player.transform.position.z / scale);
        prevX = vakX;
        prevY = vakY;

        BuildTerrain();
    }
    
    void Update() {
        Void returned = thread_texture.returnFinishedTask();
        if (returned != null)
        {
            Action action;
            voidbase.database.TryGetValue(returned.keyToFollowup, out action);
            action();
            tasksInQueue--;
        }

        vakX = (int)Mathf.Floor((player.transform.position.x + scale / 2) / scale);
        vakY = (int)Mathf.Floor((player.transform.position.z + scale / 2) / scale);

        if (prevX == vakX && prevY == vakY)
            return;
        prevX = vakX;
        prevY = vakY;

        playerY = player.transform.position.y;
        StartCoroutine(holdPlayerInAir());

        if (!running)
        {
            BuildTerrain();
        }
    }
    void BuildTerrain()
    {
        running = true;
        for (int i = 0; i < planes.Count; i++)
        {
            DestroyImmediate(planes[i].g.GetComponent<MeshFilter>().sharedMesh);
            //DestroyImmediate(planes[i].g.GetComponent<MeshRenderer>().material.mainTexture);
            DestroyImmediate(planes[i].g.gameObject);
        }
        planes.Clear();
        textureCounterFar = 0;
        textureCounterClose = 0;

        SpawnPlane(vakX, vakY, 25, scale, true);
        standingOn = planes[0];
        ///////////////0
        SpawnPlane(vakX - 1, vakY - 1, 25, scale);
        SpawnPlane(vakX - 0, vakY - 1, 25, scale);
        SpawnPlane(vakX + 1, vakY - 1, 25, scale);

        SpawnPlane(vakX - 1, vakY - 0, 25, scale);
        SpawnPlane(vakX + 1, vakY - 0, 25, scale);

        SpawnPlane(vakX - 1, vakY + 1, 25, scale);
        SpawnPlane(vakX - 0, vakY + 1, 25, scale);
        SpawnPlane(vakX + 1, vakY + 1, 25, scale);
        ////////////////////1
        SpawnPlane(vakX - 3, vakY - 3, 25, scale * 3);
        SpawnPlane(vakX - 0, vakY - 3, 25, scale * 3);
        SpawnPlane(vakX + 3, vakY - 3, 25, scale * 3);

        SpawnPlane(vakX - 3, vakY - 0, 25, scale * 3);
        SpawnPlane(vakX + 3, vakY - 0, 25, scale * 3);

        SpawnPlane(vakX - 3, vakY + 3, 25, scale * 3);
        SpawnPlane(vakX - 0, vakY + 3, 25, scale * 3);
        SpawnPlane(vakX + 3, vakY + 3, 25, scale * 3);
        ////////////////2
        SpawnPlane(vakX - 9, vakY - 9, 25, scale * 9);
        SpawnPlane(vakX - 0, vakY - 9, 25, scale * 9);
        SpawnPlane(vakX + 9, vakY - 9, 25, scale * 9);

        SpawnPlane(vakX - 9, vakY - 0, 25, scale * 9);
        SpawnPlane(vakX + 9, vakY - 0, 25, scale * 9);

        SpawnPlane(vakX - 9, vakY + 9, 25, scale * 9);
        SpawnPlane(vakX - 0, vakY + 9, 25, scale * 9);
        SpawnPlane(vakX + 9, vakY + 9, 25, scale * 9);
        /////////////////3
        SpawnPlane(vakX - 27, vakY - 27, 25, scale * 27);
        SpawnPlane(vakX - 0, vakY - 27, 25, scale * 27);
        SpawnPlane(vakX + 27, vakY - 27, 25, scale * 27);

        SpawnPlane(vakX - 27, vakY - 0, 25, scale * 27);
        SpawnPlane(vakX + 27, vakY - 0, 25, scale * 27);

        SpawnPlane(vakX - 27, vakY + 27, 25, scale * 27);
        SpawnPlane(vakX - 0, vakY + 27, 25, scale * 27);
        SpawnPlane(vakX + 27, vakY + 27, 25, scale * 27);
        /////////////////4
        SpawnPlane(vakX - 81, vakY - 81, 50, scale * 81);
        SpawnPlane(vakX - 0, vakY - 81, 50, scale * 81);
        SpawnPlane(vakX + 81, vakY - 81, 50, scale * 81);

        SpawnPlane(vakX - 81, vakY - 0, 50, scale * 81);
        SpawnPlane(vakX + 81, vakY - 0, 50, scale * 81);

        SpawnPlane(vakX - 81, vakY + 81, 50, scale * 81);
        SpawnPlane(vakX - 0, vakY + 81, 50, scale * 81);
        SpawnPlane(vakX + 81, vakY + 81, 50, scale * 81);
        running = false;
    }
    
    public void SpawnPlane(int x, int y, int LOD, int sizeScale, bool collidable = false)
    {
        GameObject plane;
        plane = (GameObject)Instantiate(dummy, new Vector3((x * scale) - (sizeScale / 2), 0, (y * scale) - (sizeScale / 2)), transform.rotation);
        plane.transform.localScale = new Vector3(sizeScale, 1, sizeScale);
        MeshRenderer rend = plane.AddComponent<MeshRenderer>();
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        plane.AddComponent<MeshFilter>();
        plane.AddComponent<MeshCollider>();
        plane.GetComponent<Renderer>().material = mat;

        if(sizeScale == scale)
        {
            Plane_TG p = new Plane_TG(LOD, x, y, sizeScale, plane, textureCounterClose);
            planes.Add(p);
            GenPerlinPlane(p, LOD, xo + (x * scale) - (sizeScale / 2), yo + (y * scale) - (sizeScale / 2), sizeScale, collidable);
            textureCounterClose++;
        }
        else
        {
            Plane_TG p = new Plane_TG(LOD, x, y, sizeScale, plane, textureCounterFar);
            planes.Add(p);
            GenPerlinPlane(p, LOD, xo + (x * scale) - (sizeScale / 2), yo + (y * scale) - (sizeScale / 2), sizeScale, collidable);
            textureCounterFar++;
        }
    }
    public void GenPerlinPlane(Plane_TG p, int quads, int xx, int zz, int sizeScale, bool collidable = false)
    {
        MeshFilter filter = p.g.GetComponent<MeshFilter>();
        MeshCollider collider = p.g.GetComponent<MeshCollider>();

        Vector3[] vertices = new Vector3[quads * quads * 4];
        int[] triangles = new int[quads * quads * 6];
        Vector2[] uvs = new Vector2[quads * quads * 4];
        MeshData data = new MeshData(ref vertices, ref triangles, ref uvs, collidable);
        
        float biomeNumber = GetTemperature(xx / scale * biomeScale, zz / scale * biomeScale);
        BiomeData biomeData = new BiomeData();
        biomeData.recalc(biomeNumber);

        GenMesh(quads, xx, zz, sizeScale, data, p.textureID);

        int thisTextureSize = 0;
        if (sizeScale == scale)
            thisTextureSize = TEXTURESIZE_CLOSE;
        else
            thisTextureSize = TEXTURESIZE_FAR;

        TextureData texData = new TextureData(p.textureID, thisTextureSize);
        Void _void_tex = new Void( () => GenTexture(texData, xx, zz, sizeScale, biomeData), GLOBAL_VOID_ID);
        voidbase.database.Add(GLOBAL_VOID_ID++, () => FinishTexture(texData));
        thread_texture.Feed(_void_tex);
        tasksInQueue++;

        filter.sharedMesh = new Mesh();
        FinishMesh(filter.sharedMesh, collider, data, p.textureID, thisTextureSize);
    }

    public void GenMesh(int quads, int xx, int zz, int sizeScale, MeshData data, int textureID)
    {
        BiomeData biome = new BiomeData();
        Vector3[] vertices = data.vertices;
        int[] triangles = data.triangles;
        Vector2[] uvs = data.uvs;

        int vc = 0, tc = 0, uc = 0;
        float step = ((float)sizeScale) / quads;
        float uvstep = 1f / quads;

        for (int x = 0; x < quads; x++)
        {
            for (int z = 0; z < quads; z++)
            {
                biome.recalc(GetTemperature((xx + (x + 0) * step) / scale * biomeScale, (zz + (z + 0) * step) / scale * biomeScale));
                float _0relief0 = this.relief0 * biome.relief0Mul;
                float _0relief1 = this.relief1 * biome.relief1Mul;
                float _0relief2 = this.relief2 * biome.relief2Mul;
                float _0relief3 = this.relief3 * biome.relief3Mul;
                biome.recalc(GetTemperature((xx + (x + 0) * step) / scale * biomeScale, (zz + (z + 1) * step) / scale * biomeScale));
                float _1relief0 = this.relief0 * biome.relief0Mul;
                float _1relief1 = this.relief1 * biome.relief1Mul;
                float _1relief2 = this.relief2 * biome.relief2Mul;
                float _1relief3 = this.relief3 * biome.relief3Mul;
                biome.recalc(GetTemperature((xx + (x + 1) * step) / scale * biomeScale, (zz + (z + 1) * step) / scale * biomeScale));
                float _2relief0 = this.relief0 * biome.relief0Mul;
                float _2relief1 = this.relief1 * biome.relief1Mul;
                float _2relief2 = this.relief2 * biome.relief2Mul;
                float _2relief3 = this.relief3 * biome.relief3Mul;
                biome.recalc(GetTemperature((xx + (x + 1) * step) / scale * biomeScale, (zz + (z + 0) * step) / scale * biomeScale));
                float _3relief0 = this.relief0 * biome.relief0Mul;
                float _3relief1 = this.relief1 * biome.relief1Mul;
                float _3relief2 = this.relief2 * biome.relief2Mul;
                float _3relief3 = this.relief3 * biome.relief3Mul;

                float v0l0 = 0, v0l1 = 0, v0l2 = 0, v0l3 = 0,
                        v1l0 = 0, v1l1 = 0, v1l2 = 0, v1l3 = 0,
                        v2l0 = 0, v2l1 = 0, v2l2 = 0, v2l3 = 0,
                        v3l0 = 0, v3l1 = 0, v3l2 = 0, v3l3 = 0;

                if (true)
                {
                    v0l0 = Mathf.PerlinNoise((xx + (x + 0) * step) * perlinScale0, (zz + (z + 0) * step) * perlinScale0) * _0relief0;
                    v1l0 = Mathf.PerlinNoise((xx + (x + 0) * step) * perlinScale0, (zz + (z + 1) * step) * perlinScale0) * _1relief0;
                    v2l0 = Mathf.PerlinNoise((xx + (x + 1) * step) * perlinScale0, (zz + (z + 1) * step) * perlinScale0) * _2relief0;
                    v3l0 = Mathf.PerlinNoise((xx + (x + 1) * step) * perlinScale0, (zz + (z + 0) * step) * perlinScale0) * _3relief0;

                    v0l1 = Mathf.PerlinNoise((xx + (x + 0) * step) * perlinScale1, (zz + (z + 0) * step) * perlinScale1) * _0relief1;
                    v1l1 = Mathf.PerlinNoise((xx + (x + 0) * step) * perlinScale1, (zz + (z + 1) * step) * perlinScale1) * _1relief1;
                    v2l1 = Mathf.PerlinNoise((xx + (x + 1) * step) * perlinScale1, (zz + (z + 1) * step) * perlinScale1) * _2relief1;
                    v3l1 = Mathf.PerlinNoise((xx + (x + 1) * step) * perlinScale1, (zz + (z + 0) * step) * perlinScale1) * _3relief1;
                }
                if (sizeScale <= 9 * scale)
                {
                    v0l2 = Mathf.PerlinNoise((xx + (x + 0) * step) * perlinScale2, (zz + (z + 0) * step) * perlinScale2) * _0relief2;
                    v1l2 = Mathf.PerlinNoise((xx + (x + 0) * step) * perlinScale2, (zz + (z + 1) * step) * perlinScale2) * _1relief2;
                    v2l2 = Mathf.PerlinNoise((xx + (x + 1) * step) * perlinScale2, (zz + (z + 1) * step) * perlinScale2) * _2relief2;
                    v3l2 = Mathf.PerlinNoise((xx + (x + 1) * step) * perlinScale2, (zz + (z + 0) * step) * perlinScale2) * _3relief2;
                }
                if (sizeScale <= 3 * scale)
                {
                    v0l3 = Mathf.PerlinNoise((xx + (x + 0) * step) * perlinScale3, (zz + (z + 0) * step) * perlinScale3) * _0relief3;
                    v1l3 = Mathf.PerlinNoise((xx + (x + 0) * step) * perlinScale3, (zz + (z + 1) * step) * perlinScale3) * _1relief3;
                    v2l3 = Mathf.PerlinNoise((xx + (x + 1) * step) * perlinScale3, (zz + (z + 1) * step) * perlinScale3) * _2relief3;
                    v3l3 = Mathf.PerlinNoise((xx + (x + 1) * step) * perlinScale3, (zz + (z + 0) * step) * perlinScale3) * _3relief3;
                }

                float mul0 = curve.Evaluate(v0l0 / _0relief0);
                float mul1 = curve.Evaluate(v1l0 / _1relief0);
                float mul2 = curve.Evaluate(v2l0 / _2relief0);
                float mul3 = curve.Evaluate(v3l0 / _3relief0);

                vertices[vc + 0] = new Vector3((0 + (float)x) / quads, (v0l0 * mul0) + v0l1 + v0l2 + v0l3, (0 + (float)z) / quads);
                vertices[vc + 1] = new Vector3((0 + (float)x) / quads, (v1l0 * mul1) + v1l1 + v1l2 + v1l3, (1 + (float)z) / quads);
                vertices[vc + 2] = new Vector3((1 + (float)x) / quads, (v2l0 * mul2) + v2l1 + v2l2 + v2l3, (1 + (float)z) / quads);
                vertices[vc + 3] = new Vector3((1 + (float)x) / quads, (v3l0 * mul3) + v3l1 + v3l2 + v3l3, (0 + (float)z) / quads);

                triangles[tc + 0] = vc + 0;
                triangles[tc + 1] = vc + 1;
                triangles[tc + 2] = vc + 2;
                triangles[tc + 3] = vc + 2;
                triangles[tc + 4] = vc + 3;
                triangles[tc + 5] = vc + 0;

                uvs[uc + 0] = new Vector2(uvstep * (x - 0), uvstep * (z - 0));
                uvs[uc + 1] = new Vector2(uvstep * (x - 0), uvstep * (z - -1));
                uvs[uc + 2] = new Vector2(uvstep * (x - -1), uvstep * (z - -1));
                uvs[uc + 3] = new Vector2(uvstep * (x - -1), uvstep * (z - 0));

                vc += 4;
                tc += 6;
                uc += 4;
            }
        }
    }
    public void FinishMesh(Mesh mesh, MeshCollider collider, MeshData data, int textureID, int textureSize)
    {
        mesh.Clear();
        mesh.vertices = data.vertices;
        mesh.triangles = data.triangles;
        mesh.uv = data.uvs;
        mesh.Optimize();
        mesh.RecalculateNormals();

        Material material = collider.gameObject.GetComponent<MeshRenderer>().material;
        material.EnableKeyword("_DETAIL_MULX2");
        if (textureSize == TEXTURESIZE_CLOSE)
            material.mainTexture = textures_close[textureID];
        else
            material.mainTexture = textures_far[textureID];
        material.SetTexture("_DetailAlbedoMap", detailTexture);

        if (data.collidable)
            collider.sharedMesh = mesh;
        //release memory!
        data.vertices = null;
        data.triangles = null;
        data.uvs = null;
        data = null;
    }

    public void GenTexture(TextureData texData, int xx, int zz, int sizeScale, BiomeData biome)
    {
        float step = (float)sizeScale / texData.size;

        float _relief0 = relief0 * biome.relief0Mul;
        float _relief1 = relief1 * biome.relief1Mul;

        float frac = 1f / texData.size;

        for (int x = 0; x < texData.size; x++)
        {
            for(int z = 0; z < texData.size; z++)
            {
                float v0 = Mathf.PerlinNoise((xx + (x + 0) * step) * perlinScale0, (zz + (z + 0) * step) * perlinScale0) * _relief0;
                float v1 = Mathf.PerlinNoise((xx + (x + 0) * step) * perlinScale1, (zz + (z + 0) * step) * perlinScale1) * _relief1;

                float heightFrac = curve.Evaluate((v0 + v1) / (_relief0 + _relief1));//0f - 1f
                texData.colors[z * texData.size + x] = biome.GetColor(heightFrac, GetTemperature((xx + (x + 0) * step) / scale * biomeScale, (zz + (z + 0) * step) / scale * biomeScale));

                /*TerainMaterial mat = biome.GetMaterial(heightFrac);
                float patternScale = mat.particleSize;
                float colorAddition = mat.particleColor;
                float hardness = mat.particleHardness;

                float val = Mathf.PerlinNoise(x * patternScale, z * patternScale);
                if (val > hardness)
                {
                    texData.colors[z * texData.size + x].r += colorAddition * val;
                    texData.colors[z * texData.size + x].g += colorAddition * val;
                    texData.colors[z * texData.size + x].b += colorAddition * val;
                }*/
            }
        }
    }
    public void FinishTexture(TextureData texData)
    {
        if(texData.size == TEXTURESIZE_CLOSE)
        {
            textures_close[texData.textureID].SetPixels(texData.colors);
            textures_close[texData.textureID].Apply();
        }
        else
        {
            textures_far[texData.textureID].SetPixels(texData.colors);
            textures_far[texData.textureID].Apply();
        }
        texData.colors = null;
        texData = null;
        System.GC.Collect();
    }

    public void GenDetailTexture_Sand(Texture2D tex)
    {
        float frac = 1f / 4048;
        float power0 = 0.1f, power1 = 0.2f;

        for (int x = 0; x < 4048; x++)
        {
            for(int y = 0; y < 4048; y++)
            {
                float val0 = (Mathf.PerlinNoise(x * 0.02f, y * 0.02f) * 2 - 1) * power0;
                float val1 = (Mathf.PerlinNoise(x * 0.4f, y * 0.4f) * 2 - 1) * power1;
                if (val1 > 0)
                    val1 *= 1.5f;
                tex.SetPixel(x, y, new Color(0.5f - val0 - val1, 0.5f - val0 - val1, 0.5f - val0 - val1));
            }
        }
        tex.Apply();
    }

    public float GetTemperature(float x, float y)
    {
        float val = Mathf.PerlinNoise(x, y);
        if (val < 0.19f)
            val = 0.1f;
        else if (val > 0.21f && val < 0.39f)
            val = 0.3f;
        else if (val > 0.41f && val < 0.59f)
            val = 0.5f;
        else if (val > 0.61f && val < 0.79f)
            val = 0.7f;
        else if(val > 0.81f)
            val = 0.9f;

        return val;
    }
    public float GetTemperatureWorldSpace(float x, float z)
    {
        return GetTemperature((x + xo) / scale * biomeScale, (z + yo) / scale * biomeScale);
    }
    public float GetHeight(float xx, float zz)
    {
        xx += xo;
        zz += yo;

        dummyBiome.recalc(GetTemperature(xx / scale * biomeScale, zz / scale * biomeScale));
        float _0relief0 = this.relief0 * dummyBiome.relief0Mul;
        float _0relief1 = this.relief1 * dummyBiome.relief1Mul;
        float _0relief2 = this.relief2 * dummyBiome.relief2Mul;
        float _0relief3 = this.relief3 * dummyBiome.relief3Mul;

        float v0l0 = 0, v0l1 = 0, v0l2 = 0, v0l3 = 0;

        v0l0 = Mathf.PerlinNoise(xx * perlinScale0, zz * perlinScale0) * _0relief0;
        v0l1 = Mathf.PerlinNoise(xx * perlinScale1, zz * perlinScale1) * _0relief1;
        v0l2 = Mathf.PerlinNoise(xx * perlinScale2, zz * perlinScale2) * _0relief2;
        v0l3 = Mathf.PerlinNoise(xx * perlinScale3, zz * perlinScale3) * _0relief3;

        float mul0 = curve.Evaluate(v0l0 / _0relief0);

        return (v0l0 * mul0) + v0l1 + v0l2 + v0l3;
    }

    void OnApplicationQuit()
    {
        thread_texture.Kill();
        Debug.Log("__DID_KILL_TEXTURE_THREAD");
    }

    private IEnumerator holdPlayerInAir()
    {
        yield return new WaitForFixedUpdate();
        player.transform.position = new Vector3(player.transform.position.x, playerY, player.transform.position.z);
        yield return new WaitForFixedUpdate();
        player.transform.position = new Vector3(player.transform.position.x, playerY, player.transform.position.z);
    }
}

public class Plane_TG
{
    public int lod, x, y, size, textureID;
    public GameObject g;

    public Plane_TG(int lod, int x, int y, int size, GameObject g, int textureID)
    {
        this.lod = lod;
        this.x = x;
        this.y = y;
        this.size = size;
        this.g = g;
        this.textureID = textureID;
    }
}

public class BiomeData
{
    public float relief0Mul, relief1Mul, relief2Mul, relief3Mul;
    public TerrainMaterial[] materials;
    public float[] layerHeights;

    public void recalc(float temp)
    {
        //temp                  //alt
        //0-.2 artic            //swamp, underwater, island
        //.2-.4 cold            //flat
        //.4-.6 normal          //hills
        //.6-.8 tropical        //
        //.8-1 desert           //mountains

        relief0Mul = 0;
        relief1Mul = 0;
        relief2Mul = 0;
        relief3Mul = 0;

        //2000, 200, 20, 2
        if (temp >= 0f && temp < 0.19f)
        {
            relief0Mul = 1f;
            relief1Mul = 3f;
            relief2Mul = 2.5f;
            relief3Mul = 0.001f;
        }
        else if (temp >= 0.19f && temp < 0.21f)
        {
            float percentage = (temp - 0.19f) * 50;
            relief0Mul = Mathf.Lerp(1f, 0.5f, percentage);
            relief1Mul = Mathf.Lerp(3f, 1.5f, percentage);
            relief2Mul = Mathf.Lerp(2.5f, 1f, percentage);
            relief3Mul = Mathf.Lerp(0.001f, 0.5f, percentage);
        }
        else if (temp >= 0.21f && temp < 0.39f)
        {
            relief0Mul = 0.5f;
            relief1Mul = 1.5f;
            relief2Mul = 1f;
            relief3Mul = 0.5f;
        }
        else if (temp >= 0.39f && temp < 0.41f)
        {
            float percentage = (temp - 0.39f) * 50;
            relief0Mul = Mathf.Lerp(0.5f, 1f, percentage);
            relief1Mul = Mathf.Lerp(1.5f, 1f, percentage);
            relief2Mul = Mathf.Lerp(1f, 0.5f, percentage);
            relief3Mul = Mathf.Lerp(0.5f, 0.001f, percentage);
        }
        else if (temp >= 0.41f && temp < 0.59f)
        {
            relief0Mul = 1f;
            relief1Mul = 1f;
            relief2Mul = 0.5f;
            relief3Mul = 0.001f;
        }
        else if (temp >= 0.59f && temp < 0.61f)
        {
            float percentage = (temp - 0.59f) * 50;
            relief0Mul = Mathf.Lerp(1f, 0.5f, percentage);
            relief1Mul = Mathf.Lerp(1f, 1f, percentage);
            relief2Mul = Mathf.Lerp(0.5f, 3f, percentage);
            relief3Mul = Mathf.Lerp(0.001f, 1f, percentage);
        }
        else if (temp >= 0.61f && temp < 0.79f)
        {
            relief0Mul = 0.5f;
            relief1Mul = 1f;
            relief2Mul = 3f;
            relief3Mul = 1f;
        }
        else if (temp >= 0.79f && temp < 0.81f)
        {
            float percentage = (temp - 0.79f) * 50;
            relief0Mul = Mathf.Lerp(0.5f, 0.001f, percentage);
            relief1Mul = Mathf.Lerp(1f, 1f, percentage);
            relief2Mul = Mathf.Lerp(3f, 0.001f, percentage);
            relief3Mul = Mathf.Lerp(1f, 0.001f, percentage);
        }
        else if (temp >= 0.81f && temp <= 1.0f)
        {
            relief0Mul = 0.001f;
            relief1Mul = 1f;
            relief2Mul = 0.001f;
            relief3Mul = 0.001f;
        }
    }

    public Color GetColor(float heightFrac, float temp)
    {
        if (temp >= 0f && temp < 0.19f)
        {
            if(heightFrac < 0.3f)
                return new Color(TerrainMaterialSelection.clay.red, TerrainMaterialSelection.clay.green, TerrainMaterialSelection.clay.blue);
            else
                return new Color(TerrainMaterialSelection.snow.red, TerrainMaterialSelection.snow.green, TerrainMaterialSelection.snow.blue);
        }
        else if (temp >= 0.19f && temp < 0.21f)
        {
            float percentage = (temp - 0.19f) * 50;

            if(heightFrac < 0.3f)
            {
                return new Color(TerrainMaterialSelection.clay.red, TerrainMaterialSelection.clay.green, TerrainMaterialSelection.clay.blue);
            }
            else if(heightFrac < 0.5f)
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.snow.red, TerrainMaterialSelection.stone.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.snow.green, TerrainMaterialSelection.stone.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.snow.blue, TerrainMaterialSelection.stone.blue, percentage);
                return new Color(red, green, blue);
            }
            else
            {
                return new Color(TerrainMaterialSelection.snow.red, TerrainMaterialSelection.snow.green, TerrainMaterialSelection.snow.blue);
            }
        }
        else if (temp >= 0.21f && temp < 0.39f)
        {
            if(heightFrac < 0.3f)
                return new Color(TerrainMaterialSelection.clay.red, TerrainMaterialSelection.clay.green, TerrainMaterialSelection.clay.blue);
            else if(heightFrac < 0.5f)
                return new Color(TerrainMaterialSelection.stone.red, TerrainMaterialSelection.stone.green, TerrainMaterialSelection.stone.blue);
            else
                return new Color(TerrainMaterialSelection.snow.red, TerrainMaterialSelection.snow.green, TerrainMaterialSelection.snow.blue);
        }
        else if (temp >= 0.39f && temp < 0.41f)
        {
            float percentage = (temp - 0.39f) * 50;

            if (heightFrac < 0.2f)
            {
                return new Color(TerrainMaterialSelection.clay.red, TerrainMaterialSelection.clay.green, TerrainMaterialSelection.clay.blue);
            }
            else if (heightFrac < 0.3f)
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.clay.red, TerrainMaterialSelection.grass.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.clay.green, TerrainMaterialSelection.grass.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.clay.blue, TerrainMaterialSelection.grass.blue, percentage);
                return new Color(red, green, blue);
            }
            else if (heightFrac < 0.5f)
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.stone.red, TerrainMaterialSelection.stone.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.stone.green, TerrainMaterialSelection.stone.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.stone.blue, TerrainMaterialSelection.stone.blue, percentage);
                return new Color(red, green, blue);
            }
            else if (heightFrac < 0.7f)
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.snow.red, TerrainMaterialSelection.grass.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.snow.green, TerrainMaterialSelection.grass.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.snow.blue, TerrainMaterialSelection.grass.blue, percentage);
                return new Color(red, green, blue);
            }
            else if (heightFrac < 0.9f)
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.snow.red, TerrainMaterialSelection.stone.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.snow.green, TerrainMaterialSelection.stone.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.snow.blue, TerrainMaterialSelection.stone.blue, percentage);
                return new Color(red, green, blue);
            }
            else
            {
                return new Color(TerrainMaterialSelection.snow.red, TerrainMaterialSelection.snow.green, TerrainMaterialSelection.snow.blue);
            }
        }
        else if (temp >= 0.41f && temp < 0.59f)
        {
            if(heightFrac < 0.2f)
                return new Color(TerrainMaterialSelection.clay.red, TerrainMaterialSelection.clay.green, TerrainMaterialSelection.clay.blue);
            else if(heightFrac < 0.7f)
                return new Color(TerrainMaterialSelection.grass.red, TerrainMaterialSelection.grass.green, TerrainMaterialSelection.grass.blue);
            else if(heightFrac < 0.9f)
                return new Color(TerrainMaterialSelection.stone.red, TerrainMaterialSelection.stone.green, TerrainMaterialSelection.stone.blue);
            else
                return new Color(TerrainMaterialSelection.snow.red, TerrainMaterialSelection.snow.green, TerrainMaterialSelection.snow.blue);
        }
        else if (temp >= 0.59f && temp < 0.61f)
        {
            float percentage = (temp - 0.59f) * 50;

            if(heightFrac < 0.2f)
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.clay.red, TerrainMaterialSelection.sand.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.clay.green, TerrainMaterialSelection.sand.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.clay.blue, TerrainMaterialSelection.sand.blue, percentage);
                return new Color(red, green, blue);
            }
            else if(heightFrac < 0.3f)
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.grass.red, TerrainMaterialSelection.sand.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.grass.green, TerrainMaterialSelection.sand.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.grass.blue, TerrainMaterialSelection.sand.blue, percentage);
                return new Color(red, green, blue);
            }
            else if(heightFrac < 0.6f)
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.grass.red, TerrainMaterialSelection.rock.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.grass.green, TerrainMaterialSelection.rock.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.grass.blue, TerrainMaterialSelection.rock.blue, percentage);
                return new Color(red, green, blue);
            }
            else if(heightFrac < 0.7f)
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.grass.red, TerrainMaterialSelection.stone.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.grass.green, TerrainMaterialSelection.stone.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.grass.blue, TerrainMaterialSelection.stone.blue, percentage);
                return new Color(red, green, blue);
            }
            else if(heightFrac < 0.9f)
            {
                return new Color(TerrainMaterialSelection.stone.red, TerrainMaterialSelection.stone.green, TerrainMaterialSelection.stone.blue);
            }
            else
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.snow.red, TerrainMaterialSelection.stone.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.snow.green, TerrainMaterialSelection.stone.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.snow.blue, TerrainMaterialSelection.stone.blue, percentage);
                return new Color(red, green, blue);
            }
        }
        else if (temp >= 0.61f && temp < 0.79f)
        {
            if(heightFrac < 0.3f)
                return new Color(TerrainMaterialSelection.sand.red, TerrainMaterialSelection.sand.green, TerrainMaterialSelection.sand.blue);
            else if(heightFrac < 0.6f)
                return new Color(TerrainMaterialSelection.rock.red, TerrainMaterialSelection.rock.green, TerrainMaterialSelection.rock.blue);
            else
                return new Color(TerrainMaterialSelection.stone.red, TerrainMaterialSelection.stone.green, TerrainMaterialSelection.stone.blue);
        }
        else if (temp >= 0.79f && temp < 0.81f)
        {
            float percentage = (temp - 0.79f) * 50;

            if(heightFrac < 0.3f)
            {
                return new Color(TerrainMaterialSelection.sand.red, TerrainMaterialSelection.sand.green, TerrainMaterialSelection.sand.blue);
            }
            else if(heightFrac < 0.6f)
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.rock.red, TerrainMaterialSelection.sand.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.rock.green, TerrainMaterialSelection.sand.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.rock.blue, TerrainMaterialSelection.sand.blue, percentage);
                return new Color(red, green, blue);
            }
            else if(heightFrac < 0.8f)
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.stone.red, TerrainMaterialSelection.sand.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.stone.green, TerrainMaterialSelection.sand.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.stone.blue, TerrainMaterialSelection.sand.blue, percentage);
                return new Color(red, green, blue);
            }
            else
            {
                float red = Mathf.Lerp(TerrainMaterialSelection.stone.red, TerrainMaterialSelection.rock.red, percentage);
                float green = Mathf.Lerp(TerrainMaterialSelection.stone.green, TerrainMaterialSelection.rock.green, percentage);
                float blue = Mathf.Lerp(TerrainMaterialSelection.stone.blue, TerrainMaterialSelection.rock.blue, percentage);
                return new Color(red, green, blue);
            }
            new Color(1f, 1f, 1f);
        }
        else if (temp >= 0.81f && temp <= 1.0f)
        {
            if(heightFrac < 0.8f)
                return new Color(TerrainMaterialSelection.sand.red, TerrainMaterialSelection.sand.green, TerrainMaterialSelection.sand.blue);
            else
                return new Color(TerrainMaterialSelection.rock.red, TerrainMaterialSelection.rock.green, TerrainMaterialSelection.rock.blue);
        }
        
        return new Color(0, 0, 0);
    }

    public TerrainMaterial GetMaterial(float heightFrac)
    {
        for(int i = 0; i < layerHeights.Length; i++)
        {
            if(heightFrac <= layerHeights[i])
            {
                return materials[i];
            }
        }
        return materials[0];
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public bool collidable;

    public MeshData(ref Vector3[] vertices, ref int[] triangles, ref Vector2[] uvs, bool collidable = false)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
        this.collidable = collidable;
    }
}

public class TextureData
{
    public int textureID;
    public Color[] colors;
    public int size;

    public TextureData(int textureID, int size)
    {
        this.size = size;
        this.textureID = textureID;
        colors = new Color[size * size];
    }
}

public class WorkerThread
{
    private List<Void> voids;
    private List<Void> done;
    private bool donetask = true;

    private Thread thread;
    private bool run = true;

    public WorkerThread()
    {
        voids = new List<Void>();
        done = new List<Void>();

        thread = new Thread(loop);
        thread.Start();
    }

    private void loop()
    {
        while (run)
        {
            if (donetask && voids.Count > 0)
            {
                donetask = false;
                voids[0].action();
                done.Add(voids[0]);
                voids.Remove(voids[0]);
                donetask = true;
            }
            else
            {
                for(int i = 0; i < 1000; i++)((Action)(() => { }))();//noop*10^3
            }
        }
    }

    public void Kill()
    {
        run = false;
        thread.Join();
        thread.Abort();
    }

    public void Feed(Void _void)
    {
        voids.Add(_void);
    }

    public Void returnFinishedTask()
    {
        if (done.Count > 0)
        {
            Void copy = done[0];
            done.Remove(done[0]);
            return copy;
        }
        else
            return null;
    }
}

public class VoidDataBase
{
    public Dictionary<int, Action> database;

    public VoidDataBase()
    {
        database = new Dictionary<int, Action>();
    }
}

public class Void
{
    public Action action;
    public int keyToFollowup;

    public Void(Action action, int keyToFollowup)
    {
        this.action = action;
        this.keyToFollowup = keyToFollowup;
    }
}

public class TerrainMaterial
{
    public float red, green, blue;
    public float particleSize, particleHardness, particleColor;
    public double objectPopulationSmall, objectPopulationMid, objectPopulationBig, objectPopulationGiant, objectPopulationSpecial;

    public TerrainMaterial(float red, float green, float blue, float particleSize, float particleHardness, float particleColor,
        double objectPopulationSmall, double objectPopulationMid, double objectPopulationBig, double objectPopulationGiant, double objectPopulationSpecial)
    {
        this.red = red;
        this.green = green;
        this.blue = blue;
        this.particleSize = particleSize;
        this.particleHardness = particleHardness;
        this.particleColor = particleColor;
    }
}

public class TerrainMaterialSelection
{
    //                                                          r      g     b    si   hrd    col     
    public static TerrainMaterial grass =    new TerrainMaterial(0.1f, 0.9f, 0.1f, .1f, 0.0f, +0.30f, 0.01d, 0.001d, 0.000001d, 0.00000001d, 0.0000000001d);
    public static TerrainMaterial rock =     new TerrainMaterial(0.9f, 0.4f, 0.4f, .2f, 0.8f, -0.25f, 0.01d, 0.001d, 0.000001d, 0.00000001d, 0.0000000001d);
    public static TerrainMaterial sand =     new TerrainMaterial(0.7f, 0.4f, 0.4f, .9f, 0.8f, +0.05f, 0.01d, 0.001d, 0.000001d, 0.00000001d, 0.0000000001d);
    public static TerrainMaterial clay =     new TerrainMaterial(0.5f, 0.4f, 0.4f, .1f, 0.1f, +0.09f, 0.01d, 0.001d, 0.000001d, 0.00000001d, 0.0000000001d);
    public static TerrainMaterial stone =    new TerrainMaterial(0.6f, 0.6f, 0.7f, .4f, 0.4f, +0.20f, 0.01d, 0.001d, 0.000001d, 0.00000001d, 0.0000000001d);
    public static TerrainMaterial snow =     new TerrainMaterial(1.0f, 1.0f, 1.0f, .1f, 0.0f, -0.20f, 0.30d, 0.001d, 0.000001d, 0.00000001d, 0.0000000001d);
}
