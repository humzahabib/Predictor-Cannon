using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    

    [SerializeField]
    Target target;

    Cannon cannon;

    // Start is called before the first frame update
    void Start()
    {
        cannon = FindObjectOfType<Cannon>();   
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Target _target = Instantiate(target, this.transform.position, Quaternion.identity);
            _target.cannon = cannon;

            _target.velocity = new Vector3(0, 0, -Random.Range(7, 15));
            cannon.targets.Add(_target);
        }    
    }

}
