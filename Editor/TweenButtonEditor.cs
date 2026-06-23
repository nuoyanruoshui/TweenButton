#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
[CustomEditor(typeof(TweenButton)), CanEditMultipleObjects]
public class TweenButtonEditor : Editor
{
    SerializedProperty m_TweenButtonTypeProperty;
    SerializedProperty m_InteractableProperty;
    SerializedProperty m_TargetGraphicProperty;
    SerializedProperty m_TargetRendererProperty;
    SerializedProperty m_TransitionProperty;
    SerializedProperty m_ColorBlockProperty;
    SerializedProperty m_SpriteStateProperty;
    SerializedProperty m_AnimTriggerProperty;
    SerializedProperty m_NavigationProperty;
    SerializedProperty m_ScaleBlockProperty;
    SerializedProperty m_TweenAnimProperty;
    SerializedProperty m_OnClickEventProperty;
    SerializedProperty m_AutoRollbackProperty;
    GUIContent m_VisualizeNavigation = EditorGUIUtility.TrTextContent("Visualize", "显示可选择 UI 元素之间的导航流程。");
    private static bool s_ShowNavigation = false;
    private static string s_ShowNavigationKey = "SelectableEditor.ShowNavigation";
    private TweenButton tweenButton => target as TweenButton;

    private void OnEnable()
    {
        m_TweenButtonTypeProperty = serializedObject.FindProperty("m_TweenButtonType");
        m_InteractableProperty = serializedObject.FindProperty("m_Interactable");
        m_TargetGraphicProperty = serializedObject.FindProperty("m_TargetGraphic");
        m_TargetRendererProperty = serializedObject.FindProperty("m_TargetRenderer");
        m_TransitionProperty = serializedObject.FindProperty("m_TransitionType");
        m_ColorBlockProperty = serializedObject.FindProperty("m_Colors");
        m_SpriteStateProperty = serializedObject.FindProperty("m_SpriteState");
        m_AnimTriggerProperty = serializedObject.FindProperty("m_AnimationTriggers");
        m_ScaleBlockProperty = serializedObject.FindProperty("m_ScaleBlock");
        m_TweenAnimProperty = serializedObject.FindProperty("m_TweenAnimBlock");
        m_OnClickEventProperty = serializedObject.FindProperty("m_OnClick");
        m_NavigationProperty = serializedObject.FindProperty("m_Navigation");
        m_AutoRollbackProperty = serializedObject.FindProperty("AutoRollBack");

        s_ShowNavigation = EditorPrefs.GetBool(s_ShowNavigationKey);
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_TweenButtonTypeProperty);
        EditorGUILayout.PropertyField(m_InteractableProperty);
        EditorGUILayout.PropertyField(m_AutoRollbackProperty);
        EditorGUILayout.PropertyField(m_TransitionProperty);

        ++EditorGUI.indentLevel;
        {
            switch (m_TweenButtonTypeProperty.enumValueIndex)
            {
                case 0:
                    EditorGUILayout.PropertyField(m_TargetGraphicProperty);
                    var renderer = m_TargetRendererProperty.objectReferenceValue as Renderer;
                    if (renderer == null)
                    {
                        renderer = (target as TweenButton).GetComponent<Renderer>();
                    }
                    break;
                case 1:
                    EditorGUILayout.PropertyField(m_TargetRendererProperty);
                    var graphic = m_TargetGraphicProperty.objectReferenceValue as Graphic;
                    if (graphic == null)
                    {
                        graphic = (target as TweenButton).GetComponent<Graphic>();
                    }
                    if ((target as GameObject).GetComponent<Collider>() == null && (target as GameObject).GetComponent<Collider2D>() == null)
                    {
                        EditorGUILayout.HelpBox("World模式下需要一个 Collider 组件，否则无法触发点击", MessageType.Warning);
                    }
                    break;
            }
            switch (m_TransitionProperty.enumValueIndex)
            {
                case 1:
                    EditorGUILayout.PropertyField(m_ColorBlockProperty);
                    break;
                case 2:
                    EditorGUILayout.PropertyField(m_SpriteStateProperty);
                    break;
                case 3:
                    EditorGUILayout.PropertyField(m_AnimTriggerProperty);
                    break;
                case 4:
                    EditorGUILayout.PropertyField(m_ScaleBlockProperty);
                    break;
                case 5:
                    EditorGUILayout.PropertyField(m_TweenAnimProperty);
                    break;
            }


            if (tweenButton.transform is RectTransform)
            {
                if (m_TweenButtonTypeProperty.enumValueIndex == 1)
                {
                    EditorGUILayout.HelpBox("处于UI下，请选择UI模式", MessageType.Warning);
                }
            }
        }
        --EditorGUI.indentLevel;
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_NavigationProperty);
        EditorGUI.BeginChangeCheck();
        Rect toggleRect = EditorGUILayout.GetControlRect();
        toggleRect.xMin += EditorGUIUtility.labelWidth;
        s_ShowNavigation = GUI.Toggle(toggleRect, s_ShowNavigation, m_VisualizeNavigation, EditorStyles.miniButton);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetBool(s_ShowNavigationKey, s_ShowNavigation);
            SceneView.RepaintAll();
        }
        EditorGUILayout.PropertyField(m_OnClickEventProperty);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif