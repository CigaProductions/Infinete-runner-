using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class GameManager : MonoBehaviour {

    public static GameManager instance { set; get; }
    private PlayerController PlayerCtrl;

    private bool isGameStarted; 

    public Text scoreText, cointText, modifierText;
    private float score, coinScore, modifierScore; 

    private void Awake()
    {
        instance = this;
        //UpdateScores();
        PlayerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Swipe.instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            PlayerCtrl.StartRunning();
        }
		
	}

    public void UpdateScores()
    {
        scoreText.text = score.ToString();
        cointText.text = coinScore.ToString();
        modifierText.text = modifierScore.ToString();
    }
    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        UpdateScores();
    }
}
