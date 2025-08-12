using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveCheckManager : MonoBehaviour
{
    public static SaveCheckManager Instance { get; private set; }

    private Dictionary<string, int> checkPoints = new Dictionary<string, int>();

    private List<PlayerSkills> playerSkills = new List<PlayerSkills>();

    public void AddSkill(PlayerSkills skill)
    {
        if (!playerSkills.Contains(skill))
        {
            playerSkills.Add(skill);
            GameManager.ShowLog($"Skill added: {skill}");
        }
    }
    public void RemoveSkill(PlayerSkills skill)
    {
        if (playerSkills.Contains(skill))
        {
            playerSkills.Remove(skill);
            GameManager.ShowLog($"Skill Removed: {skill}");
        }
    }
    public void CleanSkills()
    {

        PlayerPrefs.SetInt(PlayerSkills.FireSkill.ToString(), 0);
        PlayerPrefs.SetInt(PlayerSkills.IceSkill.ToString(), 0);

        playerSkills = new List<PlayerSkills>();
        GameManager.ShowLog($"Skill Cleared");
    }
    public void SaveSkills()
    {
        //clean other save, add more if needed
        //if(playerSkills.Contains(PlayerSkills.FireSkill)) PlayerPrefs.SetInt(PlayerSkills.FireSkill.ToString(), 0);
        //if(playerSkills.Contains(PlayerSkills.IceSkill))  PlayerPrefs.SetInt(PlayerSkills.IceSkill.ToString(), 0);

        foreach (var item in playerSkills)
        {
            PlayerPrefs.SetInt(item.ToString(), 1);
        }
    }

    public bool CheckIfHaveSkills(PlayerSkills skill)
    => playerSkills.Contains(skill);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddToDic(string key, int priority)
    {
        if (!checkPoints.ContainsKey(key)) checkPoints.Add(key, priority);
        else checkPoints[key] = priority;
    }

    public Dictionary<string, int> GetCheckPoints()
    {
        return checkPoints;
    }
}

public enum PlayerSkills
{
    FireSkill,
    IceSkill,
}
