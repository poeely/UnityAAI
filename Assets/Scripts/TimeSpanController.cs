using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSpanController : MonoBehaviour
{
    private Scrollbar scrollBar;

    private void Awake()
    {
        scrollBar = GetComponent<Scrollbar>();
    }

    public void OnValueChanged()
    {
        Time.timeScale = scrollBar.value * 5f;
    }
}
