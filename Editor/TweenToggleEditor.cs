#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TweenToggle)), CanEditMultipleObjects]
public class TweenToggleEditor : Editor
{
    SerializedProperty m_TweenToggleTypeProperty;
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
    SerializedProperty m_OnValueChangedProperty;
    SerializedProperty m_GroupProperty;
    SerializedProperty m_IsOnProperty;
    SerializedProperty m_CheckMarkProperty;
    SerializedProperty m_CheckMarkSpriteProperty;
    SerializedProperty m_ToggleTransitionProperty;
    SerializedProperty m_AutoRollbackProperty;
    GUIContent m_VisualizeNavigation = EditorGUIUtility.TrTextContent("Visualize", "显示可选择 UI 元素之间的导航流程。");
    private static bool s_ShowNavigation = false;
    private static string s_ShowNavigationKey = "SelectableEditor.ShowNavigation";
    private TweenToggle TweenToggle;

    private void OnEnable()
    {
        m_TweenToggleTypeProperty = serializedObject.FindProperty("m_TweenToggleType");
        m_InteractableProperty = serializedObject.FindProperty("m_Interactable");
        m_TargetGraphicProperty = serializedObject.FindProperty("m_TargetGraphic");
        m_TargetRendererProperty = serializedObject.FindProperty("m_TargetRenderer");
        m_TransitionProperty = serializedObject.FindProperty("m_TransitionType");
        m_ColorBlockProperty = serializedObject.FindProperty("m_Colors");
        m_SpriteStateProperty = serializedObject.FindProperty("m_SpriteState");
        m_AnimTriggerProperty = serializedObject.FindProperty("m_AnimationTriggers");
        m_ScaleBlockProperty = serializedObject.FindProperty("m_ScaleBlock");
        m_TweenAnimProperty = serializedObject.FindProperty("m_TweenAnimBlock");
        m_OnValueChangedProperty = serializedObject.FindProperty("onValueChanged");
        m_GroupProperty = serializedObject.FindProperty("m_Group");
        m_IsOnProperty = serializedObject.FindProperty("m_IsOn");
        m_NavigationProperty = serializedObject.FindProperty("m_Navigation");
        m_CheckMarkProperty = serializedObject.FindProperty("CheckMark");
        m_CheckMarkSpriteProperty = serializedObject.FindProperty("CheckMarkSprite");
        m_ToggleTransitionProperty = serializedObject.FindProperty("toggleTransition");
        m_AutoRollbackProperty = serializedObject.FindProperty("AutoRollBack");
        TweenToggle = (target as TweenToggle);
        s_ShowNavigation = EditorPrefs.GetBool(s_ShowNavigationKey);
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Basic Properties
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_TweenToggleTypeProperty);
        EditorGUILayout.PropertyField(m_InteractableProperty);
        EditorGUILayout.PropertyField(m_AutoRollbackProperty);


        // Transition Settings
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_TransitionProperty);

        ++EditorGUI.indentLevel;
        {
            // Target Settings based on Toggle Type
            switch (m_TweenToggleTypeProperty.enumValueIndex)
            {
                case 0: // UI Mode
                    EditorGUILayout.PropertyField(m_TargetGraphicProperty);
                    break;
                case 1: // World Mode
                    EditorGUILayout.PropertyField(m_TargetRendererProperty);
                    if (TweenToggle.GetComponent<Collider>() == null && TweenToggle.GetComponent<Collider2D>() == null)
                    {
                        EditorGUILayout.HelpBox("World模式下需要一个 Collider 组件，否则无法触发点击", MessageType.Warning);
                    }
                    break;
            }

            // Transition Specific Settings
            switch (m_TransitionProperty.enumValueIndex)
            {
                case 1: // ColorTint
                    EditorGUILayout.PropertyField(m_ColorBlockProperty);
                    break;
                case 2: // SpriteSwap
                    EditorGUILayout.PropertyField(m_SpriteStateProperty);
                    break;
                case 3: // Animation
                    EditorGUILayout.PropertyField(m_AnimTriggerProperty);
                    break;
                case 4: // Scale
                    EditorGUILayout.PropertyField(m_ScaleBlockProperty);
                    break;
                case 5: // Script
                    EditorGUILayout.PropertyField(m_TweenAnimProperty);
                    break;
            }

            // Validation Warnings
            if (TweenToggle.transform is RectTransform)
            {
                if (m_TweenToggleTypeProperty.enumValueIndex == 1)
                {
                    EditorGUILayout.HelpBox("处于UI下，请选择UI模式", MessageType.Warning);
                }
            }
        }
        --EditorGUI.indentLevel;

        // Navigation Settings
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
        EditorGUILayout.PropertyField(m_IsOnProperty);
        EditorGUILayout.PropertyField(m_GroupProperty);
        EditorGUILayout.PropertyField(m_ToggleTransitionProperty);

        // CheckMark Settings based on Toggle Type
        switch (m_TweenToggleTypeProperty.enumValueIndex)
        {
            case 0: // UI Mode
                EditorGUILayout.PropertyField(m_CheckMarkProperty, new GUIContent("Check Mark"));
                break;
            case 1: // World Mode
                EditorGUILayout.PropertyField(m_CheckMarkSpriteProperty, new GUIContent("Check Mark Sprite"));
                break;
        }
        // Events
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_OnValueChangedProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif