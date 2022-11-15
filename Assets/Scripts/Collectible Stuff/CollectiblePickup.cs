using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblePickup : MonoBehaviour
{
    [Header("Collectible Manager: Collectible Options")]
    public bool isCollectible = false;
    public int collectibleValue = 1;
    private CollectibleManager cManager;

    //This component is placed on any object that is a keyItem pick up and to be placed in your "inventory"

    private void Start()
    {
        if (isCollectible)
        {
            cManager = FindObjectOfType<CollectibleManager>();
        }


    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            Destroy(gameObject);
            cManager.Collected(collectibleValue);
        }
    }
}