using NaughtyAttributes;
using UnityEngine;

namespace Utils
{
    public class LayerAssigner : MonoBehaviour
    {
        [SerializeField]
        [Layer]
        private string _layerName;

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer(_layerName);
            }
        }
    }
}