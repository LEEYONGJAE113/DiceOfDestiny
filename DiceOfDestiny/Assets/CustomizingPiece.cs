using System.Collections;
using System.Diagnostics.Contracts;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class CustomizingPiece : MonoBehaviour
{
    [SerializeField] GameObject frontFace;
    [SerializeField] GameObject backFace;
    [SerializeField] GameObject leftFace;
    [SerializeField] GameObject rightFace;
    [SerializeField] GameObject topFace;
    [SerializeField] GameObject bottomFace;

    bool isFolded = false;
    bool isFolding = false;

    [SerializeField] private float foldDuration = 1.0f;
    [SerializeField] private float enlargeDuration = 1.0f;
    [SerializeField] private float gatherDuration = 0.5f;
    [SerializeField] private float space = 30;


    public void Start()
    {
        if (isFolded)
        {

        }
        else
        {

        }
    }

    public void Toggle()
    {
        if (isFolded)
        {
            UnFoldToDiceNet();
        }
        else
        {
            FoldToDice();

        }
    }

    public void FoldToDice()
    {
        if (isFolding || isFolded)
            return;
        isFolding = true;
        StartCoroutine(GatherFaces());        
    }

    public void UnFoldToDiceNet()
    {
        if (isFolding || !isFolded)
            return;
        isFolding = true;
        StartCoroutine(CompressOverTime());
        StartCoroutine(MoveOverTime(true));
    }

    IEnumerator MoveOverTime(bool reverse = false)
    {
        float elapsedTime = 0f;
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 initialPosition;
        Vector3 targetPosition;

        if (!reverse)
        {
            initialPosition = transform.localPosition;
            targetPosition = initialPosition + Vector3.right * 550f;
        }
        else
        {
            initialPosition = transform.localPosition;
            targetPosition = transform.localPosition + Vector3.left * 550f;
        }

        while (elapsedTime < enlargeDuration)
        {
            float t = elapsedTime / enlargeDuration;
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition; // Ensure final position is set
    }

    IEnumerator EnlargeOverTime()
    {
        float elapsedTime = 0f;
        while (elapsedTime < enlargeDuration)
        {
            float t = elapsedTime / enlargeDuration;
            float scale = Mathf.Lerp(1f, 2f, t); // Scale from 1 to 2
            transform.localScale = new Vector3(scale, scale, scale);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(2f, 2f, 2f); // Ensure final scale is set

        isFolded = true;
        isFolding = false;
    }

    IEnumerator CompressOverTime()
    {
        float elapsedTime = 0f;
        while (elapsedTime < enlargeDuration)
        {
            float t = elapsedTime / enlargeDuration;
            float scale = Mathf.Lerp(2f, 1f, t); // Scale from 1 to 2
            transform.localScale = new Vector3(scale, scale, scale);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(1f, 1f, 1f); // Ensure final scale is set

        StartCoroutine(UnFoldToDiceNetCoroutine());
    }
    
    IEnumerator GatherFaces()
    {
        float elapsedTime = 0f;

        float size = frontFace.GetComponent<RectTransform>().rect.width;

        Vector3 backFacePosition = backFace.transform.position;
        Vector3 leftFacePosition = leftFace.transform.position;
        Vector3 rightFacePosition = rightFace.transform.position;
        Vector3 topFacePosition = topFace.transform.position;
        Vector3 bottomFacePosition = bottomFace.transform.position;

        Vector3 backFaceTargetPosition = frontFace.transform.position + Vector3.right * size * 2f;
        Vector3 leftFaceTargetPosition = frontFace.transform.position + Vector3.left * size;
        Vector3 rightFaceTargetPosition = frontFace.transform.position + Vector3.right * size;
        Vector3 topFaceTargetPosition = frontFace.transform.position + Vector3.up * size;
        Vector3 bottomTargetFacePosition = frontFace.transform.position + Vector3.down * size;

        while (elapsedTime < foldDuration)
        {
            float t = elapsedTime / foldDuration;

            backFace.transform.position = Vector3.Lerp(backFacePosition, backFaceTargetPosition, t);
            leftFace.transform.position = Vector3.Lerp(leftFacePosition, leftFaceTargetPosition, t);
            rightFace.transform.position = Vector3.Lerp(rightFacePosition, rightFaceTargetPosition, t);
            topFace.transform.position = Vector3.Lerp(topFacePosition, topFaceTargetPosition, t);
            bottomFace.transform.position = Vector3.Lerp(bottomFacePosition, bottomTargetFacePosition, t);


            elapsedTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(FoldToDiceCoroutine());
    }

    IEnumerator ScatterFaces()
    {
        float elapsedTime = 0f;

        float size = frontFace.GetComponent<RectTransform>().rect.width;

        Vector3 backFacePosition = backFace.transform.position;
        Vector3 leftFacePosition = leftFace.transform.position;
        Vector3 rightFacePosition = rightFace.transform.position;
        Vector3 topFacePosition = topFace.transform.position;
        Vector3 bottomFacePosition = bottomFace.transform.position;

        Vector3 backFaceTargetPosition = backFace.transform.position + Vector3.right * space * 2;
        Vector3 leftFaceTargetPosition = leftFace.transform.position + Vector3.left * space;
        Vector3 rightFaceTargetPosition = rightFace.transform.position + Vector3.right * space;
        Vector3 topFaceTargetPosition = topFace.transform.position + Vector3.up * space;
        Vector3 bottomTargetFacePosition = bottomFace.transform.position + Vector3.down * space;

        while (elapsedTime < foldDuration)
        {
            float t = elapsedTime / foldDuration;

            backFace.transform.position = Vector3.Lerp(backFacePosition, backFaceTargetPosition, t);
            leftFace.transform.position = Vector3.Lerp(leftFacePosition, leftFaceTargetPosition, t);
            rightFace.transform.position = Vector3.Lerp(rightFacePosition, rightFaceTargetPosition, t);
            topFace.transform.position = Vector3.Lerp(topFacePosition, topFaceTargetPosition, t);
            bottomFace.transform.position = Vector3.Lerp(bottomFacePosition, bottomTargetFacePosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isFolding = false;
        isFolded = false;
    }


    IEnumerator FoldToDiceCoroutine()
    {
        float elapsedTime = 0f;

        float size = frontFace.GetComponent<RectTransform>().rect.width;

        Vector3 backFacePosition = backFace.transform.position;
        Vector3 leftFacePosition = leftFace.transform.position;
        Vector3 rightFacePosition = rightFace.transform.position;
        Vector3 topFacePosition = topFace.transform.position;
        Vector3 bottomFacePosition = bottomFace.transform.position;

        Vector3 backFaceTargetPosition = frontFace.transform.position;
        Vector3 leftFaceTargetPosition = frontFace.transform.position + Vector3.left * size / 2;
        Vector3 rightFaceTargetPosition = frontFace.transform.position + Vector3.right * size / 2;
        Vector3 topFaceTargetPosition = frontFace.transform.position + Vector3.up * size / 2;
        Vector3 bottomFaceTargetPosition = frontFace.transform.position + Vector3.down * size / 2;

        while (elapsedTime < foldDuration)
        {
            float t = elapsedTime / foldDuration;

            float scale = Mathf.Lerp(1f, 0f, t);
            float backScale = Mathf.Lerp(1f, 0f, t * 2f); // Back face scales faster to hide it

            topFace.transform.localScale = new Vector3(1f, scale, 1f);
            bottomFace.transform.localScale = new Vector3(1f, scale, 1f);
            leftFace.transform.localScale = new Vector3(scale, 1f, 1f);
            rightFace.transform.localScale = new Vector3(scale, 1f, 1f);

            backFace.transform.position = Vector3.Lerp(backFacePosition, backFaceTargetPosition, t);
            leftFace.transform.position = Vector3.Lerp(leftFacePosition, leftFaceTargetPosition, t);
            rightFace.transform.position = Vector3.Lerp(rightFacePosition, rightFaceTargetPosition, t);
            topFace.transform.position = Vector3.Lerp(topFacePosition, topFaceTargetPosition , t);
            bottomFace.transform.position = Vector3.Lerp(bottomFacePosition, bottomFaceTargetPosition, t);            

            elapsedTime += Time.deltaTime;
            yield return null;           
        }

        backFace.transform.localScale = new Vector3(1f,0f,1f);
        topFace.transform.localScale = new Vector3(1f, 0f, 1f);
        bottomFace.transform.localScale = new Vector3(1f, 0f, 1f);
        leftFace.transform.localScale = new Vector3(0f, 1f, 1f);
        rightFace.transform.localScale = new Vector3(0f, 1f, 1f);

        backFace.transform.position = backFaceTargetPosition;
        leftFace.transform.position = leftFaceTargetPosition;
        rightFace.transform.position = rightFaceTargetPosition;
        topFace.transform.position = topFaceTargetPosition;
        bottomFace.transform.position = bottomFaceTargetPosition;

        StartCoroutine(MoveOverTime());
        StartCoroutine(EnlargeOverTime());
    }

    IEnumerator UnFoldToDiceNetCoroutine()
    {
        isFolding = true;

        float elapsedTime = 0f;

        float size = frontFace.GetComponent<RectTransform>().rect.width;

        Vector3 backFacePosition = frontFace.transform.position + Vector3.right * size * 2f;
        Vector3 leftFacePosition = frontFace.transform.position + Vector3.left * size;
        Vector3 rightFacePosition = frontFace.transform.position + Vector3.right * size;
        Vector3 topFacePosition = frontFace.transform.position + Vector3.up * size;
        Vector3 bottomFacePosition = frontFace.transform.position + Vector3.down * size;

        Vector3 backFaceInitialPosition = frontFace.transform.position;
        Vector3 leftFaceInitialPosition = frontFace.transform.position + Vector3.left * size / 2;
        Vector3 rightFaceInitialPosition = frontFace.transform.position + Vector3.right * size / 2;
        Vector3 topFaceInitialPosition = frontFace.transform.position + Vector3.up * size / 2;
        Vector3 bottomFaceInitialPosition = frontFace.transform.position + Vector3.down * size / 2;
        backFace.transform.position = backFaceInitialPosition;

        while (elapsedTime < foldDuration)
        {

            float t = elapsedTime / foldDuration;

            float scale = Mathf.Lerp(0f, 1f, t);
            float backScale = Mathf.Lerp(0f, 1f, Mathf.InverseLerp(0.5f, 1f, t));

            backFace.transform.localScale = new Vector3(backScale, 1f, 1f);
            topFace.transform.localScale = new Vector3(1f, scale, 1f);
            bottomFace.transform.localScale = new Vector3(1f, scale, 1f);
            leftFace.transform.localScale = new Vector3(scale, 1f, 1f);
            rightFace.transform.localScale = new Vector3(scale, 1f, 1f);

            backFace.transform.position = Vector3.Lerp(backFaceInitialPosition, backFacePosition, t);
            leftFace.transform.position = Vector3.Lerp(leftFaceInitialPosition, leftFacePosition, t);
            rightFace.transform.position = Vector3.Lerp(rightFaceInitialPosition, rightFacePosition, t);
            topFace.transform.position = Vector3.Lerp(topFaceInitialPosition, topFacePosition, t);
            bottomFace.transform.position = Vector3.Lerp(bottomFaceInitialPosition, bottomFacePosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        backFace.transform.localScale = new Vector3(1f, 1f, 1f);
        topFace.transform.localScale = new Vector3(1f, 1f, 1f);
        bottomFace.transform.localScale = new Vector3(1f, 1f, 1f);
        leftFace.transform.localScale = new Vector3(1f, 1f, 1f);
        rightFace.transform.localScale = new Vector3(1f, 1f, 1f);

        backFace.transform.position = backFacePosition;
        leftFace.transform.position = leftFacePosition;
        rightFace.transform.position = rightFacePosition;
        topFace.transform.position = topFacePosition;
        bottomFace.transform.position = bottomFacePosition;

        StartCoroutine(ScatterFaces());
    }


    // 회전 영역
    float inflateAmount = 0.3f;
    float rotateDuration = 4f;

    public void OnClickLeftTurnButton()
    {
        StartCoroutine(RotateCustomizePiece(leftFace, frontFace));
    }
    public void OnClickRightTurnButton()
    {
        StartCoroutine(RotateCustomizePiece(rightFace, frontFace));
    }
    public void OnClickTopTurnButton()
    {
    }
    public void OnClickBottomTurnButton()
    {
    }

    IEnumerator RotateCustomizePiece(GameObject expandFace, GameObject contractFace)
    {
        float elapsTime = 0f;

        Vector3 moveVec = Vector3.left;

        RectTransform expandRect = expandFace.GetComponent<RectTransform>();
        RectTransform contractRect = contractFace.GetComponent<RectTransform>();

        Vector2 expandStartPos = expandRect.anchoredPosition;
        Vector2 contractStartPos = contractRect.anchoredPosition;
        Vector2 anchorPoint = contractRect.anchoredPosition;

        Vector2 moveRightOffset = Vector2.right * contractRect.rect.width / 2f; ;

        float totalOffsetAccum = 0f;

        while (elapsTime < rotateDuration)
        {
            float t = elapsTime / rotateDuration;

            float totalScale = 1f + inflateAmount * Mathf.Sin(Mathf.PI * t);

            float expandScale = Mathf.Lerp(0f, totalScale, t);
            float contractScale = Mathf.Lerp(totalScale, 0f, t);

            expandFace.transform.localScale = new Vector3(expandScale, 1f, 1f);
            contractFace.transform.localScale = new Vector3(contractScale, 1f, 1f);

            float expandWidth = expandRect.rect.width * expandScale;
            float contractWidth = contractRect.rect.width * contractScale;

            float distance = (expandWidth / 2f) + (contractWidth / 2f);
            Vector2 offset = (Vector2)(moveVec * distance * 0.5f);

            totalOffsetAccum += offset.magnitude * Time.deltaTime / (rotateDuration / Time.deltaTime);

            Vector2 correction = moveVec * totalOffsetAccum;

            Vector2 driftOffset = Vector2.Lerp(Vector2.zero, moveRightOffset, t);

            float contractRightEdge = contractWidth / 2f;
            float expandLeftEdge = expandWidth / 2f;

            contractRect.anchoredPosition = anchorPoint + driftOffset;
            expandRect.anchoredPosition = anchorPoint + (Vector2)(moveVec * (contractRightEdge + expandLeftEdge)) + driftOffset;

            elapsTime += Time.deltaTime;
            yield return null;
        }

        expandFace.transform.localScale = Vector3.one;
        contractFace.transform.localScale = Vector3.zero;
        expandRect.anchoredPosition = contractStartPos;

    }
}
