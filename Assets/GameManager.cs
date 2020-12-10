using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using Button = UnityEngine.UIElements.Button;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour, IEventListener
{
    public string data_path;
    public static int scorePoints;
    public static int highscore;
    public static int lives;
    public static int coins;
    public static float chain;
    public int totalcoins;
    public int totalcoins_display;
    public static int coindelay;
    public static int coinwait;
    private GameObject basket;
    private GameObject basket_ori;
    private static GameObject frutaManager;
    private GameObject mainUIManager;
    private MeshFilter basketMesh;
    private EventManager _eventManager;
    public GameData gameData;
    public List<string> unlocked;
    public string selectedBasket;
    public static bool gameHasEnded;
    public static bool alreadySeen = false;
    public static string gameMode;
    public static List<ShopItem> shopitems;
    public static Dictionary<string, ShopItem> shopDict;
    public static MainUIManager MainUI;
    private static GameManager instance = null;  //Static instance of GameManager which allows it to be accessed by any other script.

    //Awake is always called before any Start functions

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
            _eventManager = EventManager.GetInstance();
            _eventManager.Subscribe(EventManager.EventTypes.LoseLife, this);
            _eventManager.Subscribe(EventManager.EventTypes.IncreaseScore, this);
            _eventManager.Subscribe(EventManager.EventTypes.IncreaseCoins, this);
            _eventManager.Subscribe(EventManager.EventTypes.LoseCoin, this);
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Tgen destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of GM
            Destroy(gameObject);
        }
        //set this to not be destroyed when loading scene
        DontDestroyOnLoad(gameObject);

        if (gameMode == "restart")
        {
            Start();
        }
    }

    private void InstantiateResources()
    {
        basket_ori = InstantiateFromPrefab("Cesta");
        basket = SpawnBasket(selectedBasket);
        Destroy(basket_ori);

        frutaManager = InstantiateFromPrefab("frutaManager");
        mainUIManager = InstantiateFromPrefab("MainUIManager");
        MainUI = mainUIManager.GetComponent<MainUIManager>();
    }

    private GameObject SpawnBasket(string basketName)
    {
        string basketModel = shopDict[basketName].model;
        basket = InstantiateFromPrefab(basketModel);

        //Vector3 offset = new Vector3(0, (float) 0.2, 0);
        basket.transform.position = basket_ori.transform.position; //+ offset;
        basket.AddComponent<movement>();
        Rigidbody basketRB = basket.AddComponent<Rigidbody>();
        BoxCollider basketBC = basket.AddComponent<BoxCollider>();

        basketRB.constraints = basket_ori.GetComponent<Rigidbody>().constraints;
        basketRB.mass = basket_ori.GetComponent<Rigidbody>().mass;
        basketRB.drag = basket_ori.GetComponent<Rigidbody>().drag;
        basketRB.angularDrag = basket_ori.GetComponent<Rigidbody>().angularDrag;
        
        basketBC.center = basket_ori.GetComponent<BoxCollider>().center;
        basketBC.size = basket_ori.GetComponent<BoxCollider>().size;

        basket.tag = basket_ori.tag;


        return basket;
    }

    private GameObject InstantiateFromPrefab(string name) //TODO: add a utils class with these kind of functions
    {
        return Instantiate(Resources.Load(name) as GameObject);
    }
    
    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = new GameManager();
        }

        return instance;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        data_path = Application.dataPath;
        LoadItemData();
        InitializeValues();
        InstantiateResources();
        
        gameMode = "starting";
        //TransformInto(selectedBasket);
    }

    private void LoadItemData()
    {
        TextAsset json = Resources.Load<TextAsset>("shopconfig");
        shopitems = JsonUtility.FromJson<ShopListObjects>(json.text).shoplist;
        
        shopDict = new Dictionary<string, ShopItem>();
        foreach (ShopItem item in shopitems)
        {
            shopDict.Add(item.name,item);
        }
    }
    
    private void InitializeValues()
    {
        if (System.IO.File.Exists(DataManager.SavePath))
        {
            Debug.Log("Loading from"+DataManager.SavePath);
            gameData = DataManager.LoadData();

            totalcoins = gameData.coins;
            highscore = gameData.highscore;
            unlocked = gameData.baskets;
            selectedBasket = gameData.selected;

        }
        else
        {
            GameData gameData = new GameData();
            totalcoins = 0;
            highscore = 0;
            unlocked = new List<string>();
            unlocked.Add("default");
            selectedBasket = "default";
            SaveProgress();
        }
    }

    public void StartLevel()
    {
        MainUI.GoCounters();

        chain = 1.0f;
        coinwait = 25;
        coindelay = 6;
        coins = 0;
        scorePoints = 0;
        lives = 3;

        MainUI.UpdateLives(lives);
        MainUI.UpdateScore(scorePoints);
        MainUI.UpdateCoins(coins);

        gameMode = "playing";
    }

    void Update()
    {
        if (gameMode == "playing")
        {
            if (lives < 2)
            {
                VibrateLives(5f);
            } else if(lives < 3)
            {
                VibrateLives(2f);
            }

            if (lives < 1 && gameHasEnded == false)
            {
                gameHasEnded = true;
                GameOver();
            }
            
        } else if (gameMode == "gameover")
        {
            (coinwait, totalcoins_display, coins) = MainUI.GameOverCoiner(coins, coinwait, coindelay, totalcoins_display);
        } else if (gameMode == "starting")
        {
            GoToMenu();
            gameMode = "atmenu";
        }
    }
    
    public void SaveProgress()
    {
        gameData.coins = totalcoins;
        gameData.highscore = highscore;
        gameData.baskets = unlocked;
        gameData.selected = selectedBasket;

        DataManager.SaveData(gameData);
    }

    void GameOver()
    {
      
        foreach (Transform child in frutaManager.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        MainUI.GameOverMenu(scorePoints, highscore, totalcoins, coins);

        totalcoins_display = totalcoins;
        totalcoins += coins;
        
        gameData.coins = totalcoins;
        gameData.highscore = highscore;

        DataManager.SaveData(gameData);

        gameMode = "gameover";
    }

    public static void SetHighscore(int new_highscore)
    {
        highscore = new_highscore;
    }

    public void ShowTutorial()
    {
        MainUI.ShowTutorial();
    }
    
    public void CloseTutorial()
    {
        MainUI.CloseTutorial();
    }

    public void GoToMenu()
    {
        MainUI.GoToMenu(totalcoins, highscore);
        gameMode = "startmenu";
    }


    public void RestartLevel()
    {
        gameHasEnded = false;

        foreach (Transform child in frutaManager.transform)
        {
            Destroy(child.gameObject);
        }
        //Destroy(frutaManager);

        FrutaManager.Restart();
        StartLevel();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OnEvent(EventManager.EventTypes evento)
    {
        if (!gameHasEnded)
        {
            if (evento == EventManager.EventTypes.LoseLife)
            {
                lives--;
                chain = 1;
                MainUI.UpdateLives(lives);
                MainUI.UpdateChain(chain);
            } else if (evento == EventManager.EventTypes.LoseCoin)
            {
                chain = 1;
                MainUI.UpdateChain(chain);
            }
            else if (evento == EventManager.EventTypes.IncreaseScore)
            {
                scorePoints += Mathf.RoundToInt(100*chain);
                chain += 0.1f;
                MainUI.UpdateScore(scorePoints);
                MainUI.UpdateChain(chain);
            }
            else if (evento == EventManager.EventTypes.IncreaseCoins)
            {
                coins += Mathf.FloorToInt(1*chain);
                MainUI.UpdateCoins(coins);
            }
        }
    }

    /*public void TransformInto(string basketName)
    {
        string basketModel = shopDict[basketName].model;
        
        basketMesh = basket.GetComponent<MeshFilter>();

        GameObject skin = Resources.Load(basketModel) as GameObject;
        Mesh mesh = skin.GetComponent<MeshFilter>().sharedMesh;
        basketMesh.sharedMesh = mesh;
    }*/

    private void VibrateLives(float intensity)
    {
        MainUI.VibrateLives(intensity);
    }

    public void GoToShop()
    {
        gameMode = "shop";

        SceneManager.LoadScene("Shop", LoadSceneMode.Single);
    }
}
