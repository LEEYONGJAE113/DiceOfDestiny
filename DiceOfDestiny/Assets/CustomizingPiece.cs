using System.Collections;
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
        StartCoroutine(FoldToDiceCoroutine());
    }

    IEnumerator FoldToDiceCoroutine()
    {
        isFolding = true;
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

        isFolded = true;
        isFolding = false;
    }

    public void UnFoldToDiceNet()
    {
        if (isFolding || !isFolded)
            return;
        StartCoroutine(UnFoldToDiceNetCoroutine());
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

        isFolded = false;
        isFolding = false;
    }

}
