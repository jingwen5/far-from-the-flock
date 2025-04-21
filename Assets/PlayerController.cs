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

    private AudioManager audioManager;
    private AudioSource walkLoopSource;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        animator = visualsTransform.GetComponent<Animator>();

        // walk sound effect
        walkLoopSource = gameObject.AddComponent<AudioSource>();
        walkLoopSource.clip = audioManager.walk;
        walkLoopSource.loop = true;
        walkLoopSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
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
        
        bool isMoving = moveDir.magnitude > 0.01f;

        if (isMoving)
        {
            if (!walkLoopSource.isPlaying)
                walkLoopSource.Play();
        }
        else
        {
            if (walkLoopSource.isPlaying)
                walkLoopSource.Stop();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDir * speed;
    }
}
