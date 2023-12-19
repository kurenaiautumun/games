// This is an extension built on top of the other Dialog Manager
// This manager is aimed for dialogs with speech bubbles while the other is more RPG oriented (print on the bottom)
// This asset is designed to be as close as possible to the original in order to preserve as many functionalities that are currently present
// This similarity also ensures that any changes made in the base manager can be extended to this too
// Also note that there are separate dialog assets and character assets for both types

using Doublsb.Dialog;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#region UNITYEDITOR
#if (UNITY_EDITOR)
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
#endif
#endregion

public class ExtendedDialogManager : MonoBehaviour
{
    private DialogManager dialogManager;
    private GameObject currentTalkingCharacter;
    private bool isInitialized = false;

    [Header("Dialog Settings")]
    public bool flipBubbleWithCharacter = true;
    public bool zoomInOnCharacters = true;
    public float zoomFactor = 1.0f;
    private float originalZoom;
    private Vector3 originalCameraPosition;

    [Header("Misc Settings")]
    public GameObject charactersContainer;

    public string Result => dialogManager.Result;
    public string currentDialogBranch;

    private class DialogSelectItem
    {
        public string Name;
        public string BranchName;
        public string Dialog;
        public bool ZoomIn;
    }

    private class DialogItem
    {
        public string Character;
        public string Name;
        public string Dialog;
        public string ZoomIn;
        public string SelectCharacter;
        public List<DialogSelectItem> SelectList;
    }

    private class DialogBranch
    {
        public string Name;
        public List<DialogItem> Dialogs;
    }

    private void Start()
    {
        isInitialized = false;

        dialogManager = GetComponent<DialogManager>();

        UpdateCameraProperties();

        if (dialogManager != null)
            isInitialized = true;
    }


    //================================================
    //Public Methods
    //================================================

    // We only need to hook the correct functions to enable the printer to correctly display the bubbles
    public void Show(DialogData Data)
    {
        dialogManager.OnPrintStartEvent += SwitchCurrentActiveTalkingCharacter;
        dialogManager.OnDialogEndedEvent += FinishedDialog;
        dialogManager.OnDialogBranchEndedEvent += FinishedDialogBranch;

        dialogManager.Show(Data);
    }

    public void Show(List<DialogData> Data, string branchName = "")
    {
        currentDialogBranch = branchName;
        dialogManager.OnPrintStartEvent += SwitchCurrentActiveTalkingCharacter;
        dialogManager.OnDialogEndedEvent += FinishedDialog;
        dialogManager.OnDialogBranchEndedEvent += FinishedDialogBranch;

        dialogManager.Show(Data);
    }

    public bool IsInitialized() { return isInitialized; }

    // Call this if for some reason dialogManager is null (or isInitialized is false)
    public void Initialize()
    {
        dialogManager = GetComponent<DialogManager>();
        if (dialogManager != null)
            isInitialized = true;
    }

    public void SetZoomInOnCharacters(bool zoomIn)
    {
        zoomInOnCharacters = zoomIn;
    }

    public void LoadFromJson(string dataPath, string branchName = null)
    {
        try
        {
            Dictionary<string, List<DialogData>> data = new Dictionary<string, List<DialogData>>();

            string json = ((TextAsset)Resources.Load(dataPath)).text;
            List<DialogBranch> items = JsonConvert.DeserializeObject<List<DialogBranch>>(json);

            foreach (DialogBranch item in items)
            {
                List<DialogData> dialogs = new List<DialogData>();
                foreach (DialogItem jsonDialog in item.Dialogs)
                {
                    DialogData dialogData = new DialogData(jsonDialog.Dialog, jsonDialog.Character, jsonDialog.Name);
                    if (jsonDialog.Name != null)
                    {
                        dialogData.Commands.Insert(0, new DialogCommand(Command.sound, jsonDialog.Name));
                    }

                    if (jsonDialog.ZoomIn != null)
                    {
                        dialogData.zoomIn = bool.Parse(jsonDialog.ZoomIn);
                    }

                    if (jsonDialog.SelectCharacter != null)
                    {
                        if (jsonDialog.SelectList.Count > 0)
                        {
                            dialogData.SelectList.Character = jsonDialog.SelectCharacter;

                            foreach (DialogSelectItem selectOption in jsonDialog.SelectList)
                            {
                                dialogData.SelectList.Add(selectOption.BranchName, selectOption.Dialog, selectOption.Name, selectOption.ZoomIn);
                            }

                            dialogData.Callback = () =>
                            {
                                if (data.TryGetValue(Result, out List<DialogData> dialogItems))
                                {
                                    Show(dialogItems, Result);
                                }
                                else
                                {
                                    LoadFromJson(dataPath.Substring(0, dataPath.LastIndexOf('/') + 1) + Result, Result);
                                }
                            };
                        }
                    }

                    dialogs.Add(dialogData);
                }
                data.Add(item.Name, dialogs);
            }

            Show(data["Main"], branchName ?? "Main");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception: " + e.Message);
        }
    }

    public void UpdateCameraProperties()
    {
        originalZoom = Camera.main.orthographicSize;
        originalCameraPosition = Camera.main.transform.position;
    }

    public void ResetToOriginalCameraProperties()
    {
        Camera.main.orthographicSize = originalZoom;
        Camera.main.transform.position = originalCameraPosition;
    }

    public void ResetToOriginalZoom()
    {
        Camera.main.orthographicSize = originalZoom;
    }

    public void SwitchCurrentActiveTalkingCharacter()
    {
        // Get the current talking character
        currentTalkingCharacter = dialogManager.GetCurrentInGameCharacter();

        GameManagerController gameManagerController = GameObject.Find("GameManager")?.GetComponent<GameManagerController>();
        var characterPos = gameManagerController?.isSideModeEnabled ?? false ? currentTalkingCharacter.transform.position : currentTalkingCharacter.GetComponent<GridObject>().altViewObject.transform.position;

        // Find the mouth position if it exists
        var mouth = currentTalkingCharacter.transform.Find("Mouth");
        if (mouth != null)
            characterPos = mouth.gameObject.GetComponent<Transform>().position;

        // Zoom into the characters
        if (dialogManager.ZoomInOnCurrentCharacter())
        {
            Camera.main.orthographicSize = originalZoom / zoomFactor;
            Camera.main.transform.position = new Vector3(characterPos.x, characterPos.y, Camera.main.transform.position.z);
        }
        else
        {
            if (Camera.main.orthographicSize != originalZoom || Camera.main.transform.position != originalCameraPosition)
            {
                ResetToOriginalCameraProperties();
            }
        }
    }

    public delegate void _dialogendeddelegate(string name);
    public event _dialogendeddelegate DialogEndedEvent;

    public delegate void _dialogbranchendeddelegate(string name);
    public event _dialogbranchendeddelegate DialogBranchEndedEvent;

    //================================================
    //Private Methods
    //================================================

    private void FinishedDialog(string name)
    {
        if (DialogEndedEvent != null)
            DialogEndedEvent(name);
    }

    private void FinishedDialogBranch()
    {
        if (currentDialogBranch != "" && DialogBranchEndedEvent != null)
        {
            DialogBranchEndedEvent(currentDialogBranch);
        }
        currentDialogBranch = "";
        // Unhook the functions from the events
        dialogManager.OnPrintStartEvent -= SwitchCurrentActiveTalkingCharacter;
        dialogManager.OnDialogEndedEvent -= FinishedDialog;
        dialogManager.OnDialogBranchEndedEvent -= FinishedDialogBranch;

        // Fix the camera zoom and position
        if (dialogManager.ZoomInOnCurrentCharacter())
        {
            Camera.main.orthographicSize = originalZoom;
            Camera.main.transform.position = originalCameraPosition;
        }
    }
}
