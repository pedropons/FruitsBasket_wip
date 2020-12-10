using UnityEngine;

public class FrutaManager : MonoBehaviour
{
    private float posZ;
    private float posX = 0f;
    private float posY = 12f;
    //public GameObject fruta;
    //public GameObject moneda;
    public GameObject spawned;
    private static float spawnTime;
    private static float maxSpawnTime = 3f;
    private float minSpawnTime = 1f;
    private float spawn_progression = 0.025f;
    private float current_drag = 4f;
    private float drag_progression = 0.02f;
    private float timer;
    
    
    // Start is called before the first frame update
    void Start()
    {
        spawnTime = maxSpawnTime;
        timer = spawnTime;
    }

    public static void Restart()
    {
        spawnTime = maxSpawnTime;
    }

    // Update is called once per frame. Fixed for better Physics rendering.
    void Update()
    {
        if (GameManager.gameMode == "playing")
        {
            timer -= Time.deltaTime;
            //current_drag -= drag_progression * Time.deltaTime; // infinite difficulty

            if (spawnTime > minSpawnTime)
            {
                spawnTime -= spawn_progression * Time.deltaTime; //TODO: adjust with difficulty
                current_drag -= drag_progression * Time.deltaTime;
            }

            if (timer <= 0 & GameManager.gameHasEnded == false)
            {
                Spawn();
                //current_drag -= drag_progression;
                timer = spawnTime;
            }
        }
    }

    void Spawn()
    {
        posZ = Random.Range(-12, 12);
        //Decide if a fruit or a coin spawns
        if (Random.Range(0,100) > 25)
        {
            spawned = InstantiateFromPrefab("Fruta");
            spawned.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
        }
        else
        {
            spawned = InstantiateFromPrefab("Moneda");
        }
        Debug.Log(current_drag);
        Debug.Log(spawnTime);
        spawned.transform.parent = transform;
        spawned.GetComponent<Rigidbody>().drag = current_drag;
    }
    
    private GameObject InstantiateFromPrefab(string name)
    {
        return Instantiate(Resources.Load(name) as GameObject, new Vector3(posX, posY, posZ), Quaternion.identity);
    }
}