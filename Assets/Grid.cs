using UnityEngine;

public class Grid : MonoBehaviour
{
    public float greenTreeLevel = .03f;
    public float birchTreeLevel = .01f;
    public float mapleTreeLevel = .02f;
    public float weedLevel = .5f;
    public float rockLevel = .1f;
    public float scale = .3f;
    public int size = 200;

    public GameObject grassPrefab;
    public GameObject[] greenTreePrefabs;
    public GameObject[] weedPrefabs;
    public GameObject[] birchTreePrefabs;
    public GameObject[] mapleTreePrefabs;
    public GameObject[] rockPrefabs;
    public GameObject[] sheepPrefabs;

    public Transform treeParent;
    public Transform weedParent;
    public Transform rockParent;
    public Transform sheepParent;
    public int sheepCount = 10;

    void Start() {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        // float[,] falloffMap = new float[size, size];
        // for (int y = 0; y < size; y++) {
        //     for (int x = 0; x < size; x++) {
        //         float xv = x / (float)size * 2 - 1;
        //         float yv = y / (float)size * 2 - 1;
        //         float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
        //         falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
        //     }
        // }
        
        SpriteRenderer sr = grassPrefab.GetComponent<SpriteRenderer>();
        Sprite sprite = sr.sprite;
        float pixels = sprite.pixelsPerUnit;
        
        float width = sprite.rect.width / pixels;
        float height = sprite.rect.height / pixels;

        Quaternion grassRotation = Quaternion.Euler(90f, 0f, 0f);
        Vector3 grassCenter = new Vector3(size / 2f, 0, size / 2f);
        GameObject grass = Instantiate(grassPrefab, grassCenter, grassRotation, transform);

        for (int y = 0; y < size; y+=2) {
            for (int x = 0; x < size; x+=2) {
                if (isPathTile(x, y)) continue;
                if (x == 0 && y == 50) continue;

                float noiseValue = noiseMap[x, y];

                if (noiseValue < birchTreeLevel)
                {
                    GameObject birchTree = PickRandom(birchTreePrefabs);
                    SpawnObject(birchTree, x, 0, y, treeParent);
                }
                else if (noiseValue < mapleTreeLevel)
                {
                    GameObject mapleTree = PickRandom(mapleTreePrefabs);
                    SpawnObject(mapleTree, x, 0, y, treeParent);
                }
                else if (noiseValue < greenTreeLevel)
                {
                    GameObject greenTree = PickRandom(greenTreePrefabs);
                    SpawnObject(greenTree, x, 0, y, treeParent);
                }
                if (noiseValue < rockLevel)
                {
                    GameObject rock = PickRandom(rockPrefabs);
                    SpawnObject(rock, x, 0, y, rockParent);
                }
                if (noiseValue < weedLevel)
                {
                    GameObject weed = PickRandom(weedPrefabs);
                    SpawnObject(weed, x, 0, y, weedParent);
                }
            }
        }

        TrySpawnSheep();
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
                SpawnObject(sheep, x, 0.07f, z, sheepParent, scaleMultiplier: 1f);
                spawned++;
            }

            attempts++;
        }
    }
}
