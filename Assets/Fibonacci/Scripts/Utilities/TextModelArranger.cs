using UnityEngine;

namespace Fibonacci.Utilities
{
    /// <summary>
    /// 3D文字モデルを均等に配置するコンポーネント
    /// </summary>
    public class TextModelArranger : MonoBehaviour
    {
        [Header("配置設定")]
        [Tooltip("配置する文字モデルの親オブジェクト")]
        [SerializeField] private Transform parentObject;

        [Tooltip("配置方向")]
        [SerializeField] private ArrangeDirection direction = ArrangeDirection.Horizontal;

        [Tooltip("モデル間の間隔")]
        [SerializeField] private float spacing = 2f;

        [Tooltip("中央揃えにするか")]
        [SerializeField] private bool centerAlign = true;

        [Tooltip("開始位置のオフセット（中央揃えOFFの場合）")]
        [SerializeField] private Vector3 startOffset = Vector3.zero;

        [Header("オプション")]
        [Tooltip("子オブジェクトを自動検出")]
        [SerializeField] private bool autoDetectChildren = true;

        [Tooltip("Editorで変更時に自動配置")]
        [SerializeField] private bool autoArrangeInEditor = true;

        public enum ArrangeDirection
        {
            Horizontal,  // 横配置（X軸）
            Vertical,    // 縦配置（Y軸）
            Depth        // 奥行き配置（Z軸）
        }

        void OnValidate()
        {
            if (autoArrangeInEditor)
            {
                ArrangeModels();
            }
        }

        /// <summary>
        /// モデルを配置
        /// </summary>
        public void ArrangeModels()
        {
            if (parentObject == null)
            {
                Debug.LogWarning("親オブジェクトが設定されていません");
                return;
            }

            Transform[] children = GetChildren();
            
            if (children.Length == 0)
            {
                Debug.LogWarning("配置する子オブジェクトがありません");
                return;
            }

            ArrangeChildrenByDirection(children);
        }

        /// <summary>
        /// 子オブジェクトを取得
        /// </summary>
        private Transform[] GetChildren()
        {
            if (!autoDetectChildren || parentObject == null)
                return new Transform[0];

            int childCount = parentObject.childCount;
            Transform[] children = new Transform[childCount];

            for (int i = 0; i < childCount; i++)
            {
                children[i] = parentObject.GetChild(i);
            }

            return children;
        }

        /// <summary>
        /// 方向に応じて配置
        /// </summary>
        private void ArrangeChildrenByDirection(Transform[] children)
        {
            if (children.Length == 0) return;

            float totalLength = (children.Length - 1) * spacing;
            float startPos = centerAlign ? -totalLength / 2f : 0f;

            for (int i = 0; i < children.Length; i++)
            {
                if (children[i] == null) continue;

                Vector3 pos = centerAlign ? Vector3.zero : startOffset;
                float offset = startPos + (i * spacing);

                switch (direction)
                {
                    case ArrangeDirection.Horizontal:
                        pos.x += offset;
                        break;
                    case ArrangeDirection.Vertical:
                        pos.y += offset;
                        break;
                    case ArrangeDirection.Depth:
                        pos.z += offset;
                        break;
                }

                children[i].localPosition = pos;
            }
        }

        /// <summary>
        /// 配置をリセット
        /// </summary>
        public void ResetPositions()
        {
            Transform[] children = GetChildren();
            foreach (var child in children)
            {
                if (child != null)
                    child.localPosition = Vector3.zero;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (parentObject == null) return;

            // 親オブジェクトの位置を可視化
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(parentObject.position, 0.2f);

            // 配置範囲を可視化
            Transform[] children = GetChildren();
            if (children.Length > 1)
            {
                Gizmos.color = Color.green;
                Vector3 start = children[0].position;
                Vector3 end = children[children.Length - 1].position;
                Gizmos.DrawLine(start, end);
            }
        }
#endif
    }
}