using UnityEngine;

namespace Fibonacci.InGame.BorderLine.UI
{
    [CreateAssetMenu(menuName = "Fibonacci/BorderLine/Effect Definition", fileName = "BorderLineEffectDefinition")]
    public sealed class BorderLineEffectDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private Sprite icon;

        public string Id => id;
        public Sprite Icon => icon;
    }
}
