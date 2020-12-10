using UnityEngine;

public class FrutaController : MonoBehaviour
{
    private AudioSource audiosource;

    public AudioClip collect;
    public AudioClip fail;

    private EventManager _eventManager;
    private float rot_speed;

    void Start()
    {
        audiosource = FindObjectOfType<FrutaManager>().GetComponent<AudioSource>();
        _eventManager = EventManager.GetInstance();
        rot_speed = 75F;
    }

    private void Update()
    {
        transform.Rotate(0f,rot_speed*Time.deltaTime,0f);
    }
    

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Cesta"))
        {
            if (audiosource.enabled)
            {
                audiosource.clip = collect;
                audiosource.Play();
            }

            _eventManager.Notify(EventManager.EventTypes.IncreaseScore);
            Destroy(this.gameObject);
        }

        if (collision.collider.name == "Plataforma")
        {
            if (audiosource.enabled)
            {
                audiosource.clip = fail;
                audiosource.Play();
            }

            _eventManager.Notify(EventManager.EventTypes.LoseLife);
            Destroy(this.gameObject);
        }
    }
}