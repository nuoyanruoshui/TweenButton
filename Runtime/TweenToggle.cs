using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using static UnityEngine.UI.Toggle;
[ExecuteAlways]
public sealed class TweenToggle : Selectable, IPointerClickHandler, ISubmitHandler
{
    #region Events
    [System.Serializable]
    public class ToggleEvent : UnityEvent<bool> { }
    #endregion
    #region Custom
    [SerializeField]
    private TransitionType m_TransitionType = TransitionType.ColorTint;
    public TransitionType transitionType { get { return m_TransitionType; } set { m_TransitionType = value; } }
    [SerializeField]
    private TweenButtonType m_TweenToggleType = TweenButtonType.UI;
    [SerializeField]
    private ScaleBlock m_ScaleBlock = ScaleBlock.defaultScaleBlock;
    public ScaleBlock scaleBlock { get { return m_ScaleBlock; } set { m_ScaleBlock = value; } }
    [SerializeField]
    private TweenAnimBlock m_TweenAnimBlock;
    public TweenAnimBlock tweenAnimBlock { get { return m_TweenAnimBlock; } set { m_TweenAnimBlock = value; } }
    #endregion
    #region World
    [SerializeField]
    private Renderer m_TargetRenderer;
    public SpriteRenderer spriteRenderer { get { return m_TargetRenderer as SpriteRenderer; } }
    #endregion
    #region Button
    public ToggleTransition toggleTransition = ToggleTransition.Fade;
    [SerializeField]
    public ToggleEvent onValueChanged = new ToggleEvent();
    [SerializeField]
    private TweenToggleGroup m_Group;

    /// <summary>
    /// Group the toggle belongs to.
    /// </summary>
    public TweenToggleGroup group
    {
        get { return m_Group; }
        set
        {
            SetToggleGroup(value, true);
            PlayEffect(true);
        }
    }
    [Tooltip("当前Toggle是否开启?")]
    [SerializeField]
    private bool m_IsOn;
    public bool isOn
    {
        get { return m_IsOn; }

        set
        {
            Set(value);
        }
    }
    public Graphic[] CheckMark;
    public SpriteRenderer[] CheckMarkSprite;
    [Tooltip("是否自动回滚到未选中状态?,即不让EventSystem自动回退")]
    public bool AutoRollBack = true;
    #endregion
    protected override void Awake()
    {
        if (m_TweenAnimBlock == null) m_TweenAnimBlock = new TweenAnimBlock(this.gameObject);
        switch (m_TweenToggleType)
        {
            case TweenButtonType.UI:
                if (targetGraphic == null)
                    targetGraphic = GetComponent<Graphic>();
                break;
            case TweenButtonType.World:
                if (m_TargetRenderer == null)
                    m_TargetRenderer = GetComponent<Renderer>();
                break;
        }
    }

    protected override void Start()
    {
        PlayEffect(true);
    }
    void Set(bool value, bool sendCallback = true)
    {
        if (m_IsOn == value)
            return;

        m_IsOn = value;
        if (m_Group != null && m_Group.isActiveAndEnabled && IsActive())
        {
            if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff))
            {
                m_IsOn = true;
                m_Group.NotifyToggleOn(this, sendCallback);
            }
        }
        PlayEffect(toggleTransition == ToggleTransition.None);
        if (sendCallback)
        {
            UISystemProfilerApi.AddMarker("Toggle.value", this);
            onValueChanged.Invoke(m_IsOn);
        }
    }
    private void SetToggleGroup(TweenToggleGroup newGroup, bool setMemberValue)
    {
        if (m_Group != null)
            m_Group.UnregisterToggle(this);

        if (setMemberValue)
            m_Group = newGroup;

        if (newGroup != null && IsActive())
            newGroup.RegisterToggle(this);

        if (newGroup != null && isOn && IsActive())
            newGroup.NotifyToggleOn(this);
    }
    private void PlayEffect(bool instant)
    {
        // Handle CheckMark for UI mode
        if (CheckMark != null)
        {
            float targetAlpha = m_IsOn ? 1f : 0f;
            if (toggleTransition == ToggleTransition.Fade && !instant)
            {
                // CheckMark.CrossFadeAlpha(targetAlpha, 0.1f, true);
                PlayMark(targetAlpha, 0.1f, true);
            }
            else
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    foreach (var mark in CheckMark)
                    {
                        if (mark != null)
                        {
                            mark.canvasRenderer.SetAlpha(targetAlpha);
                        }
                    }
                }
                // CheckMark.canvasRenderer.SetAlpha(targetAlpha);
                else
#endif
                    // CheckMark.CrossFadeAlpha(m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
                    PlayMark(m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
            }
        }


        // Handle CheckMarkSprite for World mode
        // if (CheckMarkSprite != null)
        // {
        //     CheckMarkSprite.enabled = m_IsOn;
        //     CheckMarkSprite.DOColor(m_IsOn ? Color.white : Color.clear, 0.1f);
        // }
        if (CheckMarkSprite.Length > 0)
        {
            foreach (var mark in CheckMarkSprite)
            {
                if (mark != null)
                {
                    mark.enabled = m_IsOn;
                    mark.DOColor(m_IsOn ? Color.white : Color.clear, 0.1f);
                }
            }
        }

        // Handle original transition effects
        if (targetGraphic == null && m_TargetRenderer == null)
            return;

        switch (m_TransitionType)
        {
            case TransitionType.ColorTint:
                Color targetColor = m_IsOn ? colors.highlightedColor : colors.normalColor;
                StartColorTween(targetColor * colors.colorMultiplier, instant);
                break;
            case TransitionType.SpriteSwap:
                Sprite targetSprite = m_IsOn ? spriteState.selectedSprite : null;
                DoSpriteSwap(targetSprite);
                break;
            case TransitionType.Animation:
                string triggerName = m_IsOn ? animationTriggers.selectedTrigger : animationTriggers.normalTrigger;
                TriggerAnimation(triggerName);
                break;
            case TransitionType.Scale:
                Vector3 targetScale = m_IsOn ? m_ScaleBlock.selectedScale : m_ScaleBlock.normalScale;
                StartScaleTween(targetScale);
                break;
            case TransitionType.Script:
                TweenButtonAnim targetAnim = m_IsOn ? m_TweenAnimBlock.selectedAnim : m_TweenAnimBlock.normalAnim;
                if (targetAnim != null)
                {
                    targetAnim.PlayAnim();
                }
                break;
            default:
                // Default fade effect for targetGraphic
                if (targetGraphic != null)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        targetGraphic.canvasRenderer.SetAlpha(m_IsOn ? 1f : 0f);
                    else
#endif
                        targetGraphic.CrossFadeAlpha(m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
                }
                break;
        }
    }
    private void PlayMark(float targetAlpha, float dur, bool b)
    {
        if (CheckMark.Length > 0)
        {
            foreach (var mark in CheckMark)
            {
                if (mark != null)
                {
                    mark.CrossFadeAlpha(targetAlpha, dur, b);
                }
            }
        }
    }
    public void SetIsOnWithoutNotify(bool value)
    {
        Set(value, false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        InternalToggle();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        InternalToggle();
        if (!IsActive() || !IsInteractable())
            return;

        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(OnFinishSubmit());
    }

    private System.Collections.IEnumerator OnFinishSubmit()
    {
        var fadeTime = colors.fadeDuration;
        var elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        DoStateTransition(currentSelectionState, false);
    }
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (!gameObject.activeInHierarchy)
            return;

        // 如果AutoRollBack为false，并且当前是选中状态，不允许回退到Normal状态
        if (!AutoRollBack && m_IsOn)
        {
            // 当AutoRollBack为false且按钮处于选中状态时，保持选中状态
            state = SelectionState.Selected;
        }

        Color tintColor;
        Sprite transitionSprite;
        string triggerName;
        Vector3 scale;
        TweenButtonAnim anim;

        switch (state)
        {
            case SelectionState.Normal:
                tintColor = colors.normalColor;
                transitionSprite = null;
                triggerName = animationTriggers.normalTrigger;
                scale = m_ScaleBlock.normalScale;
                anim = m_TweenAnimBlock.normalAnim;
                break;
            case SelectionState.Highlighted:
                tintColor = colors.highlightedColor;
                transitionSprite = spriteState.highlightedSprite;
                triggerName = animationTriggers.highlightedTrigger;
                scale = m_ScaleBlock.highlightedScale;
                anim = m_TweenAnimBlock.highlightedAnim;
                break;
            case SelectionState.Pressed:
                tintColor = colors.pressedColor;
                transitionSprite = spriteState.pressedSprite;
                triggerName = animationTriggers.pressedTrigger;
                scale = m_ScaleBlock.pressedScale;
                anim = m_TweenAnimBlock.pressedAnim;
                break;
            case SelectionState.Selected:
                tintColor = colors.selectedColor;
                transitionSprite = spriteState.selectedSprite;
                triggerName = animationTriggers.selectedTrigger;
                scale = m_ScaleBlock.selectedScale;
                anim = m_TweenAnimBlock.selectedAnim;
                break;
            case SelectionState.Disabled:
                tintColor = colors.disabledColor;
                transitionSprite = spriteState.disabledSprite;
                triggerName = animationTriggers.disabledTrigger;
                scale = m_ScaleBlock.disabledScale;
                anim = m_TweenAnimBlock.disabledAnim;
                break;
            default:
                tintColor = Color.black;
                transitionSprite = null;
                triggerName = string.Empty;
                scale = Vector3.one;
                anim = null;
                break;
        }

        switch (m_TransitionType)
        {
            case TransitionType.ColorTint:
                StartColorTween(tintColor * colors.colorMultiplier, instant);
                break;
            case TransitionType.SpriteSwap:
                DoSpriteSwap(transitionSprite);
                break;
            case TransitionType.Animation:
                TriggerAnimation(triggerName);
                break;
            case TransitionType.Scale:
                StartScaleTween(scale);
                break;
            case TransitionType.Script:
                if (anim != null) anim.PlayAnim();
                break;
        }
    }

    private void TriggerAnimation(string triggername)
    {
#if PACKAGE_ANIMATION
        if (transition != Transition.Animation || animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
            return;

        animator.ResetTrigger(animationTriggers.normalTrigger);
        animator.ResetTrigger(animationTriggers.highlightedTrigger);
        animator.ResetTrigger(animationTriggers.pressedTrigger);
        animator.ResetTrigger(animationTriggers.selectedTrigger);
        animator.ResetTrigger(animationTriggers.disabledTrigger);

        animator.SetTrigger(triggername);
#endif
    }

    private void DoSpriteSwap(Sprite newSprite)
    {
        if (image != null)
        {
            image.overrideSprite = newSprite;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

    private void StartColorTween(Color targetColor, bool instant)
    {
        if (targetGraphic != null)
        {
            targetGraphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
        }
        else if (m_TargetRenderer != null)
        {
            spriteRenderer.DOColor(targetColor, instant ? 0f : colors.fadeDuration);
        }
    }
    private void StartScaleTween(Vector3 targetScale)
    {
        if (m_TargetRenderer != null)
        {
            m_TargetRenderer.transform.DOScale(targetScale, scaleBlock.duration);
        }
        else if (targetGraphic != null)
        {
            targetGraphic.transform.DOScale(targetScale, scaleBlock.duration);
        }
    }

    private void InternalToggle()
    {
        if (!IsActive() || !IsInteractable())
            return;

        isOn = !isOn;
    }
}
