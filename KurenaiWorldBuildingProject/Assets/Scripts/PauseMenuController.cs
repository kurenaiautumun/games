using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    GameManagerController gameManagerController;

    [SerializeField] private GameObject pauseContainer;

    [Header("Save Section")]
    [SerializeField] private GameObject saveFileContainer;
    [SerializeField] private TMP_InputField saveFileInputField;

    [Header("Load Section")]
    [SerializeField] private GameObject loadFileContainer;
    [SerializeField] private GameObject loadFileButtonContentContainerPrefab;
    [SerializeField] private GameObject loadContentHierarchy;

    private string selectedFilePath;

    private void Start()
    {
        gameManagerController = GetComponent<GameManagerController>();
        selectedFilePath = "";

        pauseContainer.SetActive(true);
        saveFileContainer.SetActive(false);
        loadFileContainer.SetActive(false);
    }

    public void SaveButtonPressed()
    {
        saveFileContainer.SetActive(true);
        pauseContainer.SetActive(false);
    }

    public void CancelSave()
    {
        saveFileInputField.text = "";

        saveFileContainer.SetActive(false);
        pauseContainer.SetActive(true);
    }

    public void SaveMap()
    {
        var savefileName = saveFileInputField.text.Trim();
        if (savefileName.Length == 0)
            return;

        saveFileContainer.SetActive(false);
        pauseContainer.SetActive(true);

        gameManagerController.SaveCurrentMap(savefileName);
        saveFileInputField.text = "";

        gameManagerController.ResumeGame();
    }

    public void LoadButtonPressed()
    {
        loadFileContainer.SetActive(true);
        pauseContainer.SetActive(false);

        // Only get valid files
        string[] files = Directory.GetFiles(gameManagerController.baseSavePath, "*.json");

        for(int i = 0; i < files.Length; i++)
        {
            var path = files[i];
            var file = Instantiate(loadFileButtonContentContainerPrefab, loadContentHierarchy.transform);
            file.name = Path.GetFileNameWithoutExtension(path);
            file.transform.SetParent(loadContentHierarchy.transform, false);

            var btn = file.GetComponent<Button>();
            btn.GetComponentInChildren<TMP_Text>().SetText(file.name);
            btn.onClick.AddListener(() => OnLoadFileContentPressed(path, btn));
        }   
    }

    public void CancelLoad()
    {
        for (int i = loadContentHierarchy.transform.childCount - 1; i >= 0; i--)
            Destroy(loadContentHierarchy.transform.GetChild(i).gameObject);

        loadFileContainer.SetActive(false);
        pauseContainer.SetActive(true); 
    }

    private void OnLoadFileContentPressed(string path, Button btn)
    {
        selectedFilePath = path;
        var buttons = loadContentHierarchy.GetComponentsInChildren<Button>();

        for (int i = buttons.Length - 1; i >= 0; i--)
            buttons[i].interactable = true;

        btn.interactable = false;
    }

    public void LoadMap() 
    {
        if (selectedFilePath.Length <= 0)
            return;

        for (int i = loadContentHierarchy.transform.childCount - 1; i >= 0; i--)
            Destroy(loadContentHierarchy.transform.GetChild(i).gameObject);

        loadFileContainer.SetActive(false);
        pauseContainer.SetActive(true);

        gameManagerController.ClearCurrentMap();
        gameManagerController.LoadNewMap(selectedFilePath);
        gameManagerController.ResumeGame();

        if (gameManagerController.isSideModeEnabled)
            gameManagerController.SwitchViewMode(true);
    }

    public void DeleteMap()
    {
        if (selectedFilePath.Length <= 0)
            return;

        File.Delete(selectedFilePath);
        for (int i = loadContentHierarchy.transform.childCount - 1; i >= 0; i--)
            Destroy(loadContentHierarchy.transform.GetChild(i).gameObject);

        LoadButtonPressed();
    }

    public void NewMap()
    {
        gameManagerController.ClearCurrentMap();
        gameManagerController.CreateEmptyMap();
        gameManagerController.ResumeGame();

        if (gameManagerController.isSideModeEnabled)
            gameManagerController.SwitchViewMode(true);
    }

    public void Resume()
    { 
        gameManagerController.ResumeGame();
    }

    public void MainMenu()
    {
        gameManagerController.ReturnToMainMenu();
    }

}
