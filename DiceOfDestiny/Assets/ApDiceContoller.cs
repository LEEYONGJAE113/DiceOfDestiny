using System.Collections;
using UnityEngine;

public class ApDiceController : MonoBehaviour
{
    [Header("Sprite Renderers")]
    public SpriteRenderer expandRenderer; // 현재 면 표시, 항상 커진 상태 유지
    public SpriteRenderer contractRenderer; // 애니메이션 중인 면 (커짐 → 축소)
    public SpriteRenderer nextRenderer; // 다음 다음 면 미리 준비, 스케일 0 상태 유지

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

        StartCoroutine(ReSizeDiceOverTime());
        StartCoroutine(MoveDiceOverTime());
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

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            float easeT = 1f - Mathf.Pow(1f - t, 2);
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, easeT);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = endPosition;
    }
    IEnumerator ReSizeDiceOverTime()
    {
        float elapsed = 0f;
        float startScale = 3.5f;
        float resizeDuration = moveDuration / 5f;

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

    Vector3 prevExpandLocalPos = Vector3.zero;
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

            // 🔄 렌더러 스왑
            var temp = expand;
            expand = contract;
            contract = next;
            next = temp;

            // 🔁 위치/스케일 이전 값 복사
            expand.transform.localPosition = contract.transform.localPosition;
            expand.transform.localScale = contract.transform.localScale;

            contract.transform.localPosition = next.transform.localPosition;
            contract.transform.localScale = next.transform.localScale;

            next.transform.localPosition = temp.transform.localPosition;
            next.transform.localScale = temp.transform.localScale;

            // 🎴 스프라이트 교체
            next.sprite = sideSprites[nextNextIndex];
            contract.sprite = sideSprites[nextIndex];
            expand.sprite = sideSprites[currentIndex];

            float duration = cycleDurations[currentCycle];
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = Mathf.Clamp01(elapsed / duration);

                RollCycleStep(t, contract, expand);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // ✅ 위치 보정 (expand 렌더러의 위치 변화만큼 부모 위치를 역으로 보정)
            // 이전 localPosition → world 변환
            Vector3 prevWorld = transform.TransformPoint(prevExpandLocalPos);
            Vector3 currWorld = transform.TransformPoint(expand.transform.localPosition);

            Vector3 offset = prevWorld - currWorld;
            transform.position += offset;

            // 다음 사이클을 위해 현재 위치 저장
            prevExpandLocalPos = expand.transform.localPosition;

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

