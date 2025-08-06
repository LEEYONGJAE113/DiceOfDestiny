using UnityEngine;
using System.Collections.Generic;

public enum StatusType
{
    Stun,
    Disease
    // 필요하면 계속 추가
}

public class StatusEffectController : MonoBehaviour
{
    private Dictionary<StatusType, StatusEffect> statusEffects;

    private void Awake()
    {
        statusEffects = new Dictionary<StatusType, StatusEffect>
        {
            { StatusType.Stun,    new StatusEffect("기절") },
            { StatusType.Disease, new StatusEffect("질병") }
        };
    }

    public void SetStatus(StatusType type, int turn)
    {
        if (statusEffects.TryGetValue(type, out StatusEffect effect))
        {
            effect.Set(true, turn);
        }
    }
    public int GetRemainingTurn(StatusType type)
    {
        if (statusEffects.TryGetValue(type, out StatusEffect effect) && effect.IsActive)
        {
            return effect.RemainingTurn;
        }

        return 0;
    }

    public bool IsStatusActive(StatusType type)
    {
        return statusEffects.TryGetValue(type, out StatusEffect effect) && effect.IsActive;
    }

    public void EndTurn()
    {
        foreach (var kvp in statusEffects)
        {
            if(kvp.Value.IsActive)
                kvp.Value.DecreaseTurn();
        }
    }
}

[System.Serializable]
public class StatusEffect
{
    public bool IsActive { get; private set; }
    public int RemainingTurn { get; private set; }

    private string effectName;

    public StatusEffect(string name)
    {
        effectName = name;
        IsActive = false;
        RemainingTurn = 0;
    }

    public void Set(bool active, int turn)
    {
        IsActive = active;
        RemainingTurn = turn;
    }

    public void DecreaseTurn()
    {
        RemainingTurn--;
        Debug.Log($"남은 {effectName} 턴: {RemainingTurn}");

        if (RemainingTurn <= 0)
        {
            RemainingTurn = 0;
            IsActive = false;
            Debug.Log($"{effectName}이(가) 풀렸습니다.");
        }
    }
}