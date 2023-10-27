using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

#region UNITYEDITOR

[CustomEditor(typeof(speechTestScript))]
public class speechTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        speechTestScript myTarget = (speechTestScript)target;

        if (GUILayout.Button("Test Speech"))
        {
            myTarget.TestSpeech();
        }
    }
}

#endregion

public class speechTestScript : MonoBehaviour
{
    // Remember to get a reference to the extended variant for speech bubbles
    private ExtendedDialogManager dialogManager;
    private void Start()
    {
        dialogManager = GameObject.Find("DialogAssetBubble Variant").GetComponent<ExtendedDialogManager>();
    }

    public void TestSpeech()
    {
        // Follows a similar structure to the normal dialog manager in order to be easy to learn and use
        var dialogTexts = new List<DialogData>();
        dialogTexts.Add(new DialogData("Hello! My name is\n/size:up//speed:down//color:red/ALICE/color:black//size:init//speed:init/", "Girl"));
        dialogTexts.Add(new DialogData("Hello to you too!\n I'm a /size:120//color:green/COOK/color:black//size:init/", "Cook"));
        dialogTexts.Add(new DialogData("Nice meeting you here.", "Girl"));
        dialogTexts.Add(new DialogData("Same!", "Cook"));
        dialogManager.Show(dialogTexts);
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
