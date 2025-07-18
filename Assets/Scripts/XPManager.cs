using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class XPManager : MonoBehaviourPunCallbacks
{
    public int currentEnergy = 0;
    public int maxEnergy = 100;

    [SerializeField] private TextMeshProUGUI energyText;

    public static XPManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        UpdateEnergyUI();
    }

    public void AddEnergy(int amount)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
        UpdateEnergyUI();

    }

    [PunRPC]
    public void SpendEnergy(int amount)
    {
        currentEnergy = Mathf.Max(currentEnergy - amount, 0);
        UpdateEnergyUI();
    }

    private void UpdateEnergyUI()
    {
        if (energyText != null)
            energyText.text = $"Energy: {currentEnergy}/{maxEnergy}";
    }
}
