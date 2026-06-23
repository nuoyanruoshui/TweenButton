using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections;
public enum TransitionType { None, ColorTint, SpriteSwap, Animation, Scale, Script }
public enum TweenButtonType { UI, World }
[ExecuteAlways]
public sealed partial class TweenButton : Selectable, IPointerClickHandler, ISubmitHandler
{
    #region Events
    [System.Serializable]
    public class ButtonClickedEvent : UnityEvent { }
    #endregion
    #region Custom
    [SerializeField]
    private TransitionType m_TransitionType = TransitionType.ColorTint;
    public TransitionType transitionType { get { return m_TransitionType; } set { m_TransitionType = value; } }
    [SerializeField]
    private TweenButtonType m_TweenButtonType = TweenButtonType.UI;
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
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
    public ButtonClickedEvent onClick { get { return m_OnClick; } set { m_OnClick = value; } }
    #endregion
    [Tooltip("是否自动回滚到未选中状态?,即不让EventSystem自动回退")]
    public bool AutoRollBack = true;

    protected override void Awake()
    {
        if (m_TweenAnimBlock == null) m_TweenAnimBlock = new TweenAnimBlock(this.gameObject);
        switch (m_TweenButtonType)
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

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (!gameObject.activeInHierarchy)
            return;
        if (!AutoRollBack)
        {
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
                if (anim != null)
                {
                    anim.PlayAnim();
                }
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


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Press();
    }
    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_OnClick.Invoke();
    }
    public void OnSubmit(BaseEventData eventData)
    {
        Press();
        if (!IsActive() || !IsInteractable())
            return;

        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(OnFinishSubmit());
    }

    private IEnumerator OnFinishSubmit()
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
}
