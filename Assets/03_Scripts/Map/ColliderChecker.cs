using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChecker : MonoBehaviour
{
    private BoxCollider2D col;
    public bool isGround = false;
    [HideInInspector]
    public Collider2D groundCol;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Comparer.Equals(collision, groundCol)) isGround = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Comparer.Equals(collision, groundCol)) isGround = false;
    }
}
