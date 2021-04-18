using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animation mainMenuAnimator;
    [SerializeField] private AnimationClip fadeOutAnimation;

    public event Action<bool> OnMainMenuFadeComplete;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFadeOutComplete()     // animation event
    {
        OnMainMenuFadeComplete?.Invoke(true);
    }

    void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (previousState == GameManager.GameState.PREGAME && currentState == GameManager.GameState.RUNNING)    // When the game went from pregame to runnning
        {
            FadeOut();
        }
    }

    public void FadeOut()
    {
        //mainMenuAnimator.Stop();
        //mainMenuAnimator.clip = fadeOutAnimation;
        //mainMenuAnimator.Play();

        OnFadeOutComplete();                        ///// CARE: Call this in animation when ready
    }

}
