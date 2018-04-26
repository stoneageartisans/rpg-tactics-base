using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Button characterButton;
    public Button readyButton;
    public Button resumeButton;
    public Button viewCharacterButton;

    public GameObject playerToken;
    public GameObject opponentToken;

    public GameObject characterPanel;
    public GameObject inGamePanel;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject viewCharacterPanel;
    public GameObject warningDialog;

    public Text[] actionPoints;
    public Text[] agility;
    public Text[] brains;
    public Text[] brawn;
    public Text[] damage;
    public Text[] defense;
    public Text[] hitPoints;
    public Text[] stamina;
    public Text[] unspentPoints;
    public Text[] weapon;

    public Toggle musicToggle;
    public Toggle sfxToggle;

    bool resumable = false;

    ArrayList roundOrder;

    Character playerCharacter;
    Character modifiedCharacter;

    Constants.GameState currentState;
    Constants.GameState nextState = Constants.GameState.MainMenu;
    Constants.GameState previousState = Constants.GameState.MainMenu;
    Constants.GameState resumeState = Constants.GameState.MainMenu;

    int currentTurn = -1;
    int round = 1;

    MonoBehaviour cameraControls;

    List<NonPlayerCharacter> opponents;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

	void Start()
    {
        loadSettings();
        initializePlayer();
        loadProgress();
        initializeOpponents();
        initializeCamera();
        setGameState(Constants.GameState.MainMenu);
	}
	
	void Update()
    {
        handleInput();
	}

    void applyGameState()
    {
        switch(currentState)
        {
            case Constants.GameState.MainMenu:
                disableCameraControls();
                hideViewCharacterPanel();
                hideInGamePanel();
                hideSettings();
                hideCharacterPanel();
                hideWarningDialog();
                showMainMenu();
                break;
            case Constants.GameState.Settings:
                hideMainMenu();
                showSettings();
                break;
            case Constants.GameState.WarningDialog:
                hideMainMenu();
                showWarningDialog();
                break;
            case Constants.GameState.CharacterCreation:
                nextState = Constants.GameState.PreRound;
                previousState = Constants.GameState.MainMenu;
                modifiedCharacter = new Character(playerCharacter);
                hideWarningDialog();
                hideMainMenu();
                updateCharacterPanel(Constants.Modify, ref modifiedCharacter);
                showCharacterPanel();
                break;
            case Constants.GameState.PreRound:
                resumeState = Constants.GameState.PreRound;
                resumable = true;
                hideMainMenu();
                hideCharacterPanel();
                saveProgress();
                showInGamePanel();
                break;
            case Constants.GameState.CharacterModification:
                nextState = resumeState;
                previousState = resumeState;
                modifiedCharacter = new Character(playerCharacter);
                disableCameraControls();
                hideInGamePanel();
                updateCharacterPanel(Constants.Modify, ref modifiedCharacter);
                showCharacterPanel();
                break;
            case Constants.GameState.InCombat:
                resumeState = Constants.GameState.InCombat;
                hideMainMenu();
                hideViewCharacterPanel();
                showInGamePanel();
                conductCombat();
                break;
            case Constants.GameState.ViewCharacter:
                disableCameraControls();
                hideInGamePanel();
                updateCharacterPanel(Constants.View, ref playerCharacter);
                showViewCharacterPanel();
                break;
        }
    }

    public void applyMusicSetting()
    {
        if(musicToggle.isOn)
        {
            PlayerPrefs.SetInt("musicOn", 1);
        }
        else
        {
            PlayerPrefs.SetInt("musicOn", 0);
        }
    }

    public void applySfxSetting()
    {
        if(sfxToggle.isOn)
        {
            PlayerPrefs.SetInt("sfxOn", 1);
        }
        else
        {
            PlayerPrefs.SetInt("sfxOn", 0);
        }
    }

    void conductCombat()
    {
        // TODO
    }

    void decreaseStat(ref int stat, int min)
    {
        if(stat > min)
        {
            stat --;
            modifiedCharacter.unspentPoints ++;
        }

        modifiedCharacter.calculateStats();
        updateCharacterPanel(Constants.Modify, ref modifiedCharacter);
    }

    public void decreaseStat(string stat)
    {
        switch(stat.ToLower())
        {
            case "brawn":
                decreaseStat(ref modifiedCharacter.brawn, modifiedCharacter.brawnMin);
                break;
            case "agility":
                decreaseStat(ref modifiedCharacter.agility, modifiedCharacter.agilityMin);
                break;
            case "brains":
                decreaseStat(ref modifiedCharacter.brains, modifiedCharacter.brainsMin);
                break;
            case "stamina":
                decreaseStat(ref modifiedCharacter.stamina, modifiedCharacter.staminaMin);
                break;
        }
    }

    public void determineInitiative()
    {
        roundOrder = new ArrayList();

        int[] ids = new int[round + 1];
        float[] initiatives = new float[round + 1];

        ids[0] = playerCharacter.id;
        initiatives[0] = playerCharacter.getInitiative();

        foreach(NonPlayerCharacter opponent in opponents)
        {
            ids[opponent.id] = opponent.id;
            initiatives[opponent.id] = opponent.getInitiative();
        }

        int tempId = 0;
        float tempInitiative = 0;

        for(int i = round; i > -1; i --)
        {
            for(int j = round - 1; j > -1; j --)
            {
                if(initiatives[j + 1] < initiatives[j])
                {
                    tempId = ids[j + 1];
                    ids[j + 1] = ids[j];
                    ids[j] = tempId;

                    tempInitiative = initiatives[j + 1];
                    initiatives[j + 1] = initiatives[j];
                    initiatives[j] = tempInitiative;
                }
            }
        }

        for(int i = 0; i <= round; i ++)
        {
            roundOrder.Insert(i, ids[i]);
        }

        roundOrder.Reverse();

        Debug.Log("Player initiative = " + playerCharacter.getInitiative());
        for(int i = 0; i < opponents.Count; i++)
        {
            Debug.Log("Opponent #" + opponents[i].id + " initiative = " + opponents[i].getInitiative());
        }
        Debug.Log(roundOrder.ToString());
    }

    void disableCameraControls()
    {
        cameraControls.enabled = false;
        showMousePointer();
    }

    void enableCameraControls()
    {
        hideMousePointer();
        cameraControls.enabled = true;
    }

    public void exit()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }

    int getDiceRoll()
    {
        // Roll 2 "11-sided" dice
        int die1 = Random.Range(0, 11); // 0 to 10
        int die2 = Random.Range(0, 11); // 0 to 10

        // Returns a number from 0 to 20, slightly weighted towards 10
        return (die1 + die2);
    }

    void handleInput()
    {
        if(isInGame())
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                toggleCameraControls();
            }

            if(Input.GetMouseButtonDown(2))
            {
                toggleCameraControls();
            }

            if(!cameraControls.enabled)
            {
                if(currentState == Constants.GameState.PlayerTurn)
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        placePlayerToken();
                    }

                    if(Input.GetMouseButtonDown(1))
                    {
                        rotatePlayerToken();
                    }
                }
            }
        }
    }

    void hideCharacterPanel()
    {
        characterPanel.SetActive(false);
    }

    void hideInGamePanel()
    {
        inGamePanel.SetActive(false);
    }

    void hideMainMenu()
    {
        mainMenuPanel.SetActive(false);
    }

    void hideMousePointer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void hideSettings()
    {
        settingsPanel.SetActive(false);
    }

    void hideViewCharacterPanel()
    {
        viewCharacterPanel.SetActive(false);
    }

    void hideWarningDialog()
    {
        warningDialog.SetActive(false);
    }

    void increaseStat(ref int stat, int max)
    {
        if(stat < max)
        {
            if(modifiedCharacter.unspentPoints > 0)
            {
                stat ++;
                modifiedCharacter.unspentPoints --;
            }
        }

        modifiedCharacter.calculateStats();
        updateCharacterPanel(Constants.Modify, ref modifiedCharacter);
    }

    public void increaseStat(string stat)
    {
        switch(stat.ToLower())
        {
            case "brawn":
                increaseStat(ref modifiedCharacter.brawn, modifiedCharacter.statMax);
                break;
            case "agility":
                increaseStat(ref modifiedCharacter.agility, modifiedCharacter.statMax);
                break;
            case "brains":
                increaseStat(ref modifiedCharacter.brains, modifiedCharacter.statMax);
                break;
            case "stamina":
                increaseStat(ref modifiedCharacter.stamina, modifiedCharacter.statMax);
                break;
        }
    }

    void initializeCamera()
    {
        cameraControls = (Camera.main.GetComponent<CameraControls>() as MonoBehaviour);
        disableCameraControls();
    }

    void initializeOpponents()
    {
        opponents = new List<NonPlayerCharacter>();

        for(int i = 0; i < round; i++)
        {
            opponents.Add(new NonPlayerCharacter(Constants.StatDefaultValue, Constants.StatMinValue, Constants.StatMaxValue, round, i + 1));
        }
    }

    void initializePlayer()
    {
        playerCharacter = new Character(Constants.StatDefaultValue, Constants.StatMinValue, Constants.StatMaxValue, Constants.StartingPoints, 0);
    }

    bool isInGame()
    {
        bool result;

        switch(currentState)
        {
            case Constants.GameState.MainMenu:
                result = false;
                break;
            case Constants.GameState.Settings:
                result = false;
                break;
            case Constants.GameState.WarningDialog:
                result = false;
                break;
            case Constants.GameState.CharacterCreation:
                result = false;
                break;
            case Constants.GameState.PreRound:
                result = true;
                break;
            case Constants.GameState.CharacterModification:
                result = false;
                break;
            case Constants.GameState.InCombat:
                result = true;
                break;
            case Constants.GameState.ViewCharacter:
                result = false;
                break;
            case Constants.GameState.PlayerTurn:
                result = true;
                break;
            case Constants.GameState.OpponentTurn:
                result = true;
                break;
            case Constants.GameState.PostRound:
                result = false;
                break;
            default:
                result = false;
                break;
        }

        return result;
    }

    void loadProgress()
    {
        if(PlayerPrefs.GetInt("resumable", 0) != 0)
        {
            playerCharacter.agility = PlayerPrefs.GetInt("agility", Constants.StatDefaultValue);
            playerCharacter.agilityMin = PlayerPrefs.GetInt("agilityMin", Constants.StatMinValue);
            playerCharacter.brains = PlayerPrefs.GetInt("brains", Constants.StatDefaultValue);
            playerCharacter.brainsMin = PlayerPrefs.GetInt("brainsMin", Constants.StatMinValue);
            playerCharacter.brawn = PlayerPrefs.GetInt("brawn", Constants.StatDefaultValue);
            playerCharacter.brawnMin = PlayerPrefs.GetInt("brawnMin", Constants.StatMinValue);
            playerCharacter.stamina = PlayerPrefs.GetInt("stamina", Constants.StatDefaultValue);
            playerCharacter.staminaMin = PlayerPrefs.GetInt("staminaMin", Constants.StatMinValue);
            playerCharacter.unspentPoints = PlayerPrefs.GetInt("unspentPoints", Constants.StartingPoints);
            playerCharacter.calculateStats();
            round = PlayerPrefs.GetInt("round", 1);
            resumable = true;
            resumeState = Constants.GameState.PreRound;
        }
    }

    void loadSettings()
    {
        bool needsSave = false;

        // Music Setting
        if(PlayerPrefs.HasKey("musicOn"))
        {
            if(PlayerPrefs.GetInt("musicOn") == 0)
            {
                musicToggle.isOn = false;
            }
            else
            {
                musicToggle.isOn = true;
            }
        }
        else
        {
            PlayerPrefs.SetInt("musicOn", 1);
            musicToggle.isOn = true;
            needsSave = true;
        }

        applyMusicSetting();

        // SFX Setting
        if(PlayerPrefs.HasKey("sfxOn"))
        {
            if(PlayerPrefs.GetInt("sfxOn") == 0)
            {
                sfxToggle.isOn = false;
            }
            else
            {
                sfxToggle.isOn = true;
            }
        }
        else
        {
            PlayerPrefs.SetInt("sfxOn", 1);
            sfxToggle.isOn = true;
            needsSave = true;
        }

        applySfxSetting();

        if(needsSave)
        {
            PlayerPrefs.Save();
        }
    }

    void lockMinStats()
    {
        modifiedCharacter.agilityMin = modifiedCharacter.agility;
        modifiedCharacter.brainsMin = modifiedCharacter.brains;
        modifiedCharacter.brawnMin = modifiedCharacter.brawn;
        modifiedCharacter.staminaMin = modifiedCharacter.stamina;
    }

    public void placePlayerToken()
    {
        RaycastHit raycastHit;

        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, 100.0f))
        {
            if(Vector3.Distance(raycastHit.point, opponentToken.transform.position) > Constants.TokenZone)
            {
                playerToken.transform.position = raycastHit.point;
            }
        }
    }

    public void resetGame()
    {
        initializePlayer();
        nextState = Constants.GameState.MainMenu;
        previousState = Constants.GameState.MainMenu;
        resumeState = Constants.GameState.MainMenu;
        resumable = false;
        currentTurn = -1;
        round = 1;
        saveProgress();
    }

    public void rotatePlayerToken()
    {
        RaycastHit raycastHit;

        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, 100.0f))
        {
            playerToken.transform.rotation = Quaternion.LookRotation((raycastHit.point - playerToken.transform.position).normalized);
        }
    }

    void saveProgress()
    {
        PlayerPrefs.SetInt("agility", playerCharacter.agility);
        PlayerPrefs.SetInt("agilityMin", playerCharacter.agilityMin);
        PlayerPrefs.SetInt("brains", playerCharacter.brains);
        PlayerPrefs.SetInt("brainsMin", playerCharacter.brainsMin);
        PlayerPrefs.SetInt("brawn", playerCharacter.brawn);
        PlayerPrefs.SetInt("brawnMin", playerCharacter.brawnMin);
        PlayerPrefs.SetInt("stamina", playerCharacter.stamina);
        PlayerPrefs.SetInt("staminaMin", playerCharacter.staminaMin);
        PlayerPrefs.SetInt("unspentPoints", playerCharacter.unspentPoints);
        PlayerPrefs.SetInt("round", round);

        if(resumable)
        {
            PlayerPrefs.SetInt("resumable", 1);
        }
        else
        {
            PlayerPrefs.SetInt("resumable", 0);
        }

        PlayerPrefs.Save();
    }
        
    void setGameState(Constants.GameState gameState)
    {
        currentState = gameState;
        applyGameState();
    }

    public void setGameState(string gameState)
    {
        if(gameState.ToLower().Equals("next"))
        {
            setGameState(nextState);
        }
        else if(gameState.ToLower().Equals("previous"))
        {
            setGameState(previousState);
        }
        else if(gameState.ToLower().Equals("resume"))
        {
            setGameState(resumeState);
        }
        else
        {
            foreach(Constants.GameState state in System.Enum.GetValues(typeof(Constants.GameState)))
            {
                if(state.ToString().Equals(gameState))
                {
                    setGameState(state);
                    break;
                }
            }
        }
    }

    void showCharacterPanel()
    {
        characterPanel.SetActive(true);
    }

    void showInGamePanel()
    {
        inGamePanel.SetActive(true);

        if(currentState == Constants.GameState.PreRound)
        {
            viewCharacterButton.gameObject.SetActive(false);
            characterButton.gameObject.SetActive(true);
            readyButton.gameObject.SetActive(true);
        }
        else if(currentState == Constants.GameState.InCombat)
        {
            readyButton.gameObject.SetActive(false);
            characterButton.gameObject.SetActive(false);
            viewCharacterButton.gameObject.SetActive(true);
        }
    }

    void showMainMenu()
    {
        if(resumable)
        {
            resumeButton.interactable = true;
        }
        else
        {
            resumeButton.interactable = false;
        }

        mainMenuPanel.SetActive(true);
    }

    void showMousePointer()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void showSettings()
    {
        settingsPanel.SetActive(true);
    }

    void showViewCharacterPanel()
    {
        viewCharacterPanel.SetActive(true);
    }

    void showWarningDialog()
    {
        if(resumable)
        {
            warningDialog.SetActive(true);
        }
        else
        {
            setGameState(Constants.GameState.CharacterCreation);
        }
    }

    int statCheck(int statValue)
    {
        // Success is defined as a "roll" that is less than or equal to the stat
        return (statValue - getDiceRoll());
    }

    void toggleCameraControls()
    {
        if(cameraControls.enabled)
        {
            disableCameraControls();
        }
        else
        {
            enableCameraControls();
        }
    }

    public void updateCharacter()
    {
        lockMinStats();
        playerCharacter = new Character(modifiedCharacter);
        modifiedCharacter = null;
    }

    void updateCharacterPanel(int panelType, ref Character character)
    {
        brawn[panelType].text = character.brawn.ToString();
        agility[panelType].text = character.agility.ToString();
        brains[panelType].text = character.brains.ToString();
        stamina[panelType].text = character.stamina.ToString();
        unspentPoints[panelType].text = character.unspentPoints.ToString();
        if(panelType == Constants.View)
        {
            actionPoints[panelType].text = string.Format("{0} / {1}", character.getActionPoints(), character.getAction());
        }
        else
        {
            actionPoints[panelType].text = character.getActionPoints().ToString();
        }
        if(panelType == Constants.View)
        {
            hitPoints[panelType].text = string.Format("{0} / {1}", (character.getHitPoints() - character.getWounds()), character.getHitPoints());
        }
        else
        {
            hitPoints[panelType].text = character.getHitPoints().ToString();
        }
        defense[panelType].text = character.getDefense().ToString();
        weapon[panelType].text = character.weapon.ToString();
        damage[panelType].text = string.Format("{0} - {1}", character.getDamageMin(), character.getDamageMax());
    }
}
