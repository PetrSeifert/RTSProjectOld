using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    Toggle m_MenuToggle;
    float m_TimeScaleRef = 1f;
    float m_VolumeRef = 1f;
    bool m_Paused;


    void Awake()
    {
        m_MenuToggle = GetComponent <Toggle> ();
	}


    void MenuOn ()
    {
        m_TimeScaleRef = Time.timeScale;
        Time.timeScale = 0f;

        m_VolumeRef = AudioListener.volume;
        AudioListener.volume = 0f;

        m_Paused = true;
    }


    public void MenuOff ()
    {
        Time.timeScale = m_TimeScaleRef;
        AudioListener.volume = m_VolumeRef;
        m_Paused = false;
    }


    public void OnMenuStatusChange ()
    {
        if (m_MenuToggle.isOn && !m_Paused)
        {
            MenuOn();
        }
        else if (!m_MenuToggle.isOn && m_Paused)
        {
            MenuOff();
        }
    }


#if !MOBILE_INPUT
	void Update()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
		{
		    m_MenuToggle.isOn = !m_MenuToggle.isOn;
            Cursor.visible = m_MenuToggle.isOn;//force the cursor visible if anythign had hidden it
		}
	}
#endif

}
