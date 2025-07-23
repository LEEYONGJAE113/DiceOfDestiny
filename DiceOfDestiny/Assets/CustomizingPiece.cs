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

    [SerializeField] private float foldDuration = 2.0f;


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
        if(isFolding || isFolded)
            return;
        StartCoroutine(FoldToDiceCoroutine());
    }

    IEnumerator FoldToDiceCoroutine()
    {
        isFolding = true;

        float elapsedTime = 0f;
        Vector3 targetPosition = frontFace.transform.position;

        Vector3 backFacePosition = backFace.transform.position;
        Vector3 leftFacePosition = leftFace.transform.position;
        Vector3 rightFacePosition = rightFace.transform.position;
        Vector3 topFacePosition = topFace.transform.position;
        Vector3 bottomFacePosition = bottomFace.transform.position;

        while (elapsedTime < foldDuration)
        {
            float t = elapsedTime / foldDuration;
            backFace.transform.position = Vector3.Lerp(backFacePosition, targetPosition, t);
            leftFace.transform.position = Vector3.Lerp(leftFacePosition, targetPosition, t);
            rightFace.transform.position = Vector3.Lerp(rightFacePosition, targetPosition, t);
            topFace.transform.position = Vector3.Lerp(topFacePosition, targetPosition, t);
            bottomFace.transform.position = Vector3.Lerp(bottomFacePosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isFolded = true;
        isFolding = false;
    }

    public void UnFoldToDiceNet()
    {
        if(isFolding || !isFolded)
            return;
        StartCoroutine(UnFoldToDiceNetCoroutine());
    }

    IEnumerator UnFoldToDiceNetCoroutine()
    {
        isFolding = true;

        float elapsedTime = 0f;

        Vector3 initialPosition = frontFace.transform.position;
        float width = frontFace.GetComponent<RectTransform>().rect.width;


        Vector3 backFacePosition = frontFace.transform.position + Vector3.right * width * 2f;
        Vector3 leftFacePosition = frontFace.transform.position + Vector3.left * width;
        Vector3 rightFacePosition = frontFace.transform.position + Vector3.right * width;
        Vector3 topFacePosition = frontFace.transform.position + Vector3.up * width;
        Vector3 bottomFacePosition = frontFace.transform.position + Vector3.down * width;

        while (elapsedTime < foldDuration)
        {

            float t = elapsedTime / foldDuration;
            float scale = Mathf.Lerp(1f, 0f, t);

            float offset = (1f - scale) * width / 2f;

            Vector3 topOffset = Vector3.down * offset;
            Vector3 bottomOffset = Vector3.up * offset;
            Vector3 leftOffset = Vector3.right * offset;
            Vector3 rightOffset = Vector3.left * offset;



            topFace.transform.localScale = new Vector3(1f, scale, 1f);
            bottomFace.transform.localScale = new Vector3(1f, scale, 1f);
            leftFace.transform.localScale = new Vector3(scale, 1f, 1f);
            rightFace.transform.localScale = new Vector3(scale, 1f, 1f);

            backFace.transform.position = Vector3.Lerp(initialPosition, backFacePosition + leftOffset * 2, t);
            leftFace.transform.position = Vector3.Lerp(initialPosition, leftFacePosition + leftOffset, t);
            rightFace.transform.position = Vector3.Lerp(initialPosition, rightFacePosition + rightOffset, t);
            topFace.transform.position = Vector3.Lerp(initialPosition, topFacePosition + topOffset, t);
            bottomFace.transform.position = Vector3.Lerp(initialPosition, bottomFacePosition + bottomOffset, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isFolded = false;
        isFolding = false;
    }

}
