using UnityEngine;

public class CloudMover : MonoBehaviour
{
    public float speed = 1f;

    void Update()
    {
        // Move only in the X direction
        transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);

        // make sure cloud stays in map
        if (transform.position.x > 250f)
        {
            transform.position = new Vector3(-50f, transform.position.y, transform.position.z);
        }
    }
}