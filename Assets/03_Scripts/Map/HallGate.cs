using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallGate : MonoBehaviour
{
    [HideInInspector]
    public bool isOpen;

    private Animator ani;

    private void Awake()
    {
        ani = GetComponent<Animator>();
    }

    public void OpenGate()
    {
        isOpen = true;
        ani.SetBool("isOpen", true);
    }

    public void CloseGate()
    {
        isOpen = false;
        ani.SetBool("isOpen", false);
    }
}
