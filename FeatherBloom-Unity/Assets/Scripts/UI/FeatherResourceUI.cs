using System.Collections.Generic;
using Events.Core;
using Protag.Abilities;
using UnityEngine;

namespace UI
{
    public class FeatherResourceUI : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _pipObjects;

        [Header("Event In")]

        [SerializeField]
        private FeatherResourceEvent _onFeathersChanged;

        private void Awake()
        {
            _onFeathersChanged.AddListener(HandleFeathersChanged);
        }

        private void OnDestroy()
        {
            _onFeathersChanged.RemoveListener(HandleFeathersChanged);
        }

        private void HandleFeathersChanged(FeatherResources.FeatherResourceState state)
        {
            int count = state.CurrentFeathers;

            for (var i = 0; i < 3; i++)
            {
                _pipObjects[i].SetActive(i < count);
            }
        }
    }
}