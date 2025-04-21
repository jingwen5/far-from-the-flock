using UnityEngine;

public class SheepNPC : MonoBehaviour
{
    public float speed;
    public float moveDuration;
    public float idleDuration;
    private float timer;
    private bool isMoving;
    public LayerMask terrainLayer;
    public Rigidbody rb;
    public Transform visualsTransform;
    private Animator animator;
    private Vector3 moveDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = visualsTransform.GetComponent<Animator>();
        moveDuration += Random.Range(-0.5f, 0.5f);
        idleDuration += Random.Range(-0.5f, 0.5f);
        PickNewDirection();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
            rb.linearVelocity = moveDir * speed;
        else
            rb.linearVelocity = Vector3.zero;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isMoving = !isMoving;
            timer = isMoving ? moveDuration : idleDuration;
            if (isMoving) PickNewDirection();
        }

        if (rb.linearVelocity.x != 0)
        {
            Vector3 scale = visualsTransform.localScale;
            scale.x = rb.linearVelocity.x < 0 ? -1f : 1f;
            visualsTransform.localScale = scale;
        }
        if (rb.linearVelocity.z != 0)
        {
            Vector3 scale = visualsTransform.localScale;
            scale.x = rb.linearVelocity.z < 0 ? -1f : 1f;
            visualsTransform.localScale = scale;
        }
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
    }

    void PickNewDirection()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 candidate = new Vector3(Random.Range(-1f, 1f), 0.3f, Random.Range(-1f, 1f)).normalized;
            Ray ray = new Ray(transform.position, candidate);
            if (!Physics.Raycast(ray, 0.07f, terrainLayer))
            {
                moveDir = candidate;
                return;
            }
        }
        moveDir = Vector3.zero;
    }
}
