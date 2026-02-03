using System.Collections.Generic;
using Events.Core;
using UnityEngine;

namespace Protag.Abilities
{
    public class FeatherResourceVisuals : MonoBehaviour
    {
        [Header("Event In")]

        [SerializeField]
        private FeatherResourceEvent _onFeathersChanged;

        [SerializeField]
        private SkinnedMeshRenderer _targetMeshRenderer;

        [SerializeField]
        private Material _onMaterial;

        [SerializeField]
        private Material _onOutlineMaterial;

        [SerializeField]
        private Material _offMaterial;

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
            var mats = new List<Material>();
            _targetMeshRenderer.GetMaterials(mats);
            Debug.Log(count);
            for (var i = 0; i < count; i++)
            {
                mats[i] = _onMaterial;
                mats[i + 3] = _onOutlineMaterial;
            }

            for (int i = count; i < 3; i++)
            {
                mats[i] = _offMaterial;
                mats[i + 3] = _offMaterial;
            }

            _targetMeshRenderer.SetMaterials(mats);
        }
    }
}