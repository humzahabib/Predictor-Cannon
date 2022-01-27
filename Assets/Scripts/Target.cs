using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField]
    public Vector3 velocity;


    public Cannon cannon;


    public Vector3 Velocity
    { get { return velocity; }  }

    // Start is called before the first frame update
    void Start()
    {
    }


    private void OnDestroy()
    {
        if (cannon != null)
            cannon.targets.Remove(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += velocity * Time.fixedDeltaTime;

    }


}
