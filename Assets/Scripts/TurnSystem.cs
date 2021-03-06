using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{

    private int _turnNumber=1;

    private bool _isPlayerTurn = true;

    public static TurnSystem Instance { get; private set; }

    public event EventHandler onTurnChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("there is more than one TurnSystem!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    //End Turn Button OnClick
    public void NextTurn()
    {
        _turnNumber++;
        _isPlayerTurn = !_isPlayerTurn;
        
        onTurnChanged?.Invoke(this,EventArgs.Empty);
    }

    public int GetTurnNumber()
    {
        return _turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return _isPlayerTurn;
    }


}