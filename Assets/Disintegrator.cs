using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Disintegrator : MonoBehaviour
{
    public Grid grid;
    public PlayerController player;
    public float idleTimeThreshold = 5f;
    private float idleTimer = 0f;
    private bool disintegrating = false;
    private bool fullyDisintegrated = false;
    public Button restartButton;
    private GameObject titleCloudInstance;
    public GameObject titleCloudPrefab;
    
    private bool titleCloudCleared = false;
    private Vector3 startingCloudPos = new Vector3(51.5f, 1f, 48f);
    private Vector3 savedCamPos;

    void Start()
    {
        restartButton.gameObject.SetActive(false);
        restartButton.transform.position = new Vector3(-1000f, -1000f, 0f);
        titleCloudInstance = Instantiate(titleCloudPrefab, startingCloudPos, Quaternion.Euler(22f, 0f, 0f));
        titleCloudInstance.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (fullyDisintegrated)
        {
            player.enabled = false;
            return;
        }

        if (player.IsMoving())
        {
            if (!titleCloudCleared)
            {
                titleCloudCleared = true;
                StartCoroutine(SlideTitleCloudUp());
            }
            else if (disintegrating)
                StopDisintegration();
            idleTimer = 0f;
        }
        else
        {
            if (!titleCloudCleared) return;

            idleTimer += Time.deltaTime;

            if (idleTimer > idleTimeThreshold && !disintegrating)
                StartDisintegration();
        }
    }

    // If player idle for too long, start disintegration
    void StartDisintegration()
    {
        disintegrating = true;
        StartCoroutine(FadeWorldOut());
    }

    // Fade back in if player starts moving again
    void StopDisintegration()
    {
        disintegrating = false;
        idleTimer = 0f;
        if (!fullyDisintegrated)
        {
            StartCoroutine(FadeWorldIn());
        }
    }

    IEnumerator SlideTitleCloudUp()
    {
        if (titleCloudInstance == null)
            yield break;

        Vector3 startPos = titleCloudInstance.transform.position;
        Vector3 endPos = startPos + new Vector3(0f, 30f, 0f);

        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = EaseOutBack(t);
            titleCloudInstance.transform.position = Vector3.Lerp(startPos, endPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        titleCloudInstance.SetActive(false);
    }

    IEnumerator SlideTitleCloudDown()
    {
        Vector3 endPos = new Vector3(savedCamPos.x, 0f, savedCamPos.z);
        Vector3 startPos = titleCloudInstance.transform.position;
        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
            titleCloudInstance.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        titleCloudInstance.transform.position = endPos;

        Vector3 originalScale = titleCloudInstance.transform.localScale;

        titleCloudInstance.transform.localScale = new Vector3 (originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);
        yield return new WaitForSeconds(0.1f);

        titleCloudInstance.transform.localScale = originalScale;
    }

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3) + c1 * Mathf.Pow(t - 1f, 2);
    }

    IEnumerator FadeWorldOut()
    {
        // Gradually reduce alpha of sprites
        // If timer gets long enough, fully disintegrate and show title cloud
        float duration = 20f;
        float fadeDuration = 5f;
        float elapsed = 0f;
        var allSprites = new System.Collections.Generic.List<SpriteRenderer>();
        foreach (Transform parent in new Transform[] { grid.treeParent, grid.rockParent, grid.weedParent, grid.sheepParent, grid.cloudParent, grid.grassParent })
        {
            foreach(SpriteRenderer sr in parent.GetComponentsInChildren<SpriteRenderer>())
            {
                allSprites.Add(sr);
            }
        }

        float[] spriteTimers = new float[allSprites.Count];
        for (int i = 0; i < spriteTimers.Length; i++)
        {
            spriteTimers[i] = Random.Range(0f, 2f);
        }

        while (disintegrating && elapsed < duration)
        {
            for (int i = 0; i < allSprites.Count; i++)
            {
                SpriteRenderer sr = allSprites[i];
                if (sr == null) continue;

                float t = elapsed - spriteTimers[i];
                float spriteAlpha = Mathf.Clamp01(1f - t / fadeDuration);

                spriteAlpha += 0.3f * Mathf.Sin(elapsed * 4f + i);
                spriteAlpha = Mathf.Clamp01(spriteAlpha);

                Color color = sr.color;
                color.a = spriteAlpha;
                sr.color = color;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Full fade complete
        fullyDisintegrated = true;
        savedCamPos = Camera.main.transform.position;
        ShowTitleCloud();
    }

    IEnumerator FadeWorldIn()
    {
        float elapsed = 0f;
        while (!disintegrating && elapsed < 1f)
        {
            SetWorldAlpha(elapsed / 1f);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void SetWorldAlpha(float alpha)
    {
        foreach (Transform parent in new Transform[] { grid.treeParent, grid.rockParent, grid.weedParent, grid.sheepParent, grid.cloudParent, grid.grassParent })
        {
            foreach(SpriteRenderer sr in parent.GetComponentsInChildren<SpriteRenderer>())
            {
                Color color = sr.color;
                color.a = alpha;
                sr.color = color;
            }
        }
    }

    void ShowTitleCloud()
    {        
        // Hide everything and spawn title screen cloud prefab
        grid.HideWorld();

        if (titleCloudInstance == null)
        {
            titleCloudInstance = Instantiate(titleCloudPrefab, new Vector3(savedCamPos.x, 30f, savedCamPos.z), Quaternion.identity);
        }
        else
        {
            titleCloudInstance.transform.position = new Vector3(savedCamPos.x,30f, savedCamPos.z);
            titleCloudInstance.SetActive(true);
        }

        var sr = titleCloudInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 10000;
            Color color = sr.color;
            color.r = Mathf.Min(color.r * 1.5f, 1f);
            color.g = Mathf.Min(color.g * 1.5f, 1f);
            color.b = Mathf.Min(color.b * 1.5f, 1f);
            color.a = 1f;
            sr.color = color;
        }

        StartCoroutine(SlideTitleCloudDown());

        Vector3 screenPos = Camera.main.WorldToScreenPoint(savedCamPos);
        restartButton.transform.position = screenPos;
        // Show the restart button
        restartButton.gameObject.SetActive(true);
    }

    public void OnRestartButtonClicked()
    {
        restartButton.gameObject.SetActive(false);
        restartButton.transform.position = new Vector3(-1000f, -1000f, 0f);
        StartCoroutine(ResetWorld());
    }

    public IEnumerator ResetWorld()
    {
        yield return new WaitForSeconds(1f);
        
        // Destroy old world, reload new
        foreach (Transform parent in new Transform[] { grid.treeParent, grid.rockParent, grid.weedParent, grid.sheepParent, grid.cloudParent, grid.grassParent })
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }

        if (titleCloudInstance != null)
        {
            Destroy(titleCloudInstance);
        }

        yield return null;

        grid.RegenerateWorld();
        player.enabled = true;
        fullyDisintegrated = false;
        disintegrating = false;
        idleTimer = 0f;
        titleCloudCleared = false;
    }
}
