using Doublsb.Dialog;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#region UNITYEDITOR
#if (UNITY_EDITOR)
[CustomEditor(typeof(speechTestScript))]
public class speechTestEditor : Editor
{
    string stringToEdit = "Edit here...";
    string talkingCharacter = "";
    string[] talkingCharacterOptions = null;
    int selectedOption = 0;
    string command = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(!EditorApplication.isPlaying)
        {
            GUILayout.Label("Run the game in the Editor's Play mode to start testing");
            return;
        }
        speechTestScript myTarget = (speechTestScript)target;

        if (GUILayout.Button("Start test conversation"))
        {
            myTarget.TestSpeechFromJson();
        }

        talkingCharacterOptions = myTarget.GetCharacters();    

        GUILayout.Space(20);
        GUILayout.Label("Dialog Testing");

        GUILayout.Space(10);
        GUILayout.Label("Dialog:");
        stringToEdit = GUILayout.TextArea(stringToEdit);

        /*GUILayout.Space(10);
        GUILayout.Label("Character:");
        talkingCharacter = GUILayout.TextField(talkingCharacter);*/

        if (talkingCharacterOptions != null)
        {
            selectedOption = EditorGUILayout.Popup("Dropdown", selectedOption, talkingCharacterOptions);
            talkingCharacter = talkingCharacterOptions[selectedOption];
        }

        if (GUILayout.Button("Test Dialog"))
        {
            myTarget.TestDialog(stringToEdit, talkingCharacter);
            command = "new DialogData(\"" + stringToEdit + "\", \"" + talkingCharacter + "\");";
        }
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.fontSize = 20;
        fontStyle.font = EditorStyles.miniBoldFont;
        fontStyle.normal.textColor = Color.white;

        GUILayout.Space(10);
        GUILayout.Label("Command:\n");
        GUILayout.Label(command, fontStyle);
        if (GUILayout.Button("Copy Command"))
        {
            GUIUtility.systemCopyBuffer = command;
            if (command.Trim().Length == 0)  Debug.Log("Copied an empty string!");
            else Debug.Log("Command successfully copied!");
        }
    }
}
#endif
#endregion

public class speechTestScript : MonoBehaviour
{
    public bool testSpeechOnStart = true;
    public GameObject mapControlPanel;
    public GameManagerController gameManagerController;
    // Remember to get a reference to the extended variant for speech bubbles
    private ExtendedDialogManager dialogManager;
    private GameObject charactersContainer;
    private bool dialogInProgress = false;

    private void Start()
    {
        dialogManager = GameObject.Find("DialogAsset").GetComponent<ExtendedDialogManager>();
        charactersContainer = dialogManager.gameObject.transform.Find("Characters").gameObject;
        if (testSpeechOnStart)
            TestSpeechFromJson();

        gameManagerController = GameObject.Find("GameManager")?.GetComponent<GameManagerController>();

        int characterCount = 0;
        int gridIdx = 0;

        // This test scenario only works when the cook is placed first and the girl is placed next. They have to be the only two characters in the scene
        gameManagerController.OnObjectPlacedEvent += (GameObject item) =>
        {
            if (item.TryGetComponent<GridObject>(out var component) && component.type == objectType.Character)
            {
                characterCount++;
                if (characterCount == 2 && item.name == "Girl")
                {
                    gridIdx = gameManagerController.GetSubGridXIndex(component.gridPosition);
                    gameManagerController.SwitchViewMode(true);
                    gameManagerController.SwitchInteractionMode(GridInteractionMode.Select);
                    gameManagerController.ClearPreviousSubGridView(gameManagerController.sideViewActiveGrid);
                    gameManagerController.sideViewActiveGrid = gridIdx;
                    gameManagerController.SetCurrentSubGridView(gridIdx);
                    mapControlPanel.SetActive(false);
                    TestSpeechFromJson();
                }
            }
        };

        gameManagerController.OnSubGridChanged += (int grid) =>
        {
            if (!dialogInProgress)
                return;

            if (grid == gridIdx)
            {
                dialogManager.SwitchCurrentActiveTalkingCharacter();
            }
            else
            {
                dialogManager.ResetToOriginalZoom();
            }
        };
    }

    public void TestSpeech()
    {
        // Follows a similar structure to the normal dialog manager in order to be easy to learn and use
        var dialogTexts = new List<DialogData>
        {
            new DialogData("Hello! My name is\n/size:up//speed:down//color:red/ALICE/color:black//size:init//speed:init/", "Girl"),
            new DialogData("Hello to you too!\n I'm a /size:100//color:green/COOK/color:black//size:init/", "Boy"),
            new DialogData("Nice meeting you here.", "Girl"),
            new DialogData("Same!", "Boy")
        };

        DialogData optionsData = new DialogData("Do you like cooking?", "Boy");
        optionsData.SelectList.Character = "Girl";
        optionsData.SelectList.Add("True", "Yeah I sure do!");
        optionsData.SelectList.Add("Neutral", "Umm it's okay.");
        optionsData.SelectList.Add("False", "Nope, really hate it.");
        optionsData.Callback = () =>
        {
            List<DialogData> dialogs = new List<DialogData>();
            if (dialogManager.Result == "True")
            {
                dialogs.Add(new DialogData("That sounds great. Let's make something together.", "Boy"));
            } else if (dialogManager.Result == "Neutral")
            {
                dialogs.Add(new DialogData("Oh, let' cook something. Maybe cooking will grow on you...", "Boy"));
                dialogs.Add(new DialogData("I hope so.", "Girl"));
            } else
            {
                dialogs.Add(new DialogData("That's sad to hear. Maybe if you try it, you will change your mind.", "Boy"));
            }
            dialogManager.Show(dialogs);
        };

        dialogTexts.Add(optionsData);
        dialogManager.Show(dialogTexts);
    }

    public void TestSpeechFromJson()
    {
        dialogInProgress = true;
        dialogManager.UpdateCameraProperties();

        int oldIdx = 0;

        dialogManager.DialogEndedEvent += (string name) =>
        {
            Debug.Log("Current dialog that ended: " + name);
            if (name == "NiceMeetingYouHere")
            {
                dialogManager.ResetToOriginalCameraProperties();
                oldIdx = gameManagerController.sideViewActiveGrid;
                gameManagerController.ClearPreviousSubGridView(gameManagerController.sideViewActiveGrid);
                gameManagerController.sideViewActiveGrid = 0;
                gameManagerController.SetCurrentSubGridView(0);
            }
            else if (name == "DoYouLikeCooking")
            {
                gameManagerController.ClearPreviousSubGridView(gameManagerController.sideViewActiveGrid);
                gameManagerController.sideViewActiveGrid = oldIdx;
                gameManagerController.SetCurrentSubGridView(oldIdx);
                dialogManager.SwitchCurrentActiveTalkingCharacter();
            }
        };

        dialogManager.LoadFromJson("Dialogs/Dialog");

        dialogManager.DialogBranchEndedEvent += (string name) =>
        {
            if (name == "False" || name == "Neutral" || name == "True")
            {
                FinishDialogs();
            }
        };
    }

    private void FinishDialogs()
    {
        dialogInProgress = false;
        mapControlPanel.SetActive(true);
        gameManagerController.gameObject.SetActive(true);
        gameManagerController.SwitchViewMode();
        Camera.main.GetComponent<CameraController>().enabled = true;
        dialogManager.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void TestDialog(string dialog, string character)
    {
        if(character.Trim().Length == 0) {
            Debug.Log("You can't have no characters for this dialog style! Randomly choosing one to display.");
            character = charactersContainer.transform.GetChild(Random.Range(0,charactersContainer.transform.childCount)).name;
        }
        else if (charactersContainer.transform.Find(character) == null)
        {
            Debug.Log("You can't have an invalid character! Please check if the name is correct. Randomly choosing one to display.");
            character = charactersContainer.transform.GetChild(Random.Range(0, charactersContainer.transform.childCount)).name;
        }

        dialog = dialog.Replace("\\n", "\n").Replace("\\t","\t").Replace("\\r", "\r");

        dialogManager.Show(new DialogData(dialog, character));
    }

    public string[] GetCharacters()
    {
        if(charactersContainer == null || charactersContainer.transform.childCount == 0)
            return null;

        string[] talkingCharacterOptions = new string[charactersContainer.transform.childCount];
        for(int i = 0; i < charactersContainer.transform.childCount; i++)
            talkingCharacterOptions[i] = charactersContainer.transform.GetChild(i).name;
        return talkingCharacterOptions;
    }
}


/*public class speechTestScript : MonoBehaviour
{
    private DialogManager dialogManager;
    private GameObject printerObject, textObject;
    private RectTransform printerRectTransform, textRectTransform;

    public GameObject originalRPGFormatDialogPrefab;
    public bool flipBubbleWithCharacter = true;

    private GameObject currentTalkingCharacter;

    [SerializeField] private Dictionary<string, Sprite> speechBubbles = new Dictionary<string, Sprite>();

    void Start()
    {
        dialogManager = GameObject.Find("DialogAsset").GetComponent<DialogManager>();
        printerObject = dialogManager.gameObject.transform.GetChild(0).gameObject;
        textObject = printerObject.transform.GetChild(0).gameObject;
        printerRectTransform = printerObject.GetComponent<RectTransform>();
        textRectTransform = textObject.GetComponent<RectTransform>();

        ShowTestDialog();
    }

    void Update()
    {
        // For some reason the buttons aren't responding properly so we take keyboard input for now
        if(Input.GetKeyUp(KeyCode.Return)) {
            dialogManager.Click_Window();
        }
    }

    private void ShowTestDialog()
    {
        // Type of dialog
        dialogManager.OnPrintStartEvent += SwitchCurrentActiveTalkingCharacter;
        dialogManager.OnPrintEvent += BubbleDialog;
        dialogManager.OnDialogEndedEvent += FinishedDialog;
        //RPGDialog();

        // Conversation
        var dialogTexts = new List<DialogData>();
        dialogTexts.Add(new DialogData("Hello!", "RandomChar"));
        dialogTexts.Add(new DialogData("Hello to \nyou too!", "Character2"));
        dialogTexts.Add(new DialogData("Nice meeting you here.", "RandomChar"));
        dialogTexts.Add(new DialogData("Same!", "Character2"));
        dialogManager.Show(dialogTexts);
    }
    private void FinishedDialog()
    {
        // Unhook the functions from the events
        dialogManager.OnPrintEvent -= BubbleDialog;
        dialogManager.OnPrintFinishedEvent -= SwitchCurrentActiveTalkingCharacter;
        dialogManager.OnDialogEndedEvent -= FinishedDialog;
    }

    private void SwitchCurrentActiveTalkingCharacter()
    {
        // Get the current talking character
        currentTalkingCharacter = dialogManager.GetCurrentInGameCharacter();

        // Find the mouth position if it exists
        var characterPos = currentTalkingCharacter.transform.Find("Mouth").gameObject.GetComponent<Transform>().position;
        if (characterPos == null)
            characterPos = currentTalkingCharacter.transform.position;

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

    // New bubble dialog
    private void BubbleDialog()
    {
        textObject.GetComponent<HorizontalLayoutGroup>().enabled = true;
        textObject.GetComponent<ContentSizeFitter>().enabled = true;

        // Resize the printer
        var finalSize = textRectTransform.sizeDelta + new Vector2(80, 50);
        printerRectTransform.sizeDelta = finalSize;
    }

    // RPG Dialog available by default
    private void RPGDialog()
    {
        var po = originalRPGFormatDialogPrefab.transform.GetChild(0).gameObject;
        var to = po.transform.GetChild(0).gameObject;

        textObject.GetComponent<HorizontalLayoutGroup>().enabled = false;
        textObject.GetComponent<ContentSizeFitter>().enabled = false;

        CopyRectTransform(po.GetComponent<RectTransform>(), printerRectTransform);
        CopyRectTransform(to.GetComponent<RectTransform>(), textRectTransform);

        printerRectTransform.sizeDelta = new Vector2(printerRectTransform.sizeDelta.x, 0);
        printerRectTransform.anchoredPosition = Vector2.zero;

        //Debug.Log("sizedelta: " + printerRectTransform.sizeDelta);
        //Debug.Log("localpos: " + printerRectTransform.localPosition);
        //Debug.Log("anchoredpos : " + printerRectTransform.anchoredPosition);
    }

    private void CopyRectTransform(RectTransform copyFrom, RectTransform copyTo)
    {
        copyTo.anchorMin = copyFrom.anchorMin;
        copyTo.anchorMax = copyFrom.anchorMax;
        copyTo.anchoredPosition = copyFrom.anchoredPosition;
        copyTo.sizeDelta = copyFrom.sizeDelta;
        copyTo.pivot = copyFrom.pivot;
        copyTo.localPosition = copyFrom.localPosition;
    }
}
*/
