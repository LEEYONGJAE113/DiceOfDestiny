using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIRulebook : MonoBehaviour
{
    [SerializeField] private GameObject ClassRule;
    [SerializeField] private GameObject ObstacleRule;

    [SerializeField] private GameObject RuleTextPrefab;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Instantiate(RuleTextPrefab, ClassRule.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(ClassRule.transform.GetComponent<RectTransform>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Instantiate(RuleTextPrefab, ObstacleRule.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(ObstacleRule.transform.GetComponent<RectTransform>());
        }
    }
}
