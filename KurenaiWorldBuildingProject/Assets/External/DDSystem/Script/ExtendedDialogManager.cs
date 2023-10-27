// This is an extension built on top of the other Dialog Manager
// This manager is aimed for dialogs with speech bubbles while the other is more RPG oriented (print on the bottom)
// This asset is designed to be as close as possible to the original in order to preserve as many functionalities that are currently present
// This similarity also ensures that any changes made in the base manager can be extended to this too
// Also note that there are separate dialog assets and character assets for both types

using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#region UNITYEDITOR

[CustomEditor(typeof(ExtendedDialogManager))]
public class ExtendedDialogManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ExtendedDialogManager manager = (ExtendedDialogManager)target;

        GUILayout.Space(EditorGUIUtility.singleLineHeight);
        if (GUILayout.Button("Find Characters"))
        {
            var dialogManagerCharactersContainer = manager.transform.Find("Characters").gameObject;
            var worldCharactersContainer = manager.charactersContainer;

            if (worldCharactersContainer == null)
            {
                Debug.LogError("Please ensure that you have a Game Object in the world that contains all the characters that you wish to enable dialogues and it is referenced in the public field.");
            }
            else if(dialogManagerCharactersContainer.transform.childCount > 0)
            {
                Debug.LogError("Please ensure that the characters GameObject that is part of this Manager has no existing characters already present.");
            }
            else
            {
                GameObject characterPrefab = AssetDatabase.LoadAssetAtPath("Assets/External/DDSystem/Prefab/BindedCharacter.prefab", typeof(GameObject)) as GameObject;

                for (int i = 0; i < worldCharactersContainer.transform.childCount; i++)
                {
                    var character = Instantiate(characterPrefab, dialogManagerCharactersContainer.transform);
                    character.GetComponent<inGameCharacterBinderScript>().inGameCharacterRef = worldCharactersContainer.transform.GetChild(i).gameObject;
                    character.name = worldCharactersContainer.transform.GetChild(i).name;
                }

                Debug.Log("Added the characters for dialogs! (Check console for note)\nNOTE: If you rename the objects in Characters that is part of this Manager, remember to check that the character names match those that are given in DialogData()\n\n");
            }
        }
    }
}

#endregion

public class ExtendedDialogManager : MonoBehaviour
{
    private DialogManager dialogManager;
    private GameObject printerObject, textObject;
    private RectTransform printerRectTransform, textRectTransform;
    private GameObject currentTalkingCharacter;
    private bool isInitialized = false;

    [Header("Dialog Settings")]
    public bool flipBubbleWithCharacter = true;
    public bool zoomInOnCharacters = true;
    public float zoomFactor = 1.0f;
    private float originalZoom;

    [Header("Misc Settings")]
    public GameObject charactersContainer;    

    private void Start()
    {
        isInitialized = false;

        dialogManager = GetComponent<DialogManager>();
        printerObject = transform.GetChild(0).gameObject;
        textObject = printerObject.transform.GetChild(0).gameObject;
        printerRectTransform = printerObject.GetComponent<RectTransform>();
        textRectTransform = textObject.GetComponent<RectTransform>();

        originalZoom = Camera.main.orthographicSize;

        if (dialogManager != null )
            isInitialized = true;
    }


    //================================================
    //Public Methods
    //================================================

    public void Show(DialogData Data)
    {
        dialogManager.OnPrintStartEvent += SwitchCurrentActiveTalkingCharacter;
        dialogManager.OnPrintEvent += BubbleDialog;
        dialogManager.OnPrintFinishedEvent += ResizePrinter;
        dialogManager.OnDialogEndedEvent += FinishedDialog;

        dialogManager.Show(Data);
    }

    public void Show(List<DialogData> Data)
    {
        dialogManager.OnPrintStartEvent += SwitchCurrentActiveTalkingCharacter;
        dialogManager.OnPrintEvent += BubbleDialog;
        dialogManager.OnPrintFinishedEvent += ResizePrinter;
        dialogManager.OnDialogEndedEvent += FinishedDialog;

        dialogManager.Show(Data);
    }

    public bool IsInitialized() { return isInitialized; }

    public void Initialize()
    {
        dialogManager = GetComponent<DialogManager>();
        if (dialogManager != null)
            isInitialized = true;
    }

    //================================================
    //Private Methods
    //================================================

    private void FinishedDialog()
    {
        // Unhook the functions from the events
        dialogManager.OnPrintStartEvent -= SwitchCurrentActiveTalkingCharacter;
        dialogManager.OnPrintEvent -= BubbleDialog;
        dialogManager.OnPrintFinishedEvent -= ResizePrinter;
        dialogManager.OnDialogEndedEvent -= FinishedDialog;

        // Fix the camera zoom
        if(zoomInOnCharacters)
            Camera.main.orthographicSize = originalZoom;
    }

    private void SwitchCurrentActiveTalkingCharacter()
    {      
        // Get the current talking character
        currentTalkingCharacter = dialogManager.GetCurrentInGameCharacter();
        var characterPos = currentTalkingCharacter.transform.position;

        // Zoom into the characters
        if (zoomInOnCharacters)
        {
            Camera.main.orthographicSize = originalZoom / zoomFactor;
            Camera.main.transform.position = new Vector3(characterPos.x, characterPos.y, Camera.main.transform.position.z);
        }          

        // Find the mouth position if it exists
        var mouth = currentTalkingCharacter.transform.Find("Mouth");
        if( mouth != null )
            characterPos = mouth.gameObject.GetComponent<Transform>().position;

        // Convert the world space position to screen position for UI elements
        var bubbleScreenPos = Camera.main.WorldToScreenPoint(characterPos);
        printerObject.transform.position = bubbleScreenPos;

        // If flip is enabled, then bubble speech will flip accordingly (make sure the image is proper)
        if (flipBubbleWithCharacter)
        {
            var pls = printerObject.transform.localScale;
            var tls = textObject.transform.localScale;
            if (currentTalkingCharacter.GetComponent<SpriteRenderer>().flipX)
            {
                printerObject.transform.localScale = new Vector3(-1, 1, 1);
                textObject.transform.localScale = new Vector3(-1, 1, 1);
                textRectTransform.pivot = new Vector3(1, 0.5f, 0);
            }
            else
            {
                printerObject.transform.localScale = Vector3.one;
                textObject.transform.localScale = Vector3.one;
                textRectTransform.pivot = new Vector3(0, 0.5f, 0);
            }
        }
    }

    private void BubbleDialog()
    {
        textObject.GetComponent<HorizontalLayoutGroup>().enabled = true;
        textObject.GetComponent<ContentSizeFitter>().enabled = true;
        ResizePrinter();
    }

    private void ResizePrinter()
    {
        var finalSize = textRectTransform.sizeDelta + new Vector2(80, 50);
        printerRectTransform.sizeDelta = finalSize;
    }
}
