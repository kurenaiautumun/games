using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject MenuContainer;
    [SerializeField] private GameObject OptionsContainer;

    [Header("Transition Settings")]
    [SerializeField] private TransitionSettings transitionType;
    [SerializeField] private float transitionDelay;

    private TransitionManager transitionManager;

    private void Start()
    {
        MenuContainer.SetActive(true);
        transitionManager = TransitionManager.Instance();
    }

    public void Play()
    {
        transitionManager.Transition("GameScene", transitionType, transitionDelay);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShowOptions()
    {
        MenuContainer.SetActive(false);
        OptionsContainer.SetActive(true);
    }

    public void BackToMainMenu()
    {
        MenuContainer.SetActive(true);
        OptionsContainer.SetActive(false);
    }
}
