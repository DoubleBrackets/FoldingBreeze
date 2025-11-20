using Protag;
using UnityEngine;

namespace Events.Core
{
    [CreateAssetMenu(menuName = "Events/Core/FeatherResourceEvent")]
    public class FeatherResourceEvent : SOEvent<FeatherResources.FeatherResourceState>
    {
    }
}