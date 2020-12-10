using UnityEngine;

public class movement : MonoBehaviour
{
    public float force = 3000f;
    Rigidbody rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey("a"))
        {
            rb.AddForce(0, 0, -force * Time.deltaTime);
        }


        if (Input.GetKey("d"))
        {
            rb.AddForce(0, 0, force * Time.deltaTime);
        }
    }
}