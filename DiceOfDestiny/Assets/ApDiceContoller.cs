using System.Collections;
using UnityEngine;

public class FourSideRollingDice : MonoBehaviour
{
    [Header("Sprite Renderers")]
    public SpriteRenderer upperRenderer;
    public SpriteRenderer lowerRenderer;

    [Header("Dice Sides")]
    public Sprite[] sideSprites;

    [Header("Roll Settings")]
    [SerializeField] private int rollCount = 6;  // 고정값
    [SerializeField] private float moveDuration = 3f;
    [SerializeField] private float inflateAmount = 0.4f;

    [Header("Movement Settings")]
    [SerializeField] private float startOffset = 2f;
    [SerializeField] private float centerRandomRange = 1f;

    private int currentIndex = 0;
    private Coroutine rollCoroutine;
    private Coroutine moveCoroutine;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        PrepareNewRoll();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PrepareNewRoll();
        }
    }

    void PrepareNewRoll()
    {
        SetStartAndEndPositions();

        currentIndex = 0;
        upperRenderer.sprite = sideSprites[currentIndex];
        lowerRenderer.sprite = sideSprites[GetNextIndex(currentIndex)];

        transform.localPosition = startPosition;

        Vector2 dir = endPosition - startPosition;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (rollCoroutine != null) StopCoroutine(rollCoroutine);
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);

        rollCoroutine = StartCoroutine(RollLoop());
        moveCoroutine = StartCoroutine(MoveOverTime());
    }

    void SetStartAndEndPositions()
    {
        Camera cam = Camera.main;
        Vector3 screenBottomRight = new Vector3(Screen.width, 0, cam.nearClipPlane);
        startPosition = cam.ScreenToWorldPoint(screenBottomRight);
        startPosition.x += startOffset;
        startPosition.y -= startOffset;
        startPosition.z = 0f;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, cam.nearClipPlane);
        Vector3 worldCenter = cam.ScreenToWorldPoint(screenCenter);
        worldCenter.z = 0f;

        float randX = Random.Range(-centerRandomRange, centerRandomRange);
        float randY = Random.Range(-centerRandomRange, centerRandomRange);

        endPosition = worldCenter + new Vector3(randX, randY, 0f);
    }

    IEnumerator MoveOverTime()
    {
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            float easeT = 1f - Mathf.Pow(1f - t, 2); // easeOutQuad

            transform.localPosition = Vector3.Lerp(startPosition, endPosition, easeT);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = endPosition;
    }

    IEnumerator RollLoop()
    {
        int currentCycle = 0;

        float minCycleDuration = 0.1f;
        float maxCycleDuration = moveDuration / rollCount * 2f;
        float growthRate = Mathf.Pow(maxCycleDuration / minCycleDuration, 1f / (rollCount - 1));
        float[] cycleDurations = new float[rollCount];
        for (int i = 0; i < rollCount; i++)
            cycleDurations[i] = minCycleDuration * Mathf.Pow(growthRate, i);

        float cycleElapsed = 0f;
        float animTime = 0f;
        float scaleElapsed = 0f;
        float startScale = 3.5f;

        // 초기 크게 설정
        transform.localScale = Vector3.one * startScale;

        float scaleDuration = moveDuration * 0.2f; // 전체 시간의 1/5

        while (currentCycle < rollCount)
        {
            float cycleDuration = cycleDurations[currentCycle];

            float delta = Time.deltaTime;
            cycleElapsed += delta;
            animTime += delta / cycleDuration;
            scaleElapsed += delta;

            // 처음 1/5 시간 동안만 스케일 빠르게 감소
            if (scaleElapsed < scaleDuration)
            {
                float scaleT = scaleElapsed / scaleDuration;
                float scaleValue = Mathf.Lerp(startScale, 1f, scaleT);
                transform.localScale = Vector3.one * scaleValue;
            }
            else
            {
                transform.localScale = Vector3.one;
            }

            float cycleProgress = animTime % 1f;

            if (currentCycle == rollCount - 1 && cycleElapsed >= cycleDuration)
            {
                upperRenderer.sprite = sideSprites[currentIndex];
                lowerRenderer.sprite = null;
                SetFinalScale();
            }
            else
            {
                RollCycleStep(cycleProgress);
            }

            if (cycleElapsed >= cycleDuration)
            {
                cycleElapsed = 0f;
                currentCycle++;

                if (currentCycle < rollCount)
                {
                    currentIndex = GetNextIndex(currentIndex);
                    upperRenderer.sprite = sideSprites[currentIndex];
                    lowerRenderer.sprite = sideSprites[GetNextIndex(currentIndex)];
                }
            }

            yield return null;
        }
    }

    void SetFinalScale()
    {
        upperRenderer.transform.localScale = Vector3.one;
        lowerRenderer.transform.localScale = Vector3.zero;

        Vector3 moveVec = Vector3.right;
        float upperWidth = upperRenderer.sprite.bounds.size.x;
        float lowerWidth = 0f; // lowerRenderer.sprite is null at final, so zero

        float distance = (upperWidth / 2f) + (lowerWidth / 2f);

        upperRenderer.transform.localPosition = moveVec * distance * 0.5f;
        lowerRenderer.transform.localPosition = -moveVec * distance * 0.5f;
    }

    int GetNextIndex(int index) => (index + 1) % sideSprites.Length;

    void RollCycleStep(float t)
    {
        Vector3 moveVec = Vector3.right;

        float ease = Mathf.SmoothStep(0f, 1f, t);
        float inflate = Mathf.Sin(ease * Mathf.PI) * inflateAmount;
        float totalScale = 1f + inflate;

        float scaleUpper = (1f - ease) * totalScale;
        float scaleLower = ease * totalScale;

        upperRenderer.transform.localScale = new Vector3(scaleUpper, 1f, 1f);
        lowerRenderer.transform.localScale = new Vector3(scaleLower, 1f, 1f);

        float upperWidth = upperRenderer.sprite.bounds.size.x * scaleUpper;
        float lowerWidth = lowerRenderer.sprite != null ? lowerRenderer.sprite.bounds.size.x * scaleLower : 0f;

        float distance = (upperWidth / 2f) + (lowerWidth / 2f);

        Vector3 upperPos = moveVec * distance * 0.5f;
        Vector3 lowerPos = -moveVec * distance * 0.5f;

        Vector3 extraOffset = moveVec * 0.5f * t;

        upperRenderer.transform.localPosition = upperPos + extraOffset;
        lowerRenderer.transform.localPosition = lowerPos + extraOffset;
    }
}
