using UnityEngine;
using UnityEditor;

namespace Fibonacci.Utilities.Editor
{
    /// <summary>
    /// TextModelArrangerのカスタムInspector
    /// </summary>
    [CustomEditor(typeof(TextModelArranger))]
    public class TextModelArrangerEditor : UnityEditor.Editor
    {
        private SerializedProperty parentObjectProp;
        private SerializedProperty directionProp;
        private SerializedProperty spacingProp;
        private SerializedProperty centerAlignProp;
        private SerializedProperty startOffsetProp;
        private SerializedProperty autoDetectChildrenProp;
        private SerializedProperty autoArrangeInEditorProp;

        void OnEnable()
        {
            parentObjectProp = serializedObject.FindProperty("parentObject");
            directionProp = serializedObject.FindProperty("direction");
            spacingProp = serializedObject.FindProperty("spacing");
            centerAlignProp = serializedObject.FindProperty("centerAlign");
            startOffsetProp = serializedObject.FindProperty("startOffset");
            autoDetectChildrenProp = serializedObject.FindProperty("autoDetectChildren");
            autoArrangeInEditorProp = serializedObject.FindProperty("autoArrangeInEditor");
        }

        public override void OnInspectorGUI()
        {
            TextModelArranger arranger = (TextModelArranger)target;
            serializedObject.Update();

            // ヘッダー
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("3Dモデル配置ツール", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 親オブジェクト
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("基本設定", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(parentObjectProp, new GUIContent("親オブジェクト", "配置する文字モデルの親"));
            
            if (parentObjectProp.objectReferenceValue != null)
            {
                Transform parent = parentObjectProp.objectReferenceValue as Transform;
                EditorGUILayout.HelpBox($"子オブジェクト数: {parent.childCount}", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("親オブジェクトを設定してください", MessageType.Warning);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // 配置設定
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("配置設定", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(directionProp, new GUIContent("配置方向"));
            EditorGUILayout.PropertyField(spacingProp, new GUIContent("間隔", "モデル間の距離"));
            
            // 間隔のスライダー
            spacingProp.floatValue = EditorGUILayout.Slider("間隔 (スライダー)", spacingProp.floatValue, 0.1f, 10f);
            
            EditorGUILayout.PropertyField(centerAlignProp, new GUIContent("中央揃え", "中心を基準に配置"));
            
            if (!centerAlignProp.boolValue)
            {
                EditorGUILayout.PropertyField(startOffsetProp, new GUIContent("開始位置", "配置の開始オフセット"));
            }
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // オプション設定
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("オプション", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(autoDetectChildrenProp, new GUIContent("子オブジェクト自動検出"));
            EditorGUILayout.PropertyField(autoArrangeInEditorProp, new GUIContent("自動配置", "値変更時に自動で再配置"));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // 実行ボタン
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("実行", EditorStyles.boldLabel);
            
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("配置を実行", GUILayout.Height(30)))
            {
                Undo.RecordObject(target, "Arrange Models");
                arranger.ArrangeModels();
                EditorUtility.SetDirty(target);
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(5);

            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("位置をリセット", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("確認", "すべての子オブジェクトの位置をリセットしますか？", "はい", "キャンセル"))
                {
                    Undo.RecordObject(target, "Reset Positions");
                    arranger.ResetPositions();
                    EditorUtility.SetDirty(target);
                }
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // プレビュー情報
            if (parentObjectProp.objectReferenceValue != null)
            {
                Transform parent = parentObjectProp.objectReferenceValue as Transform;
                if (parent.childCount > 0)
                {
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.LabelField("プレビュー情報", EditorStyles.boldLabel);
                    
                    float totalLength = (parent.childCount - 1) * spacingProp.floatValue;
                    string directionStr = directionProp.enumNames[directionProp.enumValueIndex];
                    
                    EditorGUILayout.LabelField($"配置方向: {directionStr}");
                    EditorGUILayout.LabelField($"全体の長さ: {totalLength:F2}");
                    EditorGUILayout.LabelField($"配置数: {parent.childCount}個");
                    
                    EditorGUILayout.EndVertical();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}