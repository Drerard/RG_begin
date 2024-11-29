using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInterface : MonoBehaviour
{
    [SerializeField] private TMP_Text HPText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text keyText;
    [Space (5)]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private float delayActivateDeathScreen = 1;


    private void Awake()
    {
        UIEventManager.OnHPChange.AddListener(ChangeHPText);
        UIEventManager.OnCoinChange.AddListener(ChangeCoinText);
        UIEventManager.OnKeyChange.AddListener(ChangeKeyText);

        UIEventManager.OnDeath.AddListener(StartDeathScreen);
    }

    private void ChangeHPText(float currentHP, float maxHP)
    {
        HPText.text = string.Format("{0}/{1}", Math.Round(currentHP), Math.Round(maxHP));
    }

    private void ChangeCoinText(float count)
    {
        coinText.text = count.ToString();
    }

    private void ChangeKeyText(float count)
    {
        keyText.text = count.ToString();
    }


    private void StartDeathScreen()
    {
        StartCoroutine(DelayStartDeathScreen());
    }
    IEnumerator DelayStartDeathScreen()
    {
        yield return new WaitForSeconds(delayActivateDeathScreen);
        deathScreen.SetActive(true);
    }

    public void ReStartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
