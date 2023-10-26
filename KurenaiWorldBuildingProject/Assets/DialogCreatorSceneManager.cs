using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DialogCreator
{
    public class DialogNode
    {
        public string dialog;
        public GameObject character;
        public bool isFocusedOnCharacter;

        public DialogNode() { }
        public DialogNode(string dialog, GameObject character, bool isFocusedOnCharacter)
        {
            this.dialog = dialog;
            this.character = character;
            this.isFocusedOnCharacter = isFocusedOnCharacter;
        }

        public override string ToString()
        { return "character: " + character + "\ndialog: " + dialog + "\nfocused: " + isFocusedOnCharacter; }
    }

    public class DialogCreatorSceneManager : MonoBehaviour
    {
        enum DialogMode { BUBBLE, RPG };

        [Header("Input Section")]
        [SerializeField] private float zoomSpeed = 1f;
        [SerializeField] private float minZoom = 1f;
        [SerializeField] private float maxZoom = 10f;
        private float zoom = 1f;

        [Header("Ui Section")]
        [SerializeField] private Canvas canvas;
        private GameObject topPanelContainer, bottomPanelContainer;
        private TMP_Text currentIdxTxt, totalCountTxt;

        private List<DialogNode> dialogList;
        private List<GameObject> characterList;
        private int currentDialogIndex;
        private DialogMode dialogMode;
        private DialogNode currentDialogNode;
        private GameObject currentCharacter;
        private GameObject printerObject, dialogueTextObject;
        private Text dialogueText;
        private InputField dialogueTextInputField;
        private RectTransform printerRectTransform, textRectTransform;
        private bool flipBubbleWithCharacter;
        private Toggle zoomOnCharacterToggle;

        private GameObject loggerPanel, notifPanel;
        private TMP_Text loggerText, notifText;
        private float loggerPanelTimer, loggerPanelTimeout, notifPanelTimer, notifPanelTimeout;
        private bool isLoggerPanelShown, isNotifPanelShown;

        private TMP_InputField characterPrintDelayInputField;
        private float characterPrintDelay;
        private bool isPreviewing;

        DialogManager dialogManager;

        [Header("Misc Section")]
        [SerializeField] private GameObject charactersContainer;
        [SerializeField] private GameObject saveSceneObjectContainer;

        void Start()
        {
            if(canvas == null)
            {
                Debug.LogError("Please reference the Canvas.");
                return;
            }
            if (charactersContainer == null)
            {
                Debug.LogError("Please reference the Characters GameObject.");
                return;
            }

            zoom = Camera.main.orthographicSize;

            InputManager.ScrollEvent += ZoomCamera;
            InputManager.PinchEvent += ZoomCamera;
            InputManager.OnDragEvent += MoveCamera;
            InputManager.OnSingleTapEvent += HandleSelection;

            dialogList = new List<DialogNode>();
            characterList = new List<GameObject>();
            dialogList.Add(new DialogNode("", null, false));

            currentDialogIndex = 0;
            dialogMode = DialogMode.BUBBLE;
            currentDialogNode = dialogList[0];
            currentCharacter = null;
            flipBubbleWithCharacter = true;

            topPanelContainer = canvas.transform.Find("TopPanel").gameObject;
            bottomPanelContainer = canvas.transform.Find("BottomPanel").gameObject;

            currentIdxTxt = bottomPanelContainer.transform.Find("CurrentIdxTxt").GetComponent<TMP_Text>();
            totalCountTxt = bottomPanelContainer.transform.Find("TotalCountTxt").GetComponent<TMP_Text>();

            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);

            printerObject = transform.GetChild(0).GetChild(0).gameObject;
            dialogueTextObject = printerObject.transform.GetChild(1).gameObject;
            dialogueText = dialogueTextObject.GetComponent<Text>();
            dialogueTextInputField = dialogueTextObject.GetComponent<InputField>();
            printerRectTransform = printerObject.GetComponent<RectTransform>();
            textRectTransform = dialogueTextObject.GetComponent<RectTransform>();

            characterPrintDelayInputField = bottomPanelContainer.transform.Find("CharacterPrintDelayInputField").GetComponent<TMP_InputField>();
            characterPrintDelay = 0f;
            isPreviewing = false;

            zoomOnCharacterToggle = bottomPanelContainer.transform.Find("ZoomToggleBtn").GetComponent<Toggle>();

            dialogManager = transform.GetChild(0).GetComponent<DialogManager>();

            notifPanel = topPanelContainer.transform.Find("NotificationPanel").gameObject;
            notifText = notifPanel.transform.GetChild(0).GetComponent<TMP_Text>();
            notifPanelTimer = 0f;
            notifPanelTimeout = 3f;
            isNotifPanelShown = false;

            loggerPanel = topPanelContainer.transform.Find("LoggerPanel").gameObject;
            loggerText = loggerPanel.transform.GetChild(0).GetComponent<TMP_Text>();
            loggerPanelTimer = 0f;
            loggerPanelTimeout = 5f;
            isLoggerPanelShown = false;

            for (int i=0;i<charactersContainer.transform.childCount;i++)
            {
                characterList.Add(charactersContainer.transform.GetChild(i).gameObject);
                characterList[i].gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
            }

            currentCharacter = characterList[0];
            SwitchCurrentCharacterDialog();
            currentCharacter = null;
            ResizePrinter();
        }

        private void OnDestroy()
        {
            InputManager.ScrollEvent -= ZoomCamera;
            InputManager.PinchEvent -= ZoomCamera;
            InputManager.OnDragEvent -= MoveCamera;
            InputManager.OnSingleTapEvent -= HandleSelection;
        }

        private void OnDisable()
        {

        }

        private void Update()
        {
            if (isLoggerPanelShown)
            {
                loggerPanelTimer += Time.deltaTime;
                if(loggerPanelTimer > loggerPanelTimeout) 
                { 
                    loggerPanelTimer = 0f;
                    isLoggerPanelShown = false;
                    loggerPanel.SetActive(false);
                }
            }

            if (isNotifPanelShown)
            {
                notifPanelTimer += Time.deltaTime;
                if (notifPanelTimer > notifPanelTimeout)
                {
                    notifPanelTimer = 0f;
                    isNotifPanelShown = false;
                    notifPanel.SetActive(false);
                }
            }
        }

        private void PrintToLog(string s, float timeout = 5f)
        {
            loggerText.text = s;
            loggerPanel.SetActive(true);
            isLoggerPanelShown = true;
            loggerPanelTimer = 0f;
            loggerPanelTimeout = timeout;
        }
        private void PrintToNotification(string s, float timeout = 3f)
        {
            notifText.text = s;
            notifPanel.SetActive(true);
            isNotifPanelShown = true;
            notifPanelTimer = 0f;
            notifPanelTimeout = timeout;
        }

        private void ZoomCamera(float deltaValue)
        {
            if (deltaValue < 0)
            {
                zoom = Mathf.Min(zoom + zoomSpeed, maxZoom);
                Camera.main.orthographicSize = zoom;
            }
            else if (deltaValue > 0)
            {
                zoom = Mathf.Max(zoom - zoomSpeed, minZoom);
                Camera.main.orthographicSize = zoom;
            }
        }

        private void MoveCamera(Vector2 displacement)
        {
            if (currentCharacter != null)
                return;

            Vector3 camPos = Camera.main.transform.position;
            camPos -= new Vector3(displacement.x, displacement.y, 0) * Time.deltaTime;
            Camera.main.transform.position = camPos;
        }

        private void HandleSelection(Vector2 pos)
        {
            var touchWorldPos = Camera.main.ScreenToWorldPoint(pos);
            Collider2D collider = Physics2D.OverlapPoint(touchWorldPos);

            if (collider == null)
            {
                if (currentCharacter != null && !InputManager.isCanvasElementInteracted)
                {
                    currentCharacter.GetComponent<SpriteRenderer>().color = Color.gray;
                    currentCharacter = null;
                    currentDialogNode.character = null;
                    InputManager.OnDragEvent -= RelocateCharacter;
                    InputManager.BeginDragEvent -= StartDragCharacter;
                    InputManager.EndDragEvent -= EndDragCharacter;
                }
            }
            else
            {
                if (currentCharacter == collider.gameObject)
                    return;

                if(currentCharacter != null)
                {
                    currentCharacter.GetComponent<SpriteRenderer>().color = Color.gray;
                    InputManager.OnDragEvent -= RelocateCharacter;
                    InputManager.BeginDragEvent -= StartDragCharacter;
                    InputManager.EndDragEvent -= EndDragCharacter;
                }                 

                currentCharacter = collider.gameObject;
                currentCharacter.GetComponent<SpriteRenderer>().color = Color.white;
                currentDialogNode.character = currentCharacter;
                PrintToNotification(currentDialogNode.ToString(), 1f);
                SwitchCurrentCharacterDialog();
          
                InputManager.OnDragEvent += RelocateCharacter;
                InputManager.BeginDragEvent += StartDragCharacter;
                InputManager.EndDragEvent += EndDragCharacter;
            }
        }

        private void SwitchCurrentCharacterDialog()
        {
            var characterPos = currentCharacter.transform.Find("Mouth").gameObject.GetComponent<Transform>().position;
            if (characterPos == null)
                characterPos = currentCharacter.transform.position;

            var bubbleScreenPos = Camera.main.WorldToScreenPoint(characterPos);
            printerObject.transform.position = bubbleScreenPos;

            if (flipBubbleWithCharacter)
            {
                var pls = printerObject.transform.localScale;
                var tls = dialogueTextObject.transform.localScale;
                if (currentCharacter.GetComponent<SpriteRenderer>().flipX)
                {
                    printerObject.transform.localScale = new Vector3(-1, 1, 1);
                    dialogueTextObject.transform.localScale = new Vector3(-1, 1, 1);
                    textRectTransform.pivot = new Vector3(1, 0.5f, 0);
                }
                else
                {
                    printerObject.transform.localScale = Vector3.one;
                    dialogueTextObject.transform.localScale = Vector3.one;
                    textRectTransform.pivot = new Vector3(0, 0.5f, 0);
                }
            }
        }

        private void ResizePrinter()
        {
            var finalSize = textRectTransform.sizeDelta + new Vector2(80, 50);
            printerRectTransform.sizeDelta = finalSize;
        }

        private void RelocateCharacter(Vector2 delta)
        {
            currentCharacter.transform.Translate(delta * Time.deltaTime);
        }

        private void StartDragCharacter()
        {
            printerObject.SetActive(false);
        }

        private void EndDragCharacter()
        {
            printerObject.SetActive(true);
            SwitchCurrentCharacterDialog();
            ResizePrinter();
        }

        // UI Operations

        public void OnPrevDialogBtnClicked()
        {
            if(--currentDialogIndex < 0)
                currentDialogIndex = dialogList.Count - 1;
            currentIdxTxt.SetText("Current Dialog Index: " + currentDialogIndex.ToString());
            currentDialogNode = dialogList[currentDialogIndex];
            LoadCurrentDialogNode();
        }

        public void OnNextDialogBtnClicked()
        {
            if(++currentDialogIndex >= dialogList.Count)
                currentDialogIndex = 0;
            currentIdxTxt.SetText("Current Dialog Index: " + currentDialogIndex.ToString());
            currentDialogNode = dialogList[currentDialogIndex];
            LoadCurrentDialogNode();
        }

        private void LoadCurrentDialogNode()
        {
            if (currentDialogNode.character == null)
            {
                //PrintToLog("The current character is not present for dialog index: " + currentDialogIndex);
                if (currentCharacter != null)
                    currentCharacter.GetComponent<SpriteRenderer>().color = Color.gray;
                dialogueText.text = "Enter here...";
                dialogueTextInputField.text = "Enter here...";
                return;
            }

            if (currentCharacter != currentDialogNode.character)
            {
                if (currentCharacter != null)
                    currentCharacter.GetComponent<SpriteRenderer>().color = Color.gray;
                currentCharacter = currentDialogNode.character;
                currentCharacter.GetComponent<SpriteRenderer>().color = Color.white;
            }

            dialogueText.text = currentDialogNode.dialog;
            dialogueTextInputField.text = currentDialogNode.dialog;
            zoomOnCharacterToggle.isOn = currentDialogNode.isFocusedOnCharacter;

            SwitchCurrentCharacterDialog();
            ResizePrinter();
        }

        public void OnAddDialogBtnClicked()
        {
            if (currentCharacter == null)
            {
                PrintToLog("Choose a character for this dialog");
                return;
            }

            currentDialogNode.dialog = dialogueText.text;
            currentDialogNode.character = currentCharacter;

            dialogList.Add(new DialogNode("", null, false));
            currentDialogIndex = dialogList.Count - 1;
            currentDialogNode = dialogList[currentDialogIndex];
            currentCharacter.GetComponent<SpriteRenderer>().color = Color.gray;
            currentCharacter = null;
            dialogueText.text = "Enter here...";
            dialogueTextInputField.text = "Enter here...";
            ResizePrinter();

            totalCountTxt.SetText("Total Dialog Count: " + dialogList.Count);
        }

        public void OnPreviewBtnClicked()
        {
            if (isPreviewing) return;
            if (currentDialogNode.character == null || currentDialogNode.dialog == "")
            {
                PrintToLog("No character or dialog present!", 1f);
                return;
            }

            List<DialogData> data = new List<DialogData>
            {
                new DialogData("/speed:" + characterPrintDelay + "/" + currentDialogNode.dialog, currentDialogNode.character.name)
            };

            dialogueTextInputField.enabled = false;
            printerObject.GetComponent<Button>().interactable = true;
            isPreviewing = true;

            dialogManager.OnDialogEndedEvent += OnPreviewDialogEnded;
            dialogManager.Show(data);
        }

        public void OnFullPreviewBtnClicked()
        {
            if (isPreviewing) return;

            List<DialogData> data = new List<DialogData>();
            for(int nodeIdx = 0; nodeIdx < dialogList.Count; nodeIdx++)
            {
                DialogNode node = dialogList[nodeIdx];
                if (node.character == null || node.dialog == "")
                {
                    PrintToLog("No character or dialog present in dialog index: " + nodeIdx, 1f);
                    return;
                }
                data.Add(new DialogData("/speed:" + characterPrintDelay + "/" + node.dialog, node.character.name));
            }

            dialogueTextInputField.enabled = false;
            printerObject.GetComponent<Button>().interactable = true;
            isPreviewing = true;

            dialogManager.OnDialogEndedEvent += OnPreviewDialogEnded;
            dialogManager.Show(data);
        }

        public void OnZoomToggleBtnValueChanged()
        {
            currentDialogNode.isFocusedOnCharacter = zoomOnCharacterToggle.isOn;
            PrintToNotification(currentDialogNode.ToString(), 1f);
        }


        public void OnDialogTypeToggleBtnValueChanged(bool value)
        {
            dialogMode = value ? DialogMode.BUBBLE : DialogMode.RPG;
        }

        public void OnPrinterButtonClicked()
        {
            if (!isPreviewing) return;
            dialogManager.Click_Window();

        }

        public void OnDialogTextInputFieldValueChanged()
        {
            ResizePrinter();
        }

        public void OnDialogTextInputFieldEndEdit()
        {
            currentDialogNode.dialog = dialogueText.text;
            PrintToNotification(currentDialogNode.ToString(), 1f);
        }

        public void OnCharacterPrintDelayInputFieldEndEdit()
        {
            if (characterPrintDelayInputField.text.Trim().Length == 0)
            {
                characterPrintDelay = 0f;
                characterPrintDelayInputField.text = "0";
            }             
            else
                characterPrintDelay = float.Parse(characterPrintDelayInputField.text);
        }

        public void OnSaveSceneBtnClicked()
        {
            string prefabPath = "Assets/Prefabs/Level.prefab";
            PrefabUtility.SaveAsPrefabAssetAndConnect(saveSceneObjectContainer, prefabPath, InteractionMode.UserAction);
            string textPath = "Assets/Prefabs/commands.txt";
            using (StreamWriter sw = File.CreateText(textPath))
            {
                sw.WriteLine("List<DialogData> data = new List<DialogData>();");
                foreach (var node in dialogList)
                {
                    sw.WriteLine("data.Add(new DialogData(\"/speed:" + characterPrintDelay + "/" + node.dialog + "\" , " + node.character.name+"));");
                }
                sw.WriteLine("dialogManager.Show(dialogTexts);");
            }
        }

        private void OnPreviewDialogEnded()
        {
            isPreviewing = false;
            printerObject.GetComponent<Button>().interactable = false;
            dialogueTextInputField.enabled = true;

            dialogManager.OnDialogEndedEvent -= OnPreviewDialogEnded;

            printerObject.SetActive(true);
        }
    }
}