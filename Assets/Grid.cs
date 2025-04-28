using UnityEngine;

public class Grid : MonoBehaviour
{
    public float greenTreeLevel = .3f;
    public float birchTreeLevel = .25f;
    public float mapleTreeLevel = .2f;
    public float weedLevel = .8f;
    public float rockLevel = .5f;
    public float scale = .5f;
    public int size = 400;
    public int sizeOffset = 70;
    private int dirOffset = 40;

    public GameObject grassPrefab;
    public GameObject[] greenTreePrefabs;
    public GameObject[] weedPrefabs;
    public GameObject[] birchTreePrefabs;
    public GameObject[] mapleTreePrefabs;
    public GameObject[] rockPrefabs;
    public GameObject[] sheepPrefabs;
    public GameObject[] cloudPrefabs;

    public Transform treeParent;
    public Transform weedParent;
    public Transform rockParent;
    public Transform sheepParent;
    public Transform grassParent;
    public Transform cloudParent;
    public int sheepCount = 100;
    public int cloudCount = 100;

    void Start() {
        float[,] noiseMap = new float[size + sizeOffset, size + sizeOffset];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));

        for (int y = 0; y < size + sizeOffset; y++) {
            for (int x = 0; x < size + sizeOffset; x++) {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        // // Map for generating on edges
        // float[,] falloffMap = new float[size, size];
        // for (int y = 0; y < size; y++) {
        //     for (int x = 0; x < size; x++) {
        //         float xv = x / (float)size * 2 - 1;
        //         float yv = y / (float)size * 2 - 1;
        //         float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
        //         falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
        //     }
        // }
        
        // Generate grass terrain
        SpriteRenderer sr = grassPrefab.GetComponent<SpriteRenderer>();
        Sprite sprite = sr.sprite;
        float pixels = sprite.pixelsPerUnit;
        
        float width = sprite.rect.width / pixels;
        float height = sprite.rect.height / pixels;

        Quaternion grassRotation = Quaternion.Euler(90f, 0f, 0f);
        Vector3 grassCenter = new Vector3(size / 2f, 0, size / 2f);

        for (float y = 0; y < size; y+=height)
        {
            for (float x = 0; x < size; x+=width)
            {
                Vector3 pos = new Vector3(x + width / 2f, 0, y + height / 2f);
                Instantiate(grassPrefab, pos, grassRotation, grassParent);
            }
        }

        // Generate Terrain
        for (int y = 0; y < size + sizeOffset; y+=2) {
            for (int x = 0; x < size + sizeOffset; x+=2) {
                if (isPathTile(x, y)) continue;
                if (x == 0 && y == 50) continue;

                float noiseValue = noiseMap[x, y];

                if (noiseValue < mapleTreeLevel)
                {
                    GameObject mapleTree = PickRandom(mapleTreePrefabs);
                    SpawnObject(mapleTree, x - sizeOffset + dirOffset, 0, y - sizeOffset + dirOffset, treeParent);
                }
                else if (noiseValue < birchTreeLevel)
                {
                    GameObject birchTree = PickRandom(birchTreePrefabs);
                    SpawnObject(birchTree, x - sizeOffset + dirOffset, 0, y - sizeOffset + dirOffset, treeParent);
                }
                else if (noiseValue < greenTreeLevel)
                {
                    GameObject greenTree = PickRandom(greenTreePrefabs);
                    SpawnObject(greenTree, x - sizeOffset + dirOffset, 0, y - sizeOffset + dirOffset, treeParent);
                }
                if (noiseValue < rockLevel)
                {
                    GameObject rock = PickRandom(rockPrefabs);
                    SpawnObject(rock, x - sizeOffset + dirOffset, 0, y - sizeOffset + dirOffset, rockParent);
                }
                if (noiseValue < weedLevel)
                {
                    GameObject weed = PickRandom(weedPrefabs);
                    SpawnObject(weed, x - sizeOffset + dirOffset, 0, y - sizeOffset + dirOffset, weedParent);
                }
            }
        }

        TrySpawnSheep();
        TrySpawnClouds();
    }
    GameObject PickRandom(GameObject[] prefabs)
    {
        if (prefabs.Length == 0) return null;
        return prefabs[Random.Range(0, prefabs.Length)];
    }

    void SpawnObject(GameObject prefab, float x, float y, float z, Transform parent, float scaleMultiplier = 1f)
    {
        if (prefab == null) return;

        Vector3 position = new Vector3(x, 0, z);
        GameObject obj = Instantiate(prefab, position, Quaternion.identity, parent);
        obj.transform.localRotation = Quaternion.identity;

        foreach (Transform child in obj.transform)
        {
            Vector3 pos = child.localPosition;
            pos.y = y;
            child.localPosition = pos;
        }
        
        float baseScale = Random.Range(0.9f, 1.1f);

        if (parent == treeParent)
            scaleMultiplier *= Random.Range(3.8f, 4.8f);

        obj.transform.localScale *= baseScale * scaleMultiplier;
    }

    bool isPathTile(int x, int z)
    {
        return Mathf.Abs(x - z) <= 1;
    }
    
    void TrySpawnSheep()
    {
        int attempts = 0;
        int maxAttempts = 500;
        int spawned = 0;

        while (spawned < sheepCount && attempts < maxAttempts)
        {
            int x = Random.Range(0, size);
            int z = Random.Range(0, size);

            Collider[] hitColliders = Physics.OverlapSphere(new Vector3(x, 0.1f, z), 1f);
            bool blocked = false;

            foreach (var col in hitColliders)
            {
                if (col.transform.parent == treeParent || col.transform.parent == rockParent || col.transform.parent == sheepParent)
                {
                    blocked = true;
                    break;
                }
            }

            if (!blocked)
            {
                GameObject sheep = PickRandom(sheepPrefabs);
                SpawnObject(sheep, x - sizeOffset + dirOffset, 0.07f, z - sizeOffset + dirOffset, sheepParent, scaleMultiplier: 1f);
                spawned++;
            }

            attempts++;
        }
    }

    void TrySpawnClouds()
    {
        for (int i = 0; i < cloudCount; i++)
        {
            GameObject cloud = PickRandom(cloudPrefabs);

            float x = Random.Range(-sizeOffset, size + sizeOffset);
            float z = Random.Range(-sizeOffset, size + sizeOffset);

            Vector3 position = new Vector3(x, 70f, z);
            // Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
            GameObject obj = Instantiate(cloud, position, Quaternion.identity, cloudParent);

            CloudMover mover = obj.AddComponent<CloudMover>();
            mover.speed = Random.Range(0.5f, 1.5f);

            obj.transform.localScale *= Random.Range(5f,10f);
        }
    }
}
