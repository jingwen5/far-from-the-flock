using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float groundDist;

    public LayerMask terrainLayer;
    public Rigidbody rb;
    public Transform visualsTransform;
    private Animator animator;
    private Vector3 moveDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        animator = visualsTransform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 castPos = transform.position;
        castPos.y += 1;
        if (Physics.Raycast(castPos, -transform.up, out hit, Mathf.Infinity, terrainLayer))
        {
            if (hit.collider != null)
            {
                Vector3 movePos = transform.position;
                movePos.y = hit.point.y + groundDist;
                transform.position = movePos;
            }
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        moveDir = new Vector3(x, 0, y);
        animator.SetFloat("Speed", moveDir.magnitude);

        if (x != 0)
        {
            Vector3 scale = visualsTransform.localScale;
            scale.x = x < 0 ? -1f : 1f;
            visualsTransform.localScale = scale;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDir * speed;
    }
}
