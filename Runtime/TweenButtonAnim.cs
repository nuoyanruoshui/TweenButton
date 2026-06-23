using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class TweenButtonAnim : MonoBehaviour
{
    protected TweenButton m_TweenButton;
    protected TweenToggle m_TweenToggle;
    private bool m_IsInit = false;
    private void Init()
    {
        transform.TryGetComponent(out m_TweenButton);
        transform.TryGetComponent(out m_TweenToggle);
    }
    public virtual void PlayAnim()
    {
        if (!m_IsInit)
        {
            Init();
            m_IsInit = true;
        }
    }
}
