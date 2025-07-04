using UnityEngine;
using UnityEngine.EventSystems;

public class MouseClickSound : MonoBehaviour
{
    [SerializeField] private string soundName = "Click";

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AudioManager.Instance?.PlaySFX(soundName);            
        }
    }
}
