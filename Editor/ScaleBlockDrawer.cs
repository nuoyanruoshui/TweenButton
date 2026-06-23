#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
[CustomPropertyDrawer(typeof(ScaleBlock), true)]
public class ScaleBlockDrawer : PropertyDrawer
{
    const string kNormalScale = "m_NormalScale";
    const string kHighlightedScale = "m_HighlightedScale";
    const string kPressedScale = "m_PressedScale";
    const string kSelectedScale = "m_SelectedScale";
    const string kDisabledScale = "m_DisabledScale";
    const string kDuration = "m_Duration";
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        Rect drawRect = rect;
        drawRect.height = EditorGUIUtility.singleLineHeight;

        SerializedProperty normalScale = property.FindPropertyRelative(kNormalScale);
        SerializedProperty highlightedScale = property.FindPropertyRelative(kHighlightedScale);
        SerializedProperty pressedScale = property.FindPropertyRelative(kPressedScale);
        SerializedProperty selectedScale = property.FindPropertyRelative(kSelectedScale);
        SerializedProperty disabledScale = property.FindPropertyRelative(kDisabledScale);
        SerializedProperty duration = property.FindPropertyRelative(kDuration);

        EditorGUI.PropertyField(drawRect, normalScale);
        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(drawRect, highlightedScale);
        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(drawRect, pressedScale);
        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(drawRect, selectedScale);
        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(drawRect, disabledScale);
        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(drawRect, duration);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 6 + EditorGUIUtility.standardVerticalSpacing * 5; // 6 lines of text + 5 spaces
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement element = new VisualElement();
        var propertys = new[]
        {
            property.FindPropertyRelative(kNormalScale),
            property.FindPropertyRelative(kHighlightedScale),
            property.FindPropertyRelative(kPressedScale),
            property.FindPropertyRelative(kSelectedScale),
            property.FindPropertyRelative(kDisabledScale),
            property.FindPropertyRelative(kDuration),
        };
        foreach (var prop in propertys)
        {
            element.Add(new UnityEditor.UIElements.PropertyField(prop));
        }
        return element;
    }
}
[CustomPropertyDrawer(typeof(TweenAnimBlock), true)]
public class TweenAnimBlockDrawer : PropertyDrawer
{
    const string kNormalAnim = "m_NormalAnim";
    const string kHighlightedAnim = "m_HighlightedAnim";
    const string kPressedAnim = "m_PressedAnim";
    const string kSelectedAnim = "m_SelectedAnim";
    const string kDisabledAnim = "m_DisabledAnim";
    const string kGo = "Go";
    TweenButtonAnim[] tweenButtonAnims;
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        Rect drawRect = rect;
        drawRect.height = EditorGUIUtility.singleLineHeight;

        SerializedProperty normalAnim = property.FindPropertyRelative(kNormalAnim);
        SerializedProperty highlightedAnim = property.FindPropertyRelative(kHighlightedAnim);
        SerializedProperty pressedAnim = property.FindPropertyRelative(kPressedAnim);
        SerializedProperty selectedAnim = property.FindPropertyRelative(kSelectedAnim);
        SerializedProperty disabledAnim = property.FindPropertyRelative(kDisabledAnim);
        SerializedProperty go = property.FindPropertyRelative(kGo);

        tweenButtonAnims = (go.objectReferenceValue as GameObject)?.GetComponents<TweenButtonAnim>();

        DrawSingleSelector(drawRect, normalAnim, new GUIContent("Normal"));
        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        DrawSingleSelector(drawRect, highlightedAnim, new GUIContent("Highlighted"));
        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        DrawSingleSelector(drawRect, pressedAnim, new GUIContent("Pressed"));
        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        DrawSingleSelector(drawRect, selectedAnim, new GUIContent("Selected"));
        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        DrawSingleSelector(drawRect, disabledAnim, new GUIContent("Disabled"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 6 + EditorGUIUtility.standardVerticalSpacing * 5; // 6 lines of text + 5 spaces
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement element = new VisualElement();
        var propertys = new[]
        {
            property.FindPropertyRelative(kNormalAnim),
            property.FindPropertyRelative(kHighlightedAnim),
            property.FindPropertyRelative(kPressedAnim),
            property.FindPropertyRelative(kSelectedAnim),
            property.FindPropertyRelative(kDisabledAnim),
        };
        foreach (var prop in propertys)
        {
            element.Add(new UnityEditor.UIElements.PropertyField(prop));
        }
        return element;
    }
    private void DrawSingleSelector(Rect position, SerializedProperty property, GUIContent label)
    {
        if (tweenButtonAnims == null || tweenButtonAnims.Length == 0)
        {
            EditorGUI.LabelField(position, label.text, "没有可用的类");
            return;
        }

        int selectIndex = GetSingleSelectedIndex(property.objectReferenceValue);
        int index = EditorGUI.Popup(position, label.text, selectIndex, tweenButtonAnims.Select(x => x.GetType().Name).ToArray());

        if (index != selectIndex)
        {
            property.objectReferenceValue = index >= 0 ? tweenButtonAnims[index] : null;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
    private int GetSingleSelectedIndex(Object className)
    {
        if (tweenButtonAnims == null || tweenButtonAnims.Length <= 1)
            return 0;

        for (int i = 0; i < tweenButtonAnims.Length; i++)
        {
            if (tweenButtonAnims[i] == className)
            {
                return i;
            }
        }

        return 0;
    }
}
#endif