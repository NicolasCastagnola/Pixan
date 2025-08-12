using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class Canvas_Debug : BaseMonoSingleton<Canvas_Debug>
{
    private bool isOpen = false;
    [SerializeField] private AnimatedContainer mainContainer;

    private Player _player;
    private bool playerIsInvulnerable = false;
    [SerializeField] private TMP_Text invulnerableStateDisplay;
    
    public TMP_Dropdown dropdown;

    protected override void Start()
    {
        Initialize(FindObjectOfType<Player>());
        
        base.Start();
    }
    public void Initialize(Player player)
    {
        _player = player;
        invulnerableStateDisplay.text = $"State: {_player.Health.IsInvulnerable}";

        PopulateTeletransportDropdown();
    }
    public void GoToSelectedTPOption()
    {
        LevelController.Instance.TpPlayerToTargetArea(dropdown.options[dropdown.value].text);
    }
    
    public void LoadNextLevel() => LevelController.Instance.NextLevel();
    public void GetAllKeys() => LevelController.Instance.GetAllKeys();
    private void PopulateTeletransportDropdown()
    {
        if(LevelController.Instance.AreaKeyAreas.Count == 0) return;
        
        dropdown.ClearOptions();
        dropdown.AddOptions(LevelController.Instance.AreaKeyAreas.Select(kvp => kvp.AreaName).ToList());
    }
    public void Close()
    {
        Cursor.lockState = CursorLockMode.Confined;
        isOpen = !isOpen;
        mainContainer.Hide();
    }
    public void UnlockAllBonfires() => BonfireManager.Instance.UnlockAll();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isOpen) mainContainer.Hide();
            else mainContainer.Show();
            isOpen = !isOpen;
            Cursor.lockState = isOpen ? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }
    [Button]
    public void TogglePlayerInvulnerable()
    {
        playerIsInvulnerable = !playerIsInvulnerable;
        invulnerableStateDisplay.text = $"State: {playerIsInvulnerable}";
        invulnerableStateDisplay.color = playerIsInvulnerable ? Color.green : Color.red;
        _player.SetInvulnerability(playerIsInvulnerable);
    }
    public void WinLevel() => LevelController.Instance.Win();
    public void LoseLevel() => LevelController.Instance.Lose();
}
