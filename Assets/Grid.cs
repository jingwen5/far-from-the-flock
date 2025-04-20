using UnityEngine;

public class Grid : MonoBehaviour
{
    public float forestLevel = 0.4f;
    public float treeLevel = .5f;
    public float weedLevel = .1f;
    public float rockLevel = .15f;
    public float scale = .1f;
    public int size = 100;

    public GameObject grassPrefab;
    public GameObject[] greenTreePrefabs;
    public GameObject[] weedPrefabs;
    public GameObject[] birchTreePrefabs;
    public GameObject[] mapleTreePrefabs;
    public GameObject[] rockPrefabs;

    public Transform treeParent;
    public Transform weedParent;
    public Transform rockParent;

    void Start() {
        float xOffset = Random.Range(-10000f, 10000f);
        float yOffset = Random.Range(-10000f, 10000f);
        
        SpriteRenderer sr = grassPrefab.GetComponent<SpriteRenderer>();
        Sprite sprite = sr.sprite;
        float pixels = sprite.pixelsPerUnit;
        
        float width = sprite.rect.width / pixels;
        float height = sprite.rect.height / pixels;

        Quaternion grassRotation = Quaternion.Euler(90f, 0f, 0f);
        Vector3 grassCenter = new Vector3(size / 2f, 0, size / 2f);
        GameObject grass = Instantiate(grassPrefab, grassCenter, grassRotation, transform);

        float scaleX = size / width;
        float scaleZ = size / height;
        grass.transform.localScale = new Vector3(scaleX, 1f, scaleZ);

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                Vector3 pos = new Vector3(x, 0, y);

                float noise = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                float forestDensityNoise = Mathf.PerlinNoise((x + xOffset + 2500f) * 0.02f, (y + yOffset + 2500f) * 0.02f);

                if (x % 2 == 0 && y % 2 == 0 && noise > treeLevel && forestDensityNoise > forestLevel &&
                    Random.value < 0.8f)
                {
                    float treeTypeNoise = Mathf.PerlinNoise((x + xOffset + 5000f) * 0.05f, (y + yOffset + 5000f) * 0.05f);

                    GameObject tree;
                    if (treeTypeNoise < 0.33f)
                        tree = PickRandom(greenTreePrefabs);
                    else if (treeTypeNoise < 0.66f)
                        tree = PickRandom(birchTreePrefabs);
                    else
                        tree = PickRandom(mapleTreePrefabs);
                    Vector3 treePos = pos + new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));
                    SpawnObject(tree, treePos, treeParent);
                }
                if (noise > rockLevel && Random.value < 0.9f)
                {
                    GameObject rock = PickRandom(rockPrefabs);
                    SpawnObject(rock, pos + Vector3.up * 0.3f, rockParent);
                }
                if (noise > weedLevel && Random.value < 0.9f)
                {
                    GameObject weed = PickRandom(weedPrefabs);
                    SpawnObject(weed, pos + Vector3.up * 0.2f, weedParent);
                }
            }
        }
    }

    GameObject PickRandom(GameObject[] prefabs)
    {
        if (prefabs.Length == 0) return null;
        return prefabs[Random.Range(0, prefabs.Length)];
    }

    void SpawnObject(GameObject prefab, Vector3 position, Transform parent)
    {
        if (prefab == null) return;
        position.y = 0;
        GameObject obj = Instantiate(prefab, position, Quaternion.identity, parent);
        obj.transform.localRotation = Quaternion.identity;

        foreach (Transform child in obj.transform)
        {
            Vector3 pos = child.localPosition;
            pos.y = 0;
            child.localPosition = pos;
        }
        
        float scale = Random.Range(0.9f, 1.1f);

        if (parent == treeParent)
            scale *= Random.Range(3.8f, 4.8f);

        obj.transform.localScale *= scale;
    }
}
