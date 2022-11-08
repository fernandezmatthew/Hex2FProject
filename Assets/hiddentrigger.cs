using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hiddentrigger : MonoBehaviour {

    [SerializeField] private GameObject hidden;
    void Start()
    {
        hidden.SetActive(true);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            hidden.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            hidden.SetActive(true);
        }
    }
}
