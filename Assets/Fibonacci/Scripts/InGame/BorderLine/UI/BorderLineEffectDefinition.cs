using UnityEngine;

namespace Fibonacci.InGame.BorderLine.UI
{
    [CreateAssetMenu(menuName = "Fibonacci/BorderLine/Effect Definition", fileName = "BorderLineEffectDefinition")]
    public sealed class BorderLineEffectDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private Sprite icon;
        [TextArea(3, 10)] 
        [SerializeField] private string description;
        
        public string Id => id;
        public Sprite Icon => icon;
        public string Description => description;
    }
}
