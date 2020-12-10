using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    
    private static Vector3 livesPosition;
    
    private static Text scoreTag;
    private static Text livesTag;
    private static Text coinsTag;
    private static Text chainTag;
    
    private static GameObject gameOverCanvas ;
    private static GameObject startMenuCanvas;
    private static GameObject tutoCanvas;
    private static GameObject countersCanvas;
    private static Text startCoins;
    private static Text startHighscore;
    private static Text scoreFinal;
    private static Text coinsFinal;
    private static Text coinsPlus;
    private static Text newhighscore;
    private static Image flecha;
    private EventManager _eventManager;
    
    private GameManager gameManager;

    private AudioSource audiosource;

    public AudioClip coingain;
    //private static MainUIManager instance = null;   
    
    // Start is called before the first frame update
    
    public void Start()
    {
        gameManager = GameManager.GetInstance();
        audiosource = gameObject.AddComponent<AudioSource>();
        InstantiateResources();
        gameOverCanvas.SetActive(false);
        countersCanvas.SetActive(false);
        startMenuCanvas.SetActive(true);
        tutoCanvas.SetActive(false);
    }
    
    private void InstantiateResources()
    {
        Debug.Log("instantiate Menus");
        /*gameOverCanvas = Instantiate(Resources.Load("GameOverCanvas") as GameObject);
        countersCanvas = Instantiate(Resources.Load("Counters") as GameObject);
        startMenuCanvas = Instantiate(Resources.Load("StartMenu") as GameObject);
        tutoCanvas = Instantiate(Resources.Load("tutocanvas") as GameObject);*/
        gameOverCanvas = InstantiateFromPrefab("GameOverCanvas");
        countersCanvas = InstantiateFromPrefab("Counters");
        startMenuCanvas = InstantiateFromPrefab("StartMenu");
        tutoCanvas = InstantiateFromPrefab("tutocanvas");

        Button tutoClose = tutoCanvas.transform.Find("tutoClose").GetComponent<Button>();
        tutoClose.onClick.AddListener(gameManager.CloseTutorial);
        
        startCoins = startMenuCanvas.transform.Find("totalcoinsTag").GetComponent<Text>();
        startHighscore = startMenuCanvas.transform.Find("highscoreTag").GetComponent<Text>();
        flecha = startMenuCanvas.transform.Find("flecha").GetComponent<Image>();
        Button startLevel = startMenuCanvas.transform.Find("Start").GetComponent<Button>();
        startLevel.onClick.AddListener(gameManager.RestartLevel);
        Button tutoShow = startMenuCanvas.transform.Find("tutoShow").GetComponent<Button>();
        tutoShow.onClick.AddListener(gameManager.ShowTutorial);
        Button closeGame = startMenuCanvas.transform.Find("closeApp").GetComponent<Button>();
        closeGame.onClick.AddListener(gameManager.Quit);
        Button gotoshop = startMenuCanvas.transform.Find("Gotoshop").GetComponent<Button>();
        gotoshop.onClick.AddListener(gameManager.GoToShop);
        
        scoreFinal = gameOverCanvas.transform.Find("scoreFinal").GetComponent<Text>();
        coinsFinal = gameOverCanvas.transform.Find("levelcoins_go").GetComponent<Text>();
        coinsPlus = gameOverCanvas.transform.Find("levelcoins_plus").GetComponent<Text>();
        newhighscore = gameOverCanvas.transform.Find("newhighscore").GetComponent<Text>();
        Button restart = gameOverCanvas.transform.Find("Restart").GetComponent<Button>();
        restart.onClick.AddListener(gameManager.RestartLevel);
        Button quit = gameOverCanvas.transform.Find("Quit").GetComponent<Button>();
        quit.onClick.AddListener(gameManager.Quit);
        Button goToMenu_go = gameOverCanvas.transform.Find("GoToMenuGo").GetComponent<Button>();
        goToMenu_go.onClick.AddListener(gameManager.GoToMenu);

        livesTag = countersCanvas.transform.Find("lives").GetComponent<Text>();
        coinsTag = countersCanvas.transform.Find("levelcoins").GetComponent<Text>();
        scoreTag = countersCanvas.transform.Find("score").GetComponent<Text>();
        chainTag = countersCanvas.transform.Find("ChainTag").GetComponent<Text>();

    }
    
    private static GameObject InstantiateFromPrefab(string name)
    {
        return Instantiate(Resources.Load(name) as GameObject);
    }

    public void UpdateLives(int lives)
    {
        livesTag.text = lives.ToString();
    }
    
    public void UpdateScore(int scorePoints)
    {
        scoreTag.text = scorePoints.ToString();
    }
    
    public void UpdateCoins(int coins)
    {
        coinsTag.text = coins.ToString();
    }
    
    public void UpdateChain(float chain)
    {
        if (chain > 1f)
        {
            chainTag.enabled = true;
        }
        else
        {
            chainTag.enabled = false;
        }
        
        chainTag.text = "CHAIN  x"+chain.ToString("F1").Replace(",",".");
    }
    
    public void GoCounters()
    {
        gameOverCanvas.SetActive(false);
        startMenuCanvas.SetActive(false);
        countersCanvas.SetActive(true);
        
        livesPosition = livesTag.transform.position;
        chainTag.enabled = false;
    }
    
    public (int,int,int) GameOverCoiner(int coins, int coinwait, int coindelay, int totalcoins_display)
    {
        ArrayList counts = new ArrayList();
        if (coinsPlus.isActiveAndEnabled && coins > 0)
        {
            if (coinwait > 0)
            {
                coinwait -= 1;
            }
            else
            {
                coinwait = coindelay;
                totalcoins_display += 1;
                coins -= 1;
                coinsFinal.text = totalcoins_display.ToString();
                coinsPlus.text = string.Concat("+",coins.ToString());
                
                if (audiosource.enabled)
                {
                    audiosource.clip = coingain;
                    audiosource.Play();
                }
                
            }
        }
        else
        {
            coinsPlus.text = "";
        }

        return (coinwait, totalcoins_display, coins);
    }

    public void GameOverMenu(int scorePoints, int highscore, int totalcoins, int coins)
    {
        if (scorePoints > highscore)
        {
            GameManager.SetHighscore(scorePoints);
            newhighscore.enabled = true;
        }
        else
        {
            newhighscore.enabled = false;
        }
        
        scoreFinal.text = scorePoints.ToString();
        coinsFinal.text = totalcoins.ToString();
        coinsPlus.text = coins.ToString();

        gameOverCanvas.SetActive(true);
        countersCanvas.SetActive(false);
    }
    
    public void ShowTutorial()
    {
        GameManager.alreadySeen = true;
        flecha.enabled = false;
        tutoCanvas.SetActive(true);
    }
    
    public void CloseTutorial()
    {
        tutoCanvas.SetActive(false);
    }
    
    public void GoToMenu(int totalcoins, int highscore)
    {
        gameOverCanvas.SetActive(false);
        countersCanvas.SetActive(false);
        startMenuCanvas.SetActive(true);
        tutoCanvas.SetActive(false);

        if (GameManager.alreadySeen == true)
        {
            flecha.enabled = false;
        }

        startCoins.text = totalcoins.ToString();
        startHighscore.text = highscore.ToString();
    }
    
    public void VibrateLives(float intensity)
    {
        float posX = Random.Range(-intensity, intensity);
        float posY = Random.Range(-intensity, intensity);
        Vector3 vibration = new Vector3(posX, posY, 0);
        livesTag.transform.position = livesPosition + vibration;
    }
    
}
