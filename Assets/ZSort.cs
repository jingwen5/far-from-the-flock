using UnityEngine;

public class ZSort : MonoBehaviour
{
    public int baseSortingOrder = 1000;
    public int orderOffset = 0;
    // Update is called once per frame
    void LateUpdate()
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = Mathf.RoundToInt(-transform.position.z * 10f) + orderOffset + baseSortingOrder;
    }
}
