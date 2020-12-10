using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class ShopManager : MonoBehaviour
{
    private GameObject shopCanvas;
    private Text priceTag;
    private Text currencyTag;
    private Image coin;
    private Button buy;
    private Button select;
    private Button selected;

    private Dictionary<string, ShopItem> shopDict;

    public List<string> basketnames;

    private string currentBasketName;
    private GameObject currentBasket;
    private int currentBasketIndex;
    private int basketListLength;
    private int currentBasketPrice;
    private float rot_speed;

    private AudioSource audiosource;
    public AudioClip coin_gain;
    public AudioClip cuic;

    private GameManager gameManager;
    
    // Start is called before the first frame update
    public void Start()
    {
        Debug.Log("Start Shop");
        gameManager = GameManager.GetInstance();
        rot_speed = 65F;
        shopCanvas = InstantiateFromPrefab("ShopCanvas");
        buy = shopCanvas.transform.Find("Buy").GetComponent<Button>();
        buy.onClick.AddListener(BuyBasket);
        select = shopCanvas.transform.Find("Select").GetComponent<Button>();
        select.onClick.AddListener(SelectBasket);
        selected = shopCanvas.transform.Find("Selected").GetComponent<Button>();
        Button backToMenu = shopCanvas.transform.Find("Back").GetComponent<Button>();
        backToMenu.onClick.AddListener(GoToMain);
        priceTag = shopCanvas.transform.Find("priceTag").GetComponent<Text>();
        currencyTag = shopCanvas.transform.Find("Currency").GetComponent<Text>();
        coin = shopCanvas.transform.Find("Coin").GetComponent<Image>();

        Button left = shopCanvas.transform.Find("leftBtn").GetComponent<Button>();
        left.onClick.AddListener(GoLeft);
        Button right = shopCanvas.transform.Find("rightBtn").GetComponent<Button>();
        right.onClick.AddListener(GoRight);
        
        shopDict = GameManager.shopDict;

        basketnames = shopDict.Keys.ToList();
        
        currentBasketIndex = 0;
        basketListLength = basketnames.Count;
        
        audiosource = this.GetComponent<AudioSource>();

        ReloadShop(currentBasketIndex);


    }

    private GameObject InstantiateFromPrefab(string name)
    {
        return Instantiate(Resources.Load(name) as GameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate
        currentBasket.transform.Rotate(0f,rot_speed*Time.deltaTime,0f);
    }
    
    private void GoToMain()
    {
        Debug.Log("Going Back");
        GameManager.gameMode = "restart";
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    
    private void GoLeft()
    {
        if (currentBasketIndex<=0)
        {
            currentBasketIndex = basketListLength-1;
        }
        else
        {
            currentBasketIndex -= 1;
        }
        
        ReloadShop(currentBasketIndex);
    }
    
    private void GoRight()
    {
        if (currentBasketIndex>=basketListLength-1)
        {
            currentBasketIndex = 0;
        }
        else
        {
            currentBasketIndex += 1;
        }
        
        ReloadShop(currentBasketIndex);
    }
    
    private void BuyBasket()
    {
        if (gameManager.totalcoins >= currentBasketPrice)
        {
            gameManager.unlocked.Add(currentBasketName);
            gameManager.totalcoins -= currentBasketPrice;

            gameManager.SaveProgress();
            
            if (audiosource.enabled)
            {
                audiosource.clip = coin_gain;
                audiosource.Play();
            }
            ReloadShop(currentBasketIndex);
        }
    }
    
    private void SelectBasket()
    {
        gameManager.selectedBasket = currentBasketName;
        gameManager.SaveProgress();
        
        if (audiosource.enabled)
        {
            audiosource.clip = cuic;
            audiosource.Play();
        }
        
        ReloadShop(currentBasketIndex);
    }

    private void ReloadShop(int currentBasketIndex)
    {
        //Debug.Log("selected:"+gameManager.selectedBasket);
        currentBasketName = basketnames[currentBasketIndex];
        string currentBasketModel = shopDict[currentBasketName].model;
        currentBasketPrice = shopDict[currentBasketName].price;
        
        GameObject[] baskets = GameObject.FindGameObjectsWithTag("Cestas");
        foreach (GameObject cesta in baskets)
        {
            GameObject.Destroy(cesta);
        }

        currencyTag.text = gameManager.totalcoins.ToString();
        
        currentBasket = InstantiateFromPrefab(currentBasketModel);

        if (gameManager.unlocked.Contains(currentBasketName))
        {
            priceTag.enabled = false;
            coin.enabled = false;
            
            if (gameManager.selectedBasket == currentBasketName)
            {
                selected.gameObject.SetActive(true);
                buy.gameObject.SetActive(false);
                select.gameObject.SetActive(false);
            }
            else
            {
                selected.gameObject.SetActive(false);
                buy.gameObject.SetActive(false);
                select.gameObject.SetActive(true);
            }
        }
        else
        {
            priceTag.text = currentBasketPrice.ToString();
            priceTag.enabled = true;
            coin.enabled = true;
            buy.gameObject.SetActive(true);
            select.gameObject.SetActive(false);
            selected.gameObject.SetActive(false);
        }
    }

}

[System.Serializable]
public class ShopItem
{
    public string name;
    public string model;
    public int price;
}

[System.Serializable]
public class ShopListObjects
{
    public List<ShopItem> shoplist;
}

