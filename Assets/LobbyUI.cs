using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Lobbies.Models;
using System;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button JoinTab;
    [SerializeField] private Button HostTab;
    [SerializeField] private Button SearchTab;
    private GameObject prevPanel;
    private Button prev;

    [SerializeField] private GameObject JoinPanel;
    [SerializeField] private GameObject HostPanel;
    [SerializeField] private GameObject SearchPanel;

    [SerializeField] private Button create;
    [SerializeField] private Button join;
    [SerializeField] private Button joinCode;

    [SerializeField] private TMP_InputField lobbyNameField; 
    [SerializeField] private TMP_InputField lobbyCodeField; 
    [SerializeField] private TMP_InputField nicknameField; 
    

    [SerializeField] private Transform LobbyContainer;
    [SerializeField] private Transform LobbyTemplate;

    [SerializeField] private Animator CanvasAnimator;

    [Header("HostPanel")]
    [SerializeField] private TMP_InputField PointsInputField;
    public int[] LobbyPointsList = new int[]{60,120,240,300};
    int PointID = 0;

    [SerializeField] private Image IsPrivateButton;
    private bool isPrivateLobby;
    [SerializeField] Sprite[] lockSprites;

    [Header("SearchPanel")]
    [SerializeField] private GameObject SearchResults;
    [SerializeField] private GameObject EmptyResults;
    [Header("SkinMenu")]
    [SerializeField] private Button BackButton;
    [SerializeField] private GameObject HairStyles;
    [SerializeField] private GameObject EyeStyles;
    [SerializeField] private Button HairButton;
    [SerializeField] private Button EyeButton;
    [SerializeField] private Button ProfileButton;
    [SerializeField] private Transform SkinTemplate;

   

    public static LobbyUI Instance;

    private void Awake() {
        prev = SearchTab;
        prevPanel = SearchPanel;
        SkinTemplate.gameObject.SetActive(false);
        Instance = this;
        BackButton.onClick.AddListener(()=>{
            CanvasAnimator.Play("SkinBackScreen");
            SaveSkinDetails();
        });
        ProfileButton.onClick.AddListener(()=>{
            CanvasAnimator.Play("SkinScreen");
        });
        EyeButton.onClick.AddListener(()=>{
            HairStyles.SetActive(false);
            EyeStyles.SetActive(true);
        });
        HairButton.onClick.AddListener(()=>{
            HairStyles.SetActive(true);
            EyeStyles.SetActive(false);
        });
        create.onClick.AddListener(()=> {
            FadeOutScroller();
            LobbyManager.Instance.CreateLobby(lobbyNameField.text, isPrivateLobby, LobbyPointsList[PointID]);
        });
        join.onClick.AddListener(()=> {
            FadeOutScroller();
            LobbyManager.Instance.QuickJoin();});
        joinCode.onClick.AddListener(()=> {
            FadeOutScroller();
            LobbyManager.Instance.JoinCode(lobbyCodeField.text);});
        LobbyTemplate.gameObject.SetActive(false);
        

    }
    
    private void Start() {
        GetSkinSavedPrefs();
        nicknameField.text = GameState.Instance.getPlayerName();
        nicknameField.onValueChanged.AddListener((string newText) => {
            GameState.Instance.setPlayerName(newText);
        });

        LobbyManager.Instance.OnLobbyListChanged += LobbyListChanger;
        UpdateLobbyList(new List<Lobby>());

        setupEyeSkins();
        setupHairSkins();
    }
    private void GetSkinSavedPrefs(){
        int hair = PlayerPrefs.GetInt("HairID",0);
        int eyes = PlayerPrefs.GetInt("EyeID",0);
        ProfileButton.transform.Find("Hair").GetComponent<SpriteRenderer>().sprite = Skins.GetInstance().getHair(hair);
        ProfileButton.transform.Find("Eyes").GetComponent<SpriteRenderer>().sprite = Skins.GetInstance().getEye(eyes);
        SkinVisual.HairID = hair;
        SkinVisual.EyeID = eyes;
    }
    private void SaveSkinDetails(){
        PlayerPrefs.SetInt("HairID", SkinVisual.HairID);
        PlayerPrefs.SetInt("EyeID", SkinVisual.EyeID);
    }
    private void LobbyListChanger(object sender, LobbyManager.OnLobbyListChangedEventsArgs e)
    {
        UpdateLobbyList(e.LobbyList);
    }

    private void setupHairSkins(){
        List<Sprite> list = Skins.GetInstance().getHairList();
        Transform container = HairStyles.transform.GetChild(0);
        int i =0;
        foreach(Sprite s in list){
            Transform tr = Instantiate(SkinTemplate,container);
            tr.GetChild(0).GetComponent<SpriteRenderer>().sprite = s;
            tr.GetComponent<SkinVisual>().setSettings(1,i++);
            tr.gameObject.SetActive(true);
        }
    }
    private void setupEyeSkins(){
        List<Sprite> list = Skins.GetInstance().getEyeList();
        Transform container = EyeStyles.transform.GetChild(0);
        int i = 0;
        foreach(Sprite s in list){
            Transform tr = Instantiate(SkinTemplate,container);
            tr.GetChild(0).GetComponent<SpriteRenderer>().sprite = s;
            tr.GetComponent<SkinVisual>().setSettings(0,i++);
            tr.gameObject.SetActive(true);
        }
    }

    private void UpdateLobbyList(List<Lobby> LobbyList){
        foreach(Transform t in LobbyContainer){
            if(t == LobbyTemplate) continue;
            Destroy(t.gameObject);
        }
        
        foreach(Lobby l in LobbyList){
            Transform lobbyTr = Instantiate(LobbyTemplate, LobbyContainer);
            lobbyTr.gameObject.SetActive(true);
            lobbyTr.GetComponent<LobbyTemplate>().SetLobby(l);
        }
        UpdateSearchResultsVisual(LobbyList.Count == 0);

    }

    private void OnDestroy() {
        LobbyManager.Instance.OnLobbyListChanged -= LobbyListChanger;
    }

    public void FadeOutScroller(){
        //ScrollerAnimator.gameObject.GetComponent<Scroller>().activated = true;
        CanvasAnimator.Play("ThirdScreen");
    }

    public void PopUpAnimation(Animator anim){
        if(anim.gameObject.name == prev.gameObject.name){return;}
        anim.Play("ButtonsOn");
        prev.GetComponent<Animator>().Play("ButtonsOff");
        prev = anim.GetComponent<Button>();
        string name = anim.gameObject.name;
        prevPanel.SetActive(false);
        switch(name){
            case "Join":
                prevPanel = JoinPanel;
                break;
            case "Host":
                prevPanel = HostPanel;
                break;
            case "Search":
                prevPanel = SearchPanel;
                break;

        }
        prevPanel.SetActive(true);
    }

    public void Click(GameObject go){
        Destroy(go);
        GameObject.Find("Canvas").GetComponent<Animator>().Play("SecondScreen");
    }

    public void ChangePointsText(int points){
        PointsInputField.text = points.ToString();
    }

    public void changeLeft(){
        PointID -= 1;
        if(PointID < 0){
            PointID = LobbyPointsList.Length - 1;
        }

        ChangePointsText(LobbyPointsList[PointID]);
    }
    public void changeRight(){
        PointID += 1;
        if(PointID > LobbyPointsList.Length - 1){
            PointID = 0;
        }

        ChangePointsText(LobbyPointsList[PointID]);
    }

    public void togglePrivate(){
        isPrivateLobby = !isPrivateLobby;
        changeToggleImage();

    }
    private void changeToggleImage(){
        if(isPrivateLobby){
            IsPrivateButton.sprite = lockSprites[0];
        }else{
            IsPrivateButton.sprite = lockSprites[1];
        }
    }

    private void UpdateSearchResultsVisual(bool isEmpty){
        EmptyResults.SetActive(isEmpty);
        SearchResults.SetActive(!isEmpty);
    }
}
