using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedGameObject;

    private BaseAction _baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        _baseAction = baseAction;
        _textMeshProUGUI.text = baseAction.GetActionName().ToUpper();

        button.onClick.AddListener(() => UnitActionSystem.Instace.SetSelectedAction(baseAction));
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instace.GetBaseAction();
        selectedGameObject.SetActive(selectedBaseAction==_baseAction);
    }
}