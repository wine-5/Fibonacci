using System.Collections.Generic;
using UnityEngine;

namespace Fibonacci.InGame.BorderLine.UI
{
    [CreateAssetMenu(menuName = "Fibonacci/BorderLine/Effect Catalog", fileName = "BorderLineEffectCatalog")]
    public sealed class BorderLineEffectCatalog : ScriptableObject
    {
        [SerializeField] private List<BorderLineEffectDefinition> effects = new();

        public IReadOnlyList<BorderLineEffectDefinition> Effects => effects;
    }
}
