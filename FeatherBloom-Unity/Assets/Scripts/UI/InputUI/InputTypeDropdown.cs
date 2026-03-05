using System;
using System.Linq;
using Input.DataTypes;
using Input.ValueSOs;
using TMPro;
using UnityEngine;
using ValueSO;

namespace UI.InputUI
{
    public class InputTypeDropdown : MonoBehaviour, IValueSOObserver
    {
        [Header("ValueSO (Read/Write)")]

        [SerializeField]
        private GameplayInputTypeValueSO _gameplayInputTypeValueSO;

        [SerializeField]
        private TMP_Dropdown _dropdown;

        private void Awake()
        {
            _dropdown.onValueChanged.AddListener(HandleValueChanged);
            _dropdown.options = Enum.GetNames(typeof(GameplayInputType))
                .Select(a => new TMP_Dropdown.OptionData(a))
                .Skip(1)
                .ToList();

            _gameplayInputTypeValueSO.AddListener(this, OnGameplayInputTypeChanged, true);
        }

        private void OnDestroy()
        {
            _dropdown.onValueChanged.RemoveListener(HandleValueChanged);
            _gameplayInputTypeValueSO.RemoveListener(this);
        }

        private void OnGameplayInputTypeChanged(GameplayInputType newValue)
        {
            _dropdown.value = (int)newValue - 1;
        }

        private void HandleValueChanged(int value)
        {
            _gameplayInputTypeValueSO.SetValue((GameplayInputType)(value + 1), this);
        }
    }
}