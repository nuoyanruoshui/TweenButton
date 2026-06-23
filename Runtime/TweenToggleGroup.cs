using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class TweenToggleGroup : UIBehaviour
{
    [SerializeField] private bool m_AllowSwitchOff = false;
    public bool allowSwitchOff { get { return m_AllowSwitchOff; } set { m_AllowSwitchOff = value; } }
    protected List<TweenToggle> m_Toggles = new List<TweenToggle>();
    protected override void Start()
    {
        EnsureValidState();
        base.Start();
    }

    protected override void OnEnable()
    {
        EnsureValidState();
        base.OnEnable();
    }

    private void ValidateToggleIsInGroup(TweenToggle toggle)
    {
        if (toggle == null || !m_Toggles.Contains(toggle))
            throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] { toggle, this }));
    }
    public void UnregisterToggle(TweenToggle toggle)
    {
        if (m_Toggles.Contains(toggle))
            m_Toggles.Remove(toggle);
    }

    public void RegisterToggle(TweenToggle toggle)
    {
        if (!m_Toggles.Contains(toggle))
            m_Toggles.Add(toggle);
    }
    public void NotifyToggleOn(TweenToggle toggle, bool sendCallback = true)
    {
        ValidateToggleIsInGroup(toggle);
        // disable all toggles in the group
        for (var i = 0; i < m_Toggles.Count; i++)
        {
            if (m_Toggles[i] == toggle)
                continue;

            if (sendCallback)
                m_Toggles[i].isOn = false;
            else
                m_Toggles[i].SetIsOnWithoutNotify(false);
        }
    }
    public bool AnyTogglesOn()
    {
        return m_Toggles.Find(x => x.isOn) != null;
    }
    public void EnsureValidState()
    {
        // Update the toggle list to include all TweenToggles in children
        UpdateToggleList();

        if (!allowSwitchOff && !AnyTogglesOn() && m_Toggles.Count != 0)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                m_Toggles[0].SetIsOnWithoutNotify(true);
            else
                m_Toggles[0].isOn = true;
#else
            m_Toggles[0].isOn = true;
#endif
            NotifyToggleOn(m_Toggles[0], !Application.isPlaying);
        }

        IEnumerable<TweenToggle> activeToggles = ActiveToggles();

        if (activeToggles.Count() > 1)
        {
            TweenToggle firstActive = GetFirstActiveToggle();

            foreach (TweenToggle toggle in activeToggles)
            {
                if (toggle == firstActive)
                {
                    continue;
                }
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    toggle.SetIsOnWithoutNotify(false);
                else
                    toggle.isOn = false;
#else
                toggle.isOn = false;
#endif
            }
        }
    }

    private void UpdateToggleList()
    {
        // Clear the current list and add all TweenToggles in children
        m_Toggles.Clear();
        TweenToggle[] toggles = GetComponentsInChildren<TweenToggle>(true);
        foreach (var toggle in toggles)
        {
            if (toggle.group == this)
            {
                m_Toggles.Add(toggle);
            }
        }
    }
    public IEnumerable<TweenToggle> ActiveToggles()
    {
        return m_Toggles.Where(x => x.isOn);
    }
    public TweenToggle GetFirstActiveToggle()
    {
        IEnumerable<TweenToggle> activeToggles = ActiveToggles();
        return activeToggles.Count() > 0 ? activeToggles.First() : null;
    }
}
