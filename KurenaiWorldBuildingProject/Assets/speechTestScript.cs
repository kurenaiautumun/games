using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class speechTestScript : MonoBehaviour
{
    private DialogManager dialogManager;
    private GameObject printerObject, textObject;
    private RectTransform printerRectTransform, textRectTransform;

    public GameObject originalRPGFormatDialogPrefab;
    public GameObject character;

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
        if(Input.GetKeyUp(KeyCode.Return)) {
            dialogManager.Click_Window();
        }
    }

    private void ShowTestDialog()
    {
        // Type of dialog
        dialogManager.OnPrintEvent += BubbleDialog;
        dialogManager.OnPrintStartEvent += SwitchCurrentActiveTalkingCharacter;
        //RPGDialog();

        // Conversation
        var dialogTexts = new List<DialogData>();
        dialogTexts.Add(new DialogData("Hello!", "RandomChar"));
        dialogTexts.Add(new DialogData("Hello to you too!", "Character2"));
        dialogTexts.Add(new DialogData("Nice meeting you here.", "RandomChar"));
        dialogTexts.Add(new DialogData("Same!", "Character2"));
        dialogManager.Show(dialogTexts);

        //dialogManager.OnPrintEvent -= BubbleDialog;
        //dialogManager.OnPrintFinishedEvent -= SwitchCurrentActiveTalkingCharacter;
    }

    private void SwitchCurrentActiveTalkingCharacter()
    {
        character = dialogManager.GetCurrentInGameCharacter();
    }

    private void BubbleDialog()
    {
        textObject.GetComponent<HorizontalLayoutGroup>().enabled = true;
        textObject.GetComponent<ContentSizeFitter>().enabled = true;

        var characterScreenPos = Camera.main.WorldToScreenPoint(character.transform.position);
        printerObject.transform.position = characterScreenPos + new Vector3(50, 50, 0);

        var finalSize = textRectTransform.sizeDelta + new Vector2(80, 50);
        printerRectTransform.sizeDelta = finalSize;
    }

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
