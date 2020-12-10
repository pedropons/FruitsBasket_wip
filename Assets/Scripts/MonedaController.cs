using System;
using UnityEngine;

public class MonedaController : MonoBehaviour
{
    private AudioSource audiosource;

    public AudioClip coin_gain;
    public AudioClip coin_lost;
    private float rot_speed;

    private EventManager _eventManager;

    void Start()
    {
        audiosource = FindObjectOfType<FrutaManager>().GetComponent<AudioSource>();
        _eventManager = EventManager.GetInstance();

        rot_speed = 90F;
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
                audiosource.clip = coin_gain;
                audiosource.Play();
            }

            _eventManager.Notify(EventManager.EventTypes.IncreaseCoins);
            Destroy(this.gameObject);
        }

        if (collision.collider.name == "Plataforma")
        {
            if (audiosource.enabled)
            {
                audiosource.clip = coin_lost;
                audiosource.Play();
            }
            _eventManager.Notify(EventManager.EventTypes.LoseCoin);
            Destroy(this.gameObject);
        }
    }
}