using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class StaminaComponent : MonoBehaviour
{
    public event Action OnPostureBreak;
    
    public event Action<float> OnUpdate, OnConsume, OnRecover;

    public bool CanRecover { get; private set; }

    [ShowInInspector, ReadOnly] private float _current;
    [ShowInInspector, ReadOnly] private float _currentPenaltyAfterConsumption;
    
    [SerializeField] private Entity Owner;
    
    [SerializeField] private float minStamina = 0;
    [SerializeField] private float maxStamina = 100;
    
    [SerializeField] private float staminaRecoverRatePerSecond = 1f;
    [SerializeField] private float penaltyAfterConsumption = 0.5f;
    
    public float StaminaPercentage => _current / maxStamina;
    public float Current => _current;
    public void Awake()
    {
        _current = maxStamina;
        
        Owner.Health.OnDamage += Damage;
    }
    private void OnDestroy() => Owner.Health.OnDamage -= Damage;
    private void BreakPosture() => OnPostureBreak?.Invoke();
    private void Damage(HealthComponent.HealthModificationReport obj)
    {
        if (_current < minStamina)
        {
            BreakPosture();
        }
    }
    public void ConsumeStamina(float amount)
    {
        GameManager.ShowLog("Consume Stamina: " + amount);
        
        _current -= amount;
        
        OnConsume?.Invoke(_current);
        OnUpdate?.Invoke(_current);

        CanRecover = false;
        
        _currentPenaltyAfterConsumption = penaltyAfterConsumption;
    }

    private void Update()
    {
        if (_currentPenaltyAfterConsumption > 0f)
        {
            _currentPenaltyAfterConsumption -= Time.deltaTime;
            
            if (_currentPenaltyAfterConsumption <= 0f)
            {
                CanRecover = true;
            }
        }

        if (_current < maxStamina)
        {
            Recover();
        }
    }

    private void Recover()
    {
        if (CanRecover)
        {
            _current = Mathf.Min(_current + staminaRecoverRatePerSecond * Time.deltaTime, maxStamina);
            
            OnRecover?.Invoke(_current);
            
            OnUpdate?.Invoke(_current);
        }
    }
}
