using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private Button endTurnButton;

    private void Start()
    {
        endTurnButton.onClick.AddListener(() => TurnSystem.Instance.NextTurn());
        UpdateTurnNumberText();
        TurnSystem.Instance.onTurnChanged += TurnSystem_OnTurnChanged;
    }
    
    private void UpdateTurnNumberText()
    {
        turnText.text = $"Turn {TurnSystem.Instance.GetTurnNumber()}".ToUpper();
        
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnNumberText();
    }
}