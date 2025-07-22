using UnityEngine;
using System.Collections;
using System.Diagnostics.Contracts;

public class ApDiceController : MonoBehaviour
{
    [Header("Sprite Renderers")]
    public SpriteRenderer expandRenderer; 
    public SpriteRenderer contractRenderer; 
    public SpriteRenderer nextRenderer; 

    [Header("Dice Sides")]
    public Sprite[] sideSprites;

    [Header("Roll Settings")]
    [SerializeField] private int rollCountMin = 3;
    [SerializeField] private int rollCountMax = 10;
    [SerializeField] private float inflateAmount = 0.4f;

    [Header("Movement Settings")]
    [SerializeField] private float startOffset = 2f;
    [SerializeField] private float centerRandomRange = 1f;
    [SerializeField] private float moveDuration = 3f;

    private int currentIndex = 0;
    private int rollCount = 6;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DiceRoll();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // For testing purposes, roll the dice when space is pressed
            DiceRoll2();
        }
    }

    void DiceRoll2()
    {
        StartCoroutine(RollDiceSpiral());
    }

    IEnumerator RotateDiceOverTime()
    {
        float elapsed = 0f;
        float resizeDuration = moveDuration * 4;
        float degree = 0f;

        while (elapsed < resizeDuration)
        {
            transform.rotation = Quaternion.Euler(0, 0, degree); 
            elapsed += Time.deltaTime;
            degree += 5f;
            yield return null;
        }
    }

    IEnumerator RollDiceSpiral()
    {
        int currentCycle = 0;
        currentIndex = 0;
        int nextIndex = GetNextIndex(currentIndex);
        int nextNextIndex = GetNextIndex(nextIndex);

        float pos = 0f;

        SpriteRenderer expand = expandRenderer;
        SpriteRenderer contract = contractRenderer;
        SpriteRenderer next = nextRenderer;

        expand.sprite = sideSprites[currentIndex];
        expand.transform.localScale = Vector3.one;
        expand.transform.localPosition = Vector3.zero;

        contract.sprite = sideSprites[nextIndex];
        contract.transform.localScale = Vector3.zero;
        contract.transform.localPosition = Vector3.zero;

        next.sprite = sideSprites[nextNextIndex];
        next.transform.localScale = Vector3.zero;
        next.transform.localPosition = Vector3.zero;

        float baseDuration = moveDuration / rollCount;

        while (currentCycle < rollCount)
        {
            currentIndex = nextIndex;
            nextIndex = nextNextIndex;
            nextNextIndex = GetNextIndex(nextNextIndex);

            var temp = expand;
            expand = contract;
            contract = next;
            next = temp;

            expand.transform.localPosition = contract.transform.localPosition;
            expand.transform.localScale = contract.transform.localScale;

            contract.transform.localPosition = next.transform.localPosition;
            contract.transform.localScale = next.transform.localScale;

            next.transform.localPosition = Vector3.zero;
            next.transform.localScale = Vector3.zero;

            next.sprite = sideSprites[nextNextIndex];
            contract.sprite = sideSprites[nextIndex];
            expand.sprite = sideSprites[currentIndex];

            float duration = baseDuration * (1f + 0.5f * (currentCycle / (float)rollCount));
            float elapsed = 0f;

            float startAngle = pos * 30f; // 이전 프레임 각도
            float endAngle = startAngle + Mathf.Lerp(180f, 90f, currentCycle / (float)rollCount); // 한 사이클당 회전 각도 줄이기


            while (elapsed < duration)
            {
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease-out

                RollCycleStep(easedT, contract, expand);

                

                transform.position = new Vector3(pos, pos, 0);
                pos += 0.1f * (1f - t); ;
                float angle = Mathf.Lerp(startAngle, endAngle, easedT);
                transform.rotation = Quaternion.Euler(0, 0, angle); // 회전 효과

                elapsed += Time.deltaTime;
                yield return null;
            }

            currentCycle++;
        }
    }

    void DiceRoll()
    {
        SetDiceRoute();
        rollCount = Random.Range(rollCountMin, rollCountMax + 1);

        currentIndex = 0;
        expandRenderer.sprite = sideSprites[currentIndex];
        contractRenderer.sprite = sideSprites[GetNextIndex(currentIndex)];

        transform.localPosition = startPosition;

        Vector2 dir = endPosition - startPosition;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        StartCoroutine(MoveDiceOverTime());
        StartCoroutine(ReSizeDiceOverTime());
        StartCoroutine(RollCycle());
    }

    void SetDiceRoute()
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

    IEnumerator MoveDiceOverTime()
    {
        float elapsed = 0f;
        float duration = moveDuration;

        Vector3 totalDelta = endPosition - startPosition / 1.5f;
        float totalDistance = totalDelta.magnitude;
        Vector3 direction = totalDelta.normalized;

        float prevT = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easeT = 1f - Mathf.Pow(1f - t, 2); // Ease-out

            float movedDistance = easeT * totalDistance;
            float deltaDistance = movedDistance - (prevT * totalDistance);

            // 현재 스케일 고려하여 이동량 보정
            float scaleFactor = transform.localScale.magnitude; // 스케일이 크면 적게 움직이니까 더 보정
            Vector3 adjustedDelta = direction * deltaDistance * scaleFactor * 1.2f;

            transform.position += adjustedDelta;

            prevT = easeT;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ReSizeDiceOverTime()
    {
        float elapsed = 0f;
        float startScale = 3.5f;
        float resizeDuration = moveDuration / 8f;

        while (elapsed < resizeDuration)
        {
            float scaleT = elapsed / resizeDuration;
            float scaleValue = Mathf.Lerp(startScale, 1f, scaleT);

            transform.localScale = Vector3.one * scaleValue;

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }

    IEnumerator RollCycle()
    {
        int currentCycle = 0;

        float minCycleDuration = 0.1f;
        float maxCycleDuration = moveDuration / rollCount * 2f;
        float growthRate = Mathf.Pow(maxCycleDuration / minCycleDuration, 1f / (rollCount - 1));
        float[] cycleDurations = new float[rollCount];

        for (int i = 0; i < rollCount; i++)
            cycleDurations[i] = minCycleDuration * Mathf.Pow(growthRate, i);

        currentIndex = 0;
        int nextIndex = GetNextIndex(currentIndex);
        int nextNextIndex = GetNextIndex(nextIndex);

        SpriteRenderer expand = expandRenderer;
        SpriteRenderer contract = contractRenderer;
        SpriteRenderer next = nextRenderer;

        expand.sprite = sideSprites[currentIndex];
        expand.transform.localScale = Vector3.one;
        expand.transform.localPosition = Vector3.zero;

        contract.sprite = sideSprites[nextIndex];
        contract.transform.localScale = Vector3.zero;
        contract.transform.localPosition = Vector3.zero;

        next.sprite = sideSprites[nextNextIndex];
        next.transform.localScale = Vector3.zero;
        next.transform.localPosition = Vector3.zero;

        while (currentCycle < rollCount)
        {
            currentIndex = nextIndex;
            nextIndex = nextNextIndex;
            nextNextIndex = GetNextIndex(nextNextIndex);

            var temp = expand;
            expand = contract;
            contract = next;
            next = temp;

            expand.transform.localPosition = contract.transform.localPosition;
            expand.transform.localScale = contract.transform.localScale;

            contract.transform.localPosition = next.transform.localPosition;
            contract.transform.localScale = next.transform.localScale;

            next.transform.localPosition = Vector3.zero;
            next.transform.localScale = Vector3.zero;

            next.sprite = sideSprites[nextNextIndex];
            contract.sprite = sideSprites[nextIndex];
            expand.sprite = sideSprites[currentIndex];

            float duration = cycleDurations[currentCycle];
            float elapsed = 0f;

            float length = 0.67f;  // 당기고 싶은 거리
            float angleDegrees = transform.eulerAngles.z;  // 현재 회전 각도 (0~360)
            float oppositeAngle = (angleDegrees + 180f) % 360f;  // 반대 방향 각도

            // 라디안 변환
            float angleRad = oppositeAngle * Mathf.Deg2Rad;

            // 방향 벡터 계산 및 길이 곱하기
            Vector2 offset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * length;

            // 위치에 적용 (월드 좌표 기준)
            transform.position += new Vector3(offset.x, offset.y, 0);

            while (elapsed < duration)
            {
                float t = Mathf.Clamp01(elapsed / duration);

                RollCycleStep(t, contract, expand);

                elapsed += Time.deltaTime;
                yield return null;
            }

            currentCycle++;
        }
    }

    private void RollCycleStep(float t, SpriteRenderer expand, SpriteRenderer contract)
    {
        Vector3 moveVec = Vector3.left;

        float totalScale = 1f + inflateAmount * Mathf.Sin(Mathf.PI * t);

        float expandScale = Mathf.Lerp(0f, totalScale, t);
        float contractScale = Mathf.Lerp(totalScale, 0f, t);

        expand.transform.localScale = new Vector3(expandScale, 1f, 1f);
        contract.transform.localScale = new Vector3(contractScale, 1f, 1f);

        float expandWidth = expand.sprite.bounds.size.x * expandScale;
        float contractWidth = contract.sprite != null ? contract.sprite.bounds.size.x * contractScale : 0f;
        float distance = (expandWidth / 2f) + (contractWidth / 2f);

        expand.transform.localPosition = moveVec * distance * 0.5f;
        contract.transform.localPosition = -moveVec * distance * 0.5f;
    }

    int GetNextIndex(int index) => (index + 1) % sideSprites.Length;
}