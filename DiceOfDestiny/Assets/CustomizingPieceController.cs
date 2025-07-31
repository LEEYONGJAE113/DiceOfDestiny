using System.Collections;
using System.Diagnostics.Contracts;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class CustomizingPieceController : MonoBehaviour
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

    GameObject stickerPrefab;


    public void Start()
    {

    }

    public void InitializeCustomizePiece()
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

            backFace.transform.localScale = new Vector3(backScale, 1f, 1f);
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
    float rotateDuration = 1.2f;

    public void OnClickLeftTurnButton()
    {
        StartCoroutine(RotateCustomizePiece(Directions.Left));
    }
    public void OnClickRightTurnButton()
    {
        StartCoroutine(RotateCustomizePiece(Directions.Right));
    }
    public void OnClickUpTurnButton()
    {
        StartCoroutine(RotateCustomizePiece(Directions.Up));
    }
    public void OnClickDownTurnButton()
    {
        StartCoroutine(RotateCustomizePiece(Directions.Down));
    }

    IEnumerator RotateCustomizePiece(Directions dir)
    {
        RectTransform expandRect = null;
        RectTransform contractRect = null;

        GameObject expandFace = null;
        GameObject contractFace = null;

        Vector3 moveVec = Vector3.zero;

        bool isHorizontal = (dir == Directions.Left || dir == Directions.Right);

        switch (dir)
        {
            case Directions.Left:
                moveVec = Vector3.right;
                expandFace = rightFace;
                contractFace = frontFace;
                break;
            case Directions.Right:
                moveVec = Vector3.left;
                expandFace = leftFace;
                contractFace = frontFace;
                break;
            case Directions.Up:
                moveVec = Vector3.down;
                expandFace = bottomFace;
                contractFace = frontFace;
                break;
            case Directions.Down:
                moveVec = Vector3.up;
                expandFace = topFace;
                contractFace = frontFace;
                break;
        }

        expandRect = expandFace.GetComponent<RectTransform>();
        contractRect = contractFace.GetComponent<RectTransform>();

        float elapsTime = 0f;      

        Vector2 expandStartPos = expandRect.anchoredPosition;
        Vector2 contractStartPos = contractRect.anchoredPosition;
        Vector2 anchorPoint = contractRect.anchoredPosition;

        Vector2 moveDirOffset = -moveVec * contractRect.rect.width / 2f; ;

        float totalOffsetAccum = 0f;

        while (elapsTime < rotateDuration)
        {
            float t = elapsTime / rotateDuration;

            float totalScale = 1f + inflateAmount * Mathf.Sin(Mathf.PI * t);

            float expandScale = Mathf.Lerp(0f, totalScale, t);
            float contractScale = Mathf.Lerp(totalScale, 0f, t);

            expandFace.transform.localScale = isHorizontal ?
                new Vector3(expandScale, 1f, 1f) :
                new Vector3(1f, expandScale, 1f);

            contractFace.transform.localScale = isHorizontal ?
                new Vector3(contractScale, 1f, 1f) :
                new Vector3(1f, contractScale, 1f);

            float expandSize = (isHorizontal ? expandRect.rect.width : expandRect.rect.height) * expandScale;
            float contractScaledSize = (isHorizontal ? contractRect.rect.width : contractRect.rect.height) * contractScale;

            float distance = (expandSize / 2f) + (contractScaledSize / 2f);
            Vector2 offset = (Vector2)(moveVec * distance * 0.5f);

            totalOffsetAccum += offset.magnitude * Time.deltaTime / (rotateDuration / Time.deltaTime);

            Vector2 driftOffset = Vector2.Lerp(Vector2.zero, moveDirOffset, t);

            Vector2 contractPos = anchorPoint + driftOffset;
            Vector2 expandPos = anchorPoint + (Vector2)(moveVec * distance) + driftOffset;

            contractRect.anchoredPosition = contractPos;
            expandRect.anchoredPosition = expandPos;

            elapsTime += Time.deltaTime;
            yield return null;
        }

        expandFace.transform.localScale = Vector3.one;
        contractFace.transform.localScale = Vector3.zero;
        expandRect.anchoredPosition = contractStartPos;


        GameObject temp;


        switch (dir)
        {
            case Directions.Left:
                temp = frontFace;
                frontFace = rightFace;
                rightFace = backFace;
                backFace = leftFace;
                leftFace = temp;
                break;

            case Directions.Right:
                temp = frontFace;
                frontFace = leftFace;
                leftFace = backFace;
                backFace = rightFace;
                rightFace = temp;
                break;

            case Directions.Up:
                temp = frontFace;
                frontFace = bottomFace;
                bottomFace = backFace;
                backFace = topFace;
                topFace = temp;
                break;

            case Directions.Down:
                temp = frontFace;
                frontFace = topFace;
                topFace = backFace;
                backFace = bottomFace;
                bottomFace = temp;
                break;
        }

        float size = frontFace.GetComponent<RectTransform>().rect.width;

        Vector2 zeroPos = new Vector2(-380, 0);

        frontFace.GetComponent<RectTransform>().anchoredPosition = zeroPos;
        backFace.GetComponent<RectTransform>().anchoredPosition = zeroPos;

        leftFace.GetComponent<RectTransform>().anchoredPosition = zeroPos + new Vector2(-size, 0);
        rightFace.GetComponent<RectTransform>().anchoredPosition = zeroPos + new Vector2(size, 0);
        topFace.GetComponent<RectTransform>().anchoredPosition = zeroPos + new Vector2(0, size);
        bottomFace.GetComponent<RectTransform>().anchoredPosition = zeroPos + new Vector2(0, -size);

    }
}
