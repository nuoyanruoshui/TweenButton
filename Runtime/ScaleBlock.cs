using UnityEngine;
using System;


[Serializable]
public struct ScaleBlock
{
    [SerializeField]
    private Vector3 m_NormalScale;
    [SerializeField]
    private Vector3 m_HighlightedScale;
    [SerializeField]
    private Vector3 m_PressedScale;
    [SerializeField]
    private Vector3 m_SelectedScale;
    [SerializeField]
    private Vector3 m_DisabledScale;
    [SerializeField]
    private float m_Duration;

    public Vector3 normalScale { get { return m_NormalScale; } set { m_NormalScale = value; } }
    public Vector3 highlightedScale { get { return m_HighlightedScale; } set { m_HighlightedScale = value; } }
    public Vector3 pressedScale { get { return m_PressedScale; } set { m_PressedScale = value; } }
    public Vector3 selectedScale { get { return m_SelectedScale; } set { m_SelectedScale = value; } }
    public Vector3 disabledScale { get { return m_DisabledScale; } set { m_DisabledScale = value; } }
    public float duration { get { return m_Duration; } set { m_Duration = value; } }

    public static ScaleBlock defaultScaleBlock;
    static ScaleBlock()
    {
        defaultScaleBlock = new ScaleBlock()
        {
            m_NormalScale = Vector3.one,
            m_HighlightedScale = Vector3.one * 1.1f,
            m_PressedScale = Vector3.one * 0.9f,
            m_SelectedScale = Vector3.one * 1.2f,
            m_DisabledScale = Vector3.one * 0.8f,
            m_Duration = 0.2f
        };
    }

}
[Serializable]
public class TweenAnimBlock
{
    public GameObject Go;
    [SerializeField]
    private TweenButtonAnim m_NormalAnim;
    [SerializeField]
    private TweenButtonAnim m_HighlightedAnim;
    [SerializeField]
    private TweenButtonAnim m_PressedAnim;
    [SerializeField]
    private TweenButtonAnim m_SelectedAnim;
    [SerializeField]
    private TweenButtonAnim m_DisabledAnim;

    public TweenButtonAnim normalAnim { get { return m_NormalAnim; } set { m_NormalAnim = value; } }
    public TweenButtonAnim highlightedAnim { get { return m_HighlightedAnim; } set { m_HighlightedAnim = value; } }
    public TweenButtonAnim pressedAnim { get { return m_PressedAnim; } set { m_PressedAnim = value; } }
    public TweenButtonAnim selectedAnim { get { return m_SelectedAnim; } set { m_SelectedAnim = value; } }
    public TweenButtonAnim disabledAnim { get { return m_DisabledAnim; } set { m_DisabledAnim = value; } }

    public TweenAnimBlock(GameObject go)
    {
        Go = go;
    }
}