using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arvore : MonoBehaviour
{
    DiaNoite d = new DiaNoite();
    private void Start() {
        Debug.Log(d.getData().dia);
    }

    private void Update() {
        Debug.Log(d.getHora().segundo);
    }
}