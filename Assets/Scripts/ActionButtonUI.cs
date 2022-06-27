using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [SerializeField] private Button button;

    public void SetBaseAction(BaseAction baseAction)
    {
        _textMeshProUGUI.text = baseAction.GetActionName().ToUpper();
    }
}