using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(dialogCustomiserManager))]
public class dialogCustomiserEditor : Editor
{
    string stringToEdit = "Enter file name...";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        dialogCustomiserManager myTarget = (dialogCustomiserManager)target;

        stringToEdit = GUILayout.TextArea(stringToEdit);

        // Test the dialog
        if (GUILayout.Button("Test Dialog"))
        {
            // Note: we only need the Show() method to display our dialogs!
            myTarget.StartPreviewDialogMode();
            myTarget.Show(stringToEdit);
        }
    }
}

public class dialogCustomiserManager : MonoBehaviour
{
    public List<Sprite> speechBubbleImageList;
    public GameObject charactersContainer;
    public Canvas canvas;
    public bool isDialogCreatorMode;

    public ExtendedDialogManager extendedDialogManager;
    private DialogManager dialogManager;
    private GameObject edmprinterObject, printerObject, textObject;
    private RectTransform printerRectTransform, textRectTransform;
    private GameObject charNameInputField;
    private TMP_Text currentDialogIndexText, totalDialogText;
    private GameObject characterBtn;
    private TMP_InputField savefileNameInputField;

    private List<string> dialogs;
    private List<string> talkingCharacters;
    private List<int> bubbleIndices;

    private int currentIndex, currentCharacterIdx;

    private string dataPath;

    void Start()
    {
        dialogs = new List<string>();
        talkingCharacters = new List<string>();
        bubbleIndices = new List<int>();
        dataPath = Application.persistentDataPath;

        dialogManager = extendedDialogManager.gameObject.GetComponent<DialogManager>();
        edmprinterObject = extendedDialogManager.gameObject.transform.GetChild(0).gameObject;

        if (isDialogCreatorMode)
        {
            printerObject = canvas.transform.Find("Printer").gameObject;
            textObject = printerObject.transform.Find("Text").gameObject;
            printerRectTransform = printerObject.GetComponent<RectTransform>();
            textRectTransform = textObject.GetComponent<RectTransform>();
            charNameInputField = canvas.transform.Find("charNameInputField").gameObject;
            currentDialogIndexText = canvas.transform.Find("CurrentDialogIndexText").GetComponent<TMP_Text>();
            totalDialogText = canvas.transform.Find("TotalDialogText").GetComponent<TMP_Text>();
            characterBtn = canvas.transform.Find("CharacterBtn").gameObject;
            savefileNameInputField = canvas.transform.Find("savefileNameInputField").GetComponent<TMP_InputField>();
            if (charactersContainer.transform.childCount > 0)
            {
                currentCharacterIdx = 0;
                characterBtn.GetComponentInChildren<TMP_Text>().text = charactersContainer.transform.GetChild(0).name;
            }
            else currentCharacterIdx = -1;
            Create();
            printerObject.GetComponent<Image>().sprite = speechBubbleImageList[0];

            canvas.gameObject.SetActive(true);
            extendedDialogManager.gameObject.SetActive(false);
        }
        else
        {
            if(canvas != null && canvas.gameObject.activeSelf)
            {
                canvas.gameObject.SetActive(false);
            }
            extendedDialogManager.gameObject.SetActive(true);
            edmprinterObject.gameObject.SetActive(false);
        }   
    }

    public void Save(string savePath)
    {
        // Save the data 3 lines at a time into the file
        using (StreamWriter writer = new StreamWriter(savePath))
        {
            for(int i = 0; i < dialogs.Count; i++)
            {
                writer.WriteLine(talkingCharacters[i]);
                writer.WriteLine(ConvertMultilineDialogIntoSingleLine(dialogs[i]));
                writer.WriteLine(bubbleIndices[i]);
            }
        }
    }

    public void Load(string loadPath)
    {
        // We need to clear everything first
        talkingCharacters.Clear();
        bubbleIndices.Clear();
        dialogs.Clear();

        // Now we read the data from the file 3 lines at a time (1st line for the character, 2nd line for the actual dialog, 3rd line for the type of bubble)
        using (StreamReader reader = new StreamReader(loadPath))
        {
            string character, dialog, speech;
            while ((character = reader.ReadLine()) != null && (dialog = reader.ReadLine()) != null && (speech = reader.ReadLine()) != null)
            {
                talkingCharacters.Add(character);
                dialogs.Add(ConvertSingleLineIntoMultilineDialog(dialog));
                bubbleIndices.Add(int.Parse(speech));
            }
        }
    }

    // This Show() has some extra functionalities added to it to enable the dialogs to be displayed from file as well as handle the different bubbles
    public void Show(string filename = null)
    {
        // If we want to load dialogs from file
        if (filename != null)
        {
            var p = Path.Combine(dataPath, filename);
            if(File.Exists(p))
            {
                Load(p);
            }
            else
            {
                Debug.Log("File does not exist");
                return;
            }
        }

        // Reset the index so it starts from the beginning
        currentIndex = 0;

        // Hook the functions
        dialogManager.OnPrintFinishedEvent += SwitchBubble;
        dialogManager.OnDialogEndedEvent += ResetDialog;

        // Add the dialogs from the list
        List < DialogData> dialogdata = new List < DialogData>();
        for(int i = 0;i < dialogs.Count;i++)
        {
            dialogdata.Add(new DialogData(dialogs[i], talkingCharacters[i]));
        }

        // Put the correct bubble at the start and we can start showing the dialogs by using the extended manager (as it nicely handles the character switching and bubble resizing)
        edmprinterObject.GetComponent<Image>().sprite = speechBubbleImageList[bubbleIndices[currentIndex]];
        extendedDialogManager.Show(dialogdata);
    }

    // If we are previewing, then we would want the creator UI to not show up
    public void StartPreviewDialogMode()
    {
        if(isDialogCreatorMode)
        {
            canvas.gameObject.SetActive(false);
            extendedDialogManager.gameObject.SetActive(true);
        }
        
        // We just do a check first if the correct characters are already added else we will add them
        GameObject characterPrefab = AssetDatabase.LoadAssetAtPath("Assets/External/DDSystem/Prefab/BindedCharacter.prefab", typeof(GameObject)) as GameObject;
        var edmcharcontainer = extendedDialogManager.gameObject.transform.Find("Characters");
        for(int i = 0; i < charactersContainer.transform.childCount;i++)
        {
            var cname = charactersContainer.transform.GetChild(i).name;
            if(edmcharcontainer.Find(cname) == null)
            {
                var go = Instantiate(characterPrefab, edmcharcontainer.transform);
                go.name = cname;
                go.GetComponent<inGameCharacterBinderScript>().inGameCharacterRef = charactersContainer.transform.GetChild(i).gameObject;
            }
        }

        // We want to show the creator UI again if we are in creator mode
        dialogManager.OnDialogEndedEvent += EndPreviewDialogMode;
    }

    // If we are in creator mode then we want the UI to be enabled again (and the extended manager object to be disabled)
    public void EndPreviewDialogMode()
    {
        if(isDialogCreatorMode)
        {
            canvas.gameObject.SetActive(true);
            extendedDialogManager.gameObject.SetActive(false);
        }

        dialogManager.OnDialogEndedEvent -= EndPreviewDialogMode;
    }

    // Change the bubble's image
    private void SwitchBubble()
    {
        currentIndex += 1;
        if(currentIndex < dialogs.Count)
            edmprinterObject.GetComponent<Image>().sprite = speechBubbleImageList[bubbleIndices[currentIndex]];
    }

    // When we are done then we need to unhook and reset some values
    private void ResetDialog()
    {
        currentIndex = 0;
        dialogManager.OnPrintFinishedEvent -= SwitchBubble;
        dialogManager.OnDialogEndedEvent -= ResetDialog;
    }

    ////////////////////////////////
    /// Creator Mode Section
    ////////////////////////////////

    // This is here for the Creator UI's printer and not the extended manager
    private void ResizePrinter()
    {
        var finalSize = textRectTransform.sizeDelta + new Vector2(80, 50);
        printerRectTransform.sizeDelta = finalSize;
    }

    // Based on the current dialog we are editing, we set the correct values into the UI elements
    private void SetCurrentIndex()
    {
        // To prevent the user from being unable to edit the input field when it is empty we put in some default text
        textObject.GetComponent<InputField>().text = dialogs[currentIndex].Trim().Length > 0? dialogs[currentIndex] : "Edit here...";
        printerObject.GetComponent<Image>().sprite = speechBubbleImageList[bubbleIndices[currentIndex]];
        characterBtn.GetComponentInChildren<TMP_Text>().text = talkingCharacters[currentIndex];
        ResizePrinter();

        // We want to relocate the bubble to the correct character
        var c = charactersContainer.transform.Find(talkingCharacters[currentIndex]).gameObject;
        SetCurrentTalkingCharacter(c);
    }

    // Handles the bubble's location based on the current character that is speaking (similar to the one in extended manager)
    private void SetCurrentTalkingCharacter(GameObject c)
    {
        if (c == null)
            Debug.Log("This character does not exist!");
        else
        {
            var characterPos = c.transform.position;

            // Find the mouth position if it exists
            var mouth = c.transform.Find("Mouth");
            if (mouth != null)
                characterPos = mouth.gameObject.GetComponent<Transform>().position;

            // Convert the world space position to screen position for UI elements
            var bubbleScreenPos = Camera.main.WorldToScreenPoint(characterPos);
            printerObject.transform.position = bubbleScreenPos;

            // If flip is enabled, then bubble speech will flip accordingly (make sure the image is proper) [We enable this by default for the creator mode]
            if (true)
            {
                var pls = printerObject.transform.localScale;
                var tls = textObject.transform.localScale;
                if (c.GetComponent<SpriteRenderer>().flipX)
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
    }

    // We need these functions to properly convert the strings for storage and displaying
    private string ConvertMultilineDialogIntoSingleLine(string s)
    {
        return s.Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r");
    }

    private string ConvertSingleLineIntoMultilineDialog(string s)
    {
        return s.Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\r", "\r");
    }
    
    ////////////////////////////////
    /// UI
    ////////////////////////////////


    public void Create()
    {
        dialogs.Add("");
        talkingCharacters.Add(charactersContainer.transform.GetChild(0).name);
        bubbleIndices.Add(0);

        if(currentIndex < 0)
            currentIndex = 0;

        currentDialogIndexText.SetText("Current Dialog: " + (currentIndex + 1));
        totalDialogText.SetText("Total Dialog: " + dialogs.Count);
    }

    public void OnSaveDialogBtnClicked()
    {
        Save(Path.Combine(dataPath, savefileNameInputField.text));
        Debug.Log("File saved");
    }

    public void OnLoadDialogBtnClicked()
    {
        var p = Path.Combine(dataPath, savefileNameInputField.text);
        if(File.Exists(p))
        {
            Load(p);
            Debug.Log("File loaded");
            currentDialogIndexText.SetText("Current Dialog: " + (currentIndex + 1));
            totalDialogText.SetText("Total Dialog: " + dialogs.Count);
            SetCurrentIndex();
        }
        else
        {
            Debug.Log("This file does not exist");
        }
    }

    public void OnTextEndEdit()
    {
        ResizePrinter();
        dialogs[currentIndex] = textObject.GetComponent<InputField>().text;
    }

    public void OnCharNameInputFieldEndEdit()
    {
        talkingCharacters[currentIndex] = charNameInputField.GetComponent<TMP_InputField>().text;
        var c = charactersContainer.transform.Find(talkingCharacters[currentIndex]).gameObject;
        SetCurrentTalkingCharacter(c);

    }

    public void OnPickBubbleBtnClicked()
    {
        int bubbleIdx = bubbleIndices[currentIndex];

        if (++bubbleIdx >= speechBubbleImageList.Count)
            bubbleIdx = 0;
        printerObject.GetComponent<Image>().sprite = speechBubbleImageList[bubbleIdx];
        bubbleIndices[currentIndex] = bubbleIdx;
    }

    public void OnCharacterBtnClicked()
    {
        if (++currentCharacterIdx >= charactersContainer.transform.childCount)
            currentCharacterIdx = 0;

        characterBtn.GetComponentInChildren<TMP_Text>().text = charactersContainer.transform.GetChild(currentCharacterIdx).name;
        talkingCharacters[currentIndex] = characterBtn.GetComponentInChildren<TMP_Text>().text;
        var c = charactersContainer.transform.Find(talkingCharacters[currentIndex]).gameObject;
        SetCurrentTalkingCharacter(c);

    }

    public void OnPrevDialogBtnClicked()
    {
        currentIndex--;
        if(currentIndex < 0)
            currentIndex = 0;
        currentDialogIndexText.SetText("Current Dialog: " + (currentIndex + 1));

        SetCurrentIndex();
    }

    public void OnNextDialogBtnClicked()
    {
        currentIndex++;
        if(currentIndex >= dialogs.Count)
            currentIndex = dialogs.Count - 1;
        currentDialogIndexText.SetText("Current Dialog: " + (currentIndex + 1));

        SetCurrentIndex();
    }
}