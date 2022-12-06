using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hazard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

     private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
        //   collision.die;
        }
    }
}
