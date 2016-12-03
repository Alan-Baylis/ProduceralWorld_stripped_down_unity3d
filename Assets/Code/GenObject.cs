using UnityEngine;
using System.Collections;

public class GenObject : MonoBehaviour {

    public static GenObject instance;
    public static GenTerrain terrainGenerator;

    public Material stoneMat, grassMat, rockMat, struikMat, cactusMat, stamMat, leafMat;
    public GameObject bird, kikker;
    
    private GenObject()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("GenObject Singleton init done;");
        }
        else
            Destroy(gameObject);
    }

	void Start () {
        terrainGenerator = GetComponent<GenTerrain>();
    }
	
	void Update () {
	}

    public void SpawnBird()
    {
        Instantiate(bird, new Vector3(0, 2000, 0), Quaternion.identity);
        Instantiate(kikker, new Vector3(0, 2000, 0), Quaternion.identity);
    }
    public GeneratedObject MakeStone(Vector3 pos)
    {
        float sizeSmall = 2f;
        float sizeMedium = 4f;
        float sizeBig = 12f;
        float occurenceSmall = 0.7f;
        float occurenceMedium = 0.9f;
        float occurenceBig = 0.95f;

        float r = Random.value;
        float size = 0f;

        if (r < occurenceSmall)
            size = sizeSmall;
        else if (r < occurenceMedium)
            size = sizeMedium;
        else if (r < occurenceBig)
            size = sizeBig;

        return new Stone(pos.x, -1, pos.z, size);
    }
    public GeneratedObject MakeRock(Vector3 pos)
    {
        float sizeSmall = 2f;
        float sizeMedium = 4f;
        float sizeBig = 12f;
        float occurenceSmall = 0.7f;
        float occurenceMedium = 0.9f;
        float occurenceBig = 0.95f;

        float r = Random.value;
        float size = 0f;

        if (r < occurenceSmall)
            size = sizeSmall;
        else if (r < occurenceMedium)
            size = sizeMedium;
        else if (r < occurenceBig)
            size = sizeBig;

        return new Rock(pos.x, -1, pos.z, size);
    }
    public GeneratedObject MakeTree(Vector3 pos)
    {
        return new Tree(pos.x, -1, pos.z, 1);
    }
    public GeneratedObject MakeGrass(Vector3 pos)
    {
        return new Grass(pos.x, -1, pos.z, 3);
    }
    public GeneratedObject MakeCactus(Vector3 pos)
    {
        return new Cactus(pos.x, -1, pos.z, 1);
    }
    public GeneratedObject MakeStruik(Vector3 pos)
    {
        return new Struik(pos.x, -1, pos.z, 3);
    }

    public void CreateSphere(MeshFilter filter, int Nlon = 10, int Nlat = 6)
    {
        Mesh mesh = filter.mesh;
        mesh.Clear();

        float radius = 1f;
        // Longitude |||
        int nbLong = Nlon;
        // Latitude ---
        int nbLat = Nlat;

        Vector3[] vertices = new Vector3[(nbLong + 1) * nbLat + 2];
        float _pi = Mathf.PI;
        float _2pi = _pi * 2f;

        vertices[0] = Vector3.up * radius;
        for (int lat = 0; lat < nbLat; lat++)
        {
            float a1 = _pi * (float)(lat + 1) / (nbLat + 1);
            float sin1 = Mathf.Sin(a1);
            float cos1 = Mathf.Cos(a1);

            for (int lon = 0; lon <= nbLong; lon++)
            {
                float a2 = _2pi * (float)(lon == nbLong ? 0 : lon) / nbLong;
                float sin2 = Mathf.Sin(a2);
                float cos2 = Mathf.Cos(a2);

                vertices[lon + lat * (nbLong + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
            }
        }
        vertices[vertices.Length - 1] = Vector3.up * -radius;
	
        Vector3[] normales = new Vector3[vertices.Length];
        for (int n = 0; n < vertices.Length; n++)
            normales[n] = vertices[n].normalized;

        Vector2[] uvs = new Vector2[vertices.Length];
        uvs[0] = Vector2.up;
        uvs[uvs.Length - 1] = Vector2.zero;
        for (int lat = 0; lat < nbLat; lat++)
            for (int lon = 0; lon <= nbLong; lon++)
                uvs[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));

        int nbFaces = vertices.Length;
        int nbTriangles = nbFaces * 2;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        //Top Cap
        int i = 0;
        for (int lon = 0; lon < nbLong; lon++)
        {
            triangles[i++] = lon + 2;
            triangles[i++] = lon + 1;
            triangles[i++] = 0;
        }

        //Middle
        for (int lat = 0; lat < nbLat - 1; lat++)
        {
            for (int lon = 0; lon < nbLong; lon++)
            {
                int current = lon + lat * (nbLong + 1) + 1;
                int next = current + nbLong + 1;

                triangles[i++] = current;
                triangles[i++] = current + 1;
                triangles[i++] = next + 1;

                triangles[i++] = current;
                triangles[i++] = next + 1;
                triangles[i++] = next;
            }
        }

        //Bottom Cap
        for (int lon = 0; lon < nbLong; lon++)
        {
            triangles[i++] = vertices.Length - 1;
            triangles[i++] = vertices.Length - (lon + 2) - 1;
            triangles[i++] = vertices.Length - (lon + 1) - 1;
        }

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.Optimize();
    }
    public GeneratedMeshData CreateHexagonWithTop(float height, float radius)
    {
        Vector3[] vertices = new Vector3[12];
        Vector2[] uvs = new Vector2[12];
        int[] triangles = new int[48];

        vertices[0] = new Vector3(0.6f * radius, 0, 1f * radius);
        vertices[1] = new Vector3(1.3f * radius, 0, 0);
        vertices[2] = new Vector3(0.6f * radius, 0, -1f * radius);
        vertices[3] = new Vector3(-0.6f * radius, 0, -1f * radius);
        vertices[4] = new Vector3(-1.3f * radius, 0, 0);
        vertices[5] = new Vector3(-0.6f * radius, 0, 1f * radius);

        vertices[6] = new Vector3(0.6f * radius, height, 1f * radius);
        vertices[7] = new Vector3(1.3f * radius, height, 0);
        vertices[8] = new Vector3(0.6f * radius, height, -1f * radius);
        vertices[9] = new Vector3(-0.6f * radius, height, -1f * radius);
        vertices[10] = new Vector3(-1.3f * radius, height, 0);
        vertices[11] = new Vector3(-0.6f * radius, height, 1f * radius);

        uvs[0] = new Vector2(0f, 0);
        uvs[1] = new Vector2(1 / 6f, 0);
        uvs[2] = new Vector2(2 / 6f, 0);
        uvs[3] = new Vector2(3 / 6f, 0);
        uvs[4] = new Vector2(4 / 6f, 0);
        uvs[5] = new Vector2(5 / 6f, 0);
        uvs[6] = new Vector2(0f, 1);
        uvs[7] = new Vector2(1 / 6f, 1);
        uvs[8] = new Vector2(2 / 6f, 1);
        uvs[9] = new Vector2(3 / 6f, 1);
        uvs[10] = new Vector2(4 / 6f, 1);
        uvs[11] = new Vector2(5 / 6f, 1);
        //body
        triangles[0] = 0;
        triangles[1] = 6;
        triangles[2] = 5;
        triangles[3] = 6;
        triangles[4] = 11;
        triangles[5] = 5;

        triangles[6] = 1;
        triangles[7] = 7;
        triangles[8] = 0;
        triangles[9] = 7;
        triangles[10] = 6;
        triangles[11] = 0;

        triangles[12] = 2;
        triangles[13] = 8;
        triangles[14] = 1;
        triangles[15] = 8;
        triangles[16] = 7;
        triangles[17] = 1;

        triangles[18] = 3;
        triangles[19] = 9;
        triangles[20] = 2;
        triangles[21] = 9;
        triangles[22] = 8;
        triangles[23] = 2;

        triangles[24] = 4;
        triangles[25] = 10;
        triangles[26] = 3;
        triangles[27] = 10;
        triangles[28] = 9;
        triangles[29] = 3;

        triangles[30] = 5;
        triangles[31] = 11;
        triangles[32] = 4;
        triangles[33] = 11;
        triangles[34] = 10;
        triangles[35] = 4;
        //top
        triangles[36] = 9;
        triangles[37] = 11;
        triangles[38] = 6;
        triangles[39] = 9;
        triangles[40] = 6;
        triangles[41] = 8;

        triangles[42] = 6;
        triangles[43] = 7;
        triangles[44] = 8;
        triangles[45] = 9;
        triangles[46] = 10;
        triangles[47] = 11;

        GeneratedMeshData data = new GeneratedMeshData();
        data.vertices = vertices;
        data.uvs = uvs;
        data.triangles = triangles;

        return data;
    }
    public void TranslateMeshData(GeneratedMeshData data, float x, float y, float z)
    {
        Vector3 offset = new Vector3(x, y, z);

        for(int i = 0; i < data.vertices.Length; i++)
        {
            data.vertices[i] += offset;
        }
    }
    public GeneratedMeshData CombineMeshes(GeneratedMeshData data0, GeneratedMeshData data1)
    {
        GeneratedMeshData final = new GeneratedMeshData();

        int vertMax = data0.vertices.Length + data1.vertices.Length;
        int uvMax = data0.uvs.Length + data1.uvs.Length;
        int trisMax = data0.triangles.Length + data1.triangles.Length;

        final.vertices = new Vector3[vertMax];
        final.uvs = new Vector2[uvMax];
        final.triangles = new int[trisMax];

        for(int i = 0; i < data0.vertices.Length; i++)
        {
            final.vertices[i] = data0.vertices[i];
        }
        for (int i = 0; i < data0.uvs.Length; i++)
        {
            final.uvs[i] = data0.uvs[i];
        }
        for (int i = 0; i < data0.triangles.Length; i++)
        {
            final.triangles[i] = data0.triangles[i];
        }

        for (int i = 0; i < data0.vertices.Length; i++)
        {
            final.vertices[i + data0.vertices.Length] = data1.vertices[i];
        }
        for (int i = 0; i < data0.uvs.Length; i++)
        {
            final.uvs[i + data0.uvs.Length] = data1.uvs[i];
        }
        for (int i = 0; i < data0.triangles.Length; i++)
        {
            final.triangles[i + data0.triangles.Length] = data1.triangles[i];
        }

        return final;
    }
    public void BuildMeshFromData(GeneratedMeshData data, Mesh mesh)
    {
        mesh.vertices = data.vertices;
        mesh.uv = data.uvs;
        mesh.triangles = data.triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }
}

public class GeneratedObject
{
    public GameObject gameObject;

    public void Kill()
    {
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        if (filter == null)
            return;
        GameObject.Destroy(filter.sharedMesh);
    }
}
public class GeneratedMeshData
{
    public Vector3[] vertices;
    public Vector2[] uvs;
    public int[] triangles;
}

public class Stone : GeneratedObject
{
    public SphereCollider collider;
    public Renderer renderer;
    public MeshFilter filter;
    public Material mat;

    public Stone(float x, float y, float z, float size)
    {
        gameObject = new GameObject();
        gameObject.name = "stone";
        gameObject.isStatic = true;
        gameObject.transform.localScale = new Vector3(size, size, size);

        if(y != -1)
            gameObject.transform.position = new Vector3(x, y, z);
        else
            gameObject.transform.position = new Vector3(x, GenObject.terrainGenerator.GetHeight(x, z) + size / 2, z);

        renderer = gameObject.AddComponent<MeshRenderer>();
        filter = gameObject.AddComponent<MeshFilter>();
        collider = gameObject.AddComponent<SphereCollider>();
        mat = GenObject.instance.stoneMat;
        renderer.material = mat;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        GenObject.instance.CreateSphere(filter);
        ModifyStoneShape();
    }

    private void ModifyStoneShape()
    {
        float seed = Random.value * 100000;
        float strength = 0.9f;
        float freq = 7f;
        Vector3 center = gameObject.transform.position;

        Mesh mesh = filter.mesh;
        Vector3[] vertices = mesh.vertices;
        int i = 0;
        while (i < vertices.Length)
        {
            Vector3 center2vert = vertices[i] - center;
            center2vert.Normalize();
            vertices[i] += center2vert * strength * Mathf.PerlinNoise((vertices[i].x + seed) * freq, (vertices[i].z + seed) * freq);
            i++;
        }
        mesh.vertices = vertices;
    }
}
public class Rock : GeneratedObject
{
    public CapsuleCollider collider;
    public Renderer renderer;
    public MeshFilter filter;
    public Material mat;

    public Rock(float x, float y, float z, float size)
    {
        gameObject = new GameObject();
        gameObject.name = "rock";
        gameObject.isStatic = true;
        gameObject.transform.localScale = new Vector3(size, size * 3, size);

        if (y != -1)
            gameObject.transform.position = new Vector3(x, y, z);
        else
            gameObject.transform.position = new Vector3(x, GenObject.terrainGenerator.GetHeight(x, z) + size / 2, z);

        renderer = gameObject.AddComponent<MeshRenderer>();
        filter = gameObject.AddComponent<MeshFilter>();
        collider = gameObject.AddComponent<CapsuleCollider>();
        collider.height = size * 1.3f;
        collider.radius = 1f;
        mat = GenObject.instance.rockMat;
        renderer.material = mat;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        GenObject.instance.CreateSphere(filter);
        ModifyRockShape();
    }

    private void ModifyRockShape()
    {
        float seed = Random.value * 100000;
        float strength = 0.3f;
        float freq = 3f;
        Vector3 center = gameObject.transform.position;

        Mesh mesh = filter.mesh;
        Vector3[] vertices = mesh.vertices;
        int i = 0;
        while (i < vertices.Length)
        {
            Vector3 center2vert = vertices[i] - center;
            center2vert.Normalize();
            vertices[i] += center2vert * strength * Mathf.PerlinNoise((vertices[i].x + seed) * freq, (vertices[i].z + seed) * freq);
            i++;
        }
        mesh.vertices = vertices;
    }
}
public class Grass : GeneratedObject
{
    public Renderer renderer;
    public MeshFilter filter;
    public RotateGrass updateCode;

    public Grass(float x, float y, float z, float size)
    {
        gameObject = new GameObject();
        gameObject.name = "grass";
        gameObject.isStatic = true;

        gameObject.transform.localScale = new Vector3(size, size * 2, 1);
        if (y != -1)
            gameObject.transform.position = new Vector3(x, y, z);
        else
            gameObject.transform.position = new Vector3(x, GenObject.terrainGenerator.GetHeight(x, z) - 1, z);

        renderer = gameObject.AddComponent<MeshRenderer>();
        filter = gameObject.AddComponent<MeshFilter>();
        updateCode = gameObject.AddComponent<RotateGrass>();
        updateCode.player = GenObject.terrainGenerator.player.transform;

        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.material = GenObject.instance.grassMat;

        BuildGrass();
    }

    private void BuildGrass()
    {
        Mesh mesh = filter.mesh;

        Vector3[] vertices = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];

        vertices[0] = new Vector3(-0.5f, 1f, 0f);
        vertices[1] = new Vector3(0.5f, 1f, 0f);
        vertices[2] = new Vector3(0.5f, 0f, 0f);
        vertices[3] = new Vector3(-0.5f, 0f, 0f);

        uvs[0] = new Vector2(1, 1);
        uvs[1] = new Vector2(0, 1);
        uvs[2] = new Vector2(0, 0);
        uvs[3] = new Vector2(1, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 0;

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.Optimize();
    }
}
public class Struik : GeneratedObject
{
    public Renderer renderer;
    public MeshFilter filter;

    public Struik(float x, float y, float z, float size)
    {
        gameObject = new GameObject();
        gameObject.name = "struik";
        gameObject.isStatic = true;

        gameObject.transform.localScale = new Vector3(size * 2, size, size * 2);
        if (y != -1)
            gameObject.transform.position = new Vector3(x, y, z);
        else
            gameObject.transform.position = new Vector3(x, GenObject.terrainGenerator.GetHeight(x, z) + (size / 3 * 2), z);

        renderer = gameObject.AddComponent<MeshRenderer>();
        filter = gameObject.AddComponent<MeshFilter>();

        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.material = GenObject.instance.struikMat;

        GenObject.instance.CreateSphere(filter, 10, 8);
        BuildStruik();
    }

    private void BuildStruik()
    {
        Mesh mesh = filter.mesh;
        Vector3[] vertices = mesh.vertices;

        float roughness = 6f;
        float inv = 1f / roughness;

        float seed = Random.value;

        for(int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += vertices[i] * Mathf.PerlinNoise(vertices[i].x * 3844.345f * seed, vertices[i].y * 1934.093f * seed) * roughness;
            vertices[i] *= inv;
        }

        mesh.vertices = vertices;
    }
}
public class Cactus : GeneratedObject
{
    public Renderer renderer;
    public MeshFilter filter;
    public CapsuleCollider collider;

    public Cactus(float x, float y, float z, float size)
    {
        gameObject = new GameObject();
        gameObject.name = "cactus";
        gameObject.isStatic = true;

        gameObject.transform.localScale = new Vector3(size, size, size);
        if (y != -1)
            gameObject.transform.position = new Vector3(x, y, z);
        else
            gameObject.transform.position = new Vector3(x, GenObject.terrainGenerator.GetHeight(x, z) - 1f, z);

        renderer = gameObject.AddComponent<MeshRenderer>();
        filter = gameObject.AddComponent<MeshFilter>();
        collider = gameObject.AddComponent<CapsuleCollider>();
        collider.radius = 0.75f;

        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.material = GenObject.instance.cactusMat;

        BuildCactus();
    }

    private void BuildCactus()
    {
        float height = Random.Range(3.5f, 6.5f);
        collider.height = height;
        collider.center = new Vector3(0, height * 0.5f, 0);

        Mesh mesh = filter.mesh;
        GeneratedMeshData stam = GenObject.instance.CreateHexagonWithTop(height, 0.75f);
        int takken = (int)(Random.value * 5) + 3;
        for (int i = 0; i < takken; i++)
        {
            GeneratedMeshData tak0 = GenObject.instance.CreateHexagonWithTop(height * 0.4f + Random.value * 0.3f, (height * 0.05f) + (Random.value * 0.05f));
            BuildTak(tak0, new Vector3(0, height / 2 + (Random.value * height * 0.5f), 0));
        }

        GenObject.instance.BuildMeshFromData(stam, mesh);
    }

    private void BuildTak(GeneratedMeshData data, Vector3 offset)
    {
        GameObject go = new GameObject();
        gameObject.name = "cactus_child";
        go.transform.position = gameObject.transform.position + offset;
        go.transform.parent = gameObject.transform;
        go.transform.Rotate(0, Random.value * 360, 0);
        go.transform.Rotate(Random.value * 45 + 30, 0, 0);

        Renderer rend = go.AddComponent<MeshRenderer>();
        MeshFilter filt = go.AddComponent<MeshFilter>();

        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rend.material = GenObject.instance.cactusMat;

        GenObject.instance.BuildMeshFromData(data, filt.mesh);
    }
}
public class Tree : GeneratedObject
{
    public Renderer renderer;
    public MeshFilter filter;
    public CapsuleCollider collider;
    public float radius, height, branchLevel;

    public Tree(float x, float y, float z, float size)
    {
        gameObject = new GameObject();
        gameObject.name = "tree";
        gameObject.isStatic = true;

        gameObject.transform.localScale = new Vector3(size, size, size);
        if (y != -1)
            gameObject.transform.position = new Vector3(x, y, z);
        else
            gameObject.transform.position = new Vector3(x, GenObject.terrainGenerator.GetHeight(x, z) - 1f, z);

        branchLevel = 1f;

        renderer = gameObject.AddComponent<MeshRenderer>();
        filter = gameObject.AddComponent<MeshFilter>();
        collider = gameObject.AddComponent<CapsuleCollider>();

        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.material = GenObject.instance.stamMat;

        BuildTree();
    }

    private void BuildTree()
    {
        radius = Random.Range(1f, 3f);
        height = radius * (Random.value + 1) * 5 * radius;

        collider.radius = radius;
        collider.height = height;
        collider.center = new Vector3(0, height / 2, 0);

        Mesh mesh = filter.mesh;
        GeneratedMeshData data = GenObject.instance.CreateHexagonWithTop(height, radius);

        GenObject.instance.BuildMeshFromData(data, mesh);

        float branches = (int)(Random.value * (height * 0.2f));
        for(int i = 0; i < branches; i++)
        {
            float branchHeight = ((height - (height * 0.33f)) / branches) * i + (height * 0.33f);
            float partOf = (branchHeight / height);

            if (Random.value > 0.3f)
                BuildBranch(new Vector3(0, branchHeight, 0), Random.value * 360, radius / 2, height / 3);
            else
            {
                float deg = Random.value * 360;
                BuildBranch(new Vector3(0, branchHeight, 0), deg, radius / 2 * partOf, height / 3 * partOf);
                BuildBranch(new Vector3(0, branchHeight, 0), deg - 180, radius / 2 * partOf, height / 3 * partOf);
            }
        }
    }

    private void BuildBranch(Vector3 position, float deg, float radius, float height)
    {
        GameObject go = new GameObject();
        go.name = "tree_child";
        gameObject.isStatic = true;

        go.transform.parent = gameObject.transform;
        go.transform.position = gameObject.transform.position + position;
        go.transform.Rotate(0, deg, 0);
        go.transform.Rotate(Random.value * 45 + 20, 0, 0);

        MeshRenderer renderer = go.AddComponent<MeshRenderer>();
        renderer.material = GenObject.instance.stamMat;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        MeshFilter filter = go.AddComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        GeneratedMeshData data = GenObject.instance.CreateHexagonWithTop(height, radius);
        GenObject.instance.BuildMeshFromData(data, mesh);

        float size = radius * 10f;
        BuildLeafs(go.transform.position + ((go.transform.up * height) + (go.transform.up * size * radius)), new Vector3(radius * size, radius * size, radius * size));
    }

    private void BuildLeafs(Vector3 pos, Vector3 size)
    {
        GameObject go = new GameObject();
        go.name = "tree_child";
        gameObject.isStatic = true;
        go.transform.parent = gameObject.transform;
        go.transform.position = pos;
        go.transform.localScale = size;

        MeshRenderer renderer = go.AddComponent<MeshRenderer>();
        renderer.material = GenObject.instance.leafMat;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        MeshFilter filter = go.AddComponent<MeshFilter>();
        GenObject.instance.CreateSphere(filter, 15, 9);
        Mesh mesh = filter.mesh;
        Vector3[] vertices = mesh.vertices;
        float seed = Random.value;
        for(int i = 0; i < mesh.vertices.Length; i++)
        {
            vertices[i] += vertices[i] * Mathf.PerlinNoise(vertices[i].x + seed * 1000, vertices[i].z + seed * 1000) * 0.5f;
        }
        mesh.vertices = vertices;
    }
}
