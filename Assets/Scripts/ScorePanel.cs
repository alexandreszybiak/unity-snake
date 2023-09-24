using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D;
using UnityEngine;

public class ScorePanel : MonoBehaviour
{
    [SerializeField]
    GameObject counterOnes, counterTens;

    [SerializeField]
    List<Sprite> numberFont;

    private void Awake()
    {
        
    }
    void Start()
    {
        SetScore(19);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetScore(int value = 0)
    {
        counterTens.GetComponent<SpriteRenderer>().sprite = numberFont[value / 10];
        counterOnes.GetComponent<SpriteRenderer>().sprite = numberFont[value % 10];
    }
}
