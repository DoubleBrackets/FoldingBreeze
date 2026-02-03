using System;
using System.Linq;
using Input;
using TMPro;
using UnityEngine;

public class InputTypeDropdown : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown _dropdown;

    private void Awake()
    {
        _dropdown.onValueChanged.AddListener(HandleValueChanged);
        _dropdown.options = Enum.GetNames(typeof(GameplayInputService.GameplayInputType))
            .Select(a => new TMP_Dropdown.OptionData(a))
            .Skip(1)
            .ToList();
    }

    private void OnDestroy()
    {
        _dropdown.onValueChanged.RemoveListener(HandleValueChanged);
    }

    private void HandleValueChanged(int value)
    {
        GameplayInputService.Instance.SwitchInputType((GameplayInputService.GameplayInputType)(value + 1));
    }
}