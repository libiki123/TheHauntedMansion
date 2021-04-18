using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button ResumeButton;

    private void Start()
    {
        ResumeButton.onClick.AddListener(HandleResumeClicked);
    }

    void HandleResumeClicked()
    {
        GameManager.Instance.TogglePause();
    }

}
