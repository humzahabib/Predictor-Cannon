using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShell : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.forward = rb.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Target")
        {
            GameObject.Instantiate(explosion, transform.position, transform.rotation);

            Destroy(collision.gameObject);
        }
        Destroy(this.gameObject);
      
    }


}
