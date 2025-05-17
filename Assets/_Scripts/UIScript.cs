using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

public class UIScript : MonoBehaviour
{
    public GameObject winPAnel;
    public GameObject PausePanel;

    public GameObject gameOverPanel;

    int lostCount = 0;
    public static UIScript instance;



    private void Start()
    {
        if (!instance)
        {
            instance = this;
            // Advertisements.Instance.Initialize();
            lostCount = 0;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }


    }


    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }


    public void OnClaimButtonClicked()
    {

        Debug.Log("Claim button clicked!");
    }


    public void PauseBtn()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeBtn()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void HomeBtn()
    {
        PausePanel.SetActive(false);
        SceneManager.LoadScene("Game Menu");
        Time.timeScale = 1;
    }

    public void showWinPanel()
    {


        winPAnel.SetActive(true);
    }

    public void hideWinPanel()
    {
        winPAnel.SetActive(false);
    }
    public void NextLevelButton()
    {
        // Check if ads are allowed and if interstitial ads are available
        if (Configs.b_next == 1 && Configs.CheckTimeShowAds() && Gley.MobileAds.API.IsInterstitialAvailable())
        {
            // Pause the game and show the ad
            Time.timeScale = 0f;

            // Show the interstitial ad
            //Gley.MobileAds.Internal.MobileAdsTest.Instance.ShowInterstitial();

            // Callback or action after the ad is shown
            Time.timeScale = 1f;  // Resume game time
            Configs.SetStartTime();  // Reset start time
            hideWinPanel();  // Hide the win panel

            // Move to the next level
            PlayerPrefs.SetInt("Numlevel", PlayerPrefs.GetInt("Numlevel", 1) + 1);
            int n = PlayerPrefs.GetInt("Numlevel", 1) % SceneManager.sceneCountInBuildSettings;
            if (n == 0) n = 1;
            SceneManager.LoadScene(n);
        }
        else
        {
            // No ad or no ad requirement, just move to the next level
            hideWinPanel();
            PlayerPrefs.SetInt("Numlevel", PlayerPrefs.GetInt("Numlevel", 1) + 1);
            int n = PlayerPrefs.GetInt("Numlevel", 1) % SceneManager.sceneCountInBuildSettings;
            if (n == 0) n = 1;
            SceneManager.LoadScene(n);
        }
    }




    public void Restart()
    {
        // Check if ads are allowed for retry
        if (Configs.b_retry == 1 && Configs.CheckTimeShowAds() && Gley.MobileAds.API.IsInterstitialAvailable())
        {
            // Pause the game and show the ad
            Time.timeScale = 0f;

            // Show the interstitial ad (you may want to create a custom method with callback)
            //Gley.MobileAds.Internal.MobileAdsTest.Instance.ShowInterstitial();

            // Callback or action after the ad is shown
            Time.timeScale = 1f;  // Resume game time
            Configs.SetStartTime();  // Reset start time

            if (lostCount >= 3)
            {
                lostCount = 0;
                // Optionally show an ad if the player has lost 3 times
            }
            else
            {
                lostCount++;
            }

            // Hide Game Over Panel before restarting
            gameOverPanel.SetActive(false);
            ScoreManager.instance.ResetScore();

            // Restart the current level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            // No ad or no ad requirement, just restart the level
            if (lostCount >= 3)
            {
                lostCount = 0;
                // Optionally show an ad after 3 retries
            }
            else
            {
                lostCount++;
            }

            // Hide Game Over Panel before restarting
            gameOverPanel.SetActive(false);
            ScoreManager.instance.ResetScore();

            // Restart the current level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }



}
