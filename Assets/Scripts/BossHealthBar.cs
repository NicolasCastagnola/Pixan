using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    private Boss _currentBoss;
    
    [SerializeField] private Image lifeImage;
    [SerializeField] private TMP_Text bossName;
    [SerializeField] private AnimatedContainer mainContainer;

    public void Initialize(Boss boss)
    {
        _currentBoss = boss;
        _currentBoss.Health.OnDead += Terminate;
        _currentBoss.Health.OnDamage += UpdateLife;

        bossName.text = boss.BossName;
        
        mainContainer.Show();
    }
    private void UpdateLife(HealthComponent.HealthModificationReport healthModificationReport)
    {
        if (_currentBoss == null) return;
        
        lifeImage.DOFillAmount(_currentBoss.Health.CurrentPercentage, 0.1f);
    }
    private void Terminate(HealthComponent.HealthModificationReport healthModificationReport)
    {
        mainContainer.Hide();
        
        lifeImage.fillAmount = 1f;
        
        if(_currentBoss == null) return;

        _currentBoss.Health.OnDamage -= UpdateLife;
        _currentBoss = null;
    }
}
