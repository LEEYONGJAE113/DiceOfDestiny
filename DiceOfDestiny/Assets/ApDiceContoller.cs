// DiceVisualController.cs
using System.Collections;
using UnityEngine;

public class DiceVisualController : MonoBehaviour
{
    public SpriteRenderer topRenderer;
    public SpriteRenderer frontRenderer;
    public SpriteRenderer sideRenderer;

    public Sprite[] diceFaceSprites; // 6개의 스프라이트

    private int currentTopIndex = 0;
    private int currentFrontIndex = 1;

    void Start()
    {
        StartCoroutine(AutoRollLoop());
    }

    IEnumerator AutoRollLoop()
    {
        while (true)
        {
            Vector2Int randomDir = GetRandomDirection();
            yield return RollRoutine(randomDir);
            yield return new WaitForSeconds(0.5f);
        }
    }

    Vector2Int GetRandomDirection()
    {
        Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        return directions[Random.Range(0, directions.Length)];
    }

    public void Roll(Vector2Int direction)
    {
        StartCoroutine(RollRoutine(direction));
    }

    IEnumerator RollRoutine(Vector2Int dir)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(dir.x, dir.y, 0);

        float duration = 0.8f;
        float t = 0f;

        int nextTop = (currentTopIndex + 1) % diceFaceSprites.Length;
        int nextFront = (currentFrontIndex + 1) % diceFaceSprites.Length;

        Vector3 topStartScale = topRenderer.transform.localScale;
        Vector3 frontStartScale = frontRenderer.transform.localScale;
        Vector3 sideStartScale = sideRenderer.transform.localScale;
        Vector3 sideStartEuler = sideRenderer.transform.localEulerAngles;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float curve = Mathf.Sin(t * Mathf.PI);

            transform.position = Vector3.Lerp(startPos, endPos, t);

            // 육각형처럼 늘어났다 줄어드는 효과
            Vector3 topTargetScale = new Vector3(1.2f, 0.5f, 1f);
            Vector3 sideTargetScale = new Vector3(0.9f, 1.2f, 1f);
            Vector3 sideTargetEuler = new Vector3(0, 0, -30f);

            topRenderer.transform.localScale = Vector3.Lerp(topStartScale, topTargetScale, curve);
            frontRenderer.transform.localScale = Vector3.Lerp(frontStartScale, Vector3.one, curve);
            sideRenderer.transform.localScale = Vector3.Lerp(sideStartScale, sideTargetScale, curve);

            sideRenderer.transform.localEulerAngles = Vector3.Lerp(sideStartEuler, sideTargetEuler, curve);

            if (t > 0.5f && currentTopIndex != nextTop)
            {
                currentTopIndex = nextTop;
                currentFrontIndex = nextFront;
                UpdateFaceSprites(currentTopIndex, currentFrontIndex);
            }
            yield return null;
        }

        transform.position = endPos;
        topRenderer.transform.localScale = topStartScale;
        frontRenderer.transform.localScale = frontStartScale;
        sideRenderer.transform.localScale = sideStartScale;
        sideRenderer.transform.localEulerAngles = sideStartEuler;
    }

    void UpdateFaceSprites(int topIndex, int frontIndex)
    {
        topRenderer.sprite = diceFaceSprites[topIndex];
        frontRenderer.sprite = diceFaceSprites[frontIndex];
        sideRenderer.sprite = diceFaceSprites[(topIndex + frontIndex) % diceFaceSprites.Length];
    }
}
