using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using AYellowpaper;
//using AYellowpaper.Samples;


namespace DD.Web3
{
    public class ClaimButtonHandler : MonoBehaviour
    {
        public Button[] claimButtons;


        [SerializeField] private int claimAmount = 0;



        //[SerializeField] private InterfaceReference<IClaimAmountProvider, MonoBehaviour> claimAmountProvider;

        //[SerializeField] private GameSelection.Scene _currentScene;

        private void Start()
        {
            //Debug.Log("Claim Amount Provider : " + claimAmountProvider);

            foreach(var claimButton in claimButtons)
            {
                claimButton.onClick.AddListener(OnClaimButtonClicked);
            }
        }

        private void OnClaimButtonClicked()
        {
            //claimAmount = claimAmountProvider.Value.GetClaimAmount();
            claimAmount = ScoreManager.instance.currentScore;
            Debug.Log("ScoreManager.instance.currentScore = " + ScoreManager.instance.currentScore);

            Debug.Log("Claim Amount = " + claimAmount);
            //claimAmount = 1;

            if(BlockchainManager.Instance != null)
            {
                HandleClaimFlow();
            }
            else
            {
                Debug.Log("Blockchain or Wallet is not being used !");
            }
        }

        private void HandleClaimFlow()
        {
            
            BlockchainManager.Instance.connectionManager.ClaimDropERC20(claimAmount, (result) =>
            {
                if (result)
                {
                    OnTransactionSuccessful();
                }
                else
                {
                    OnTransactionFailed();
                }
            });
        }

        private void OnTransactionSuccessful()
        {
            Debug.Log("Transaction successful.");
            // You can open a success popup or trigger other success logic
            BlockchainManager.Instance.connectionManager.ShowLoadingScreen(false);
            BlockchainManager.Instance.claimPossibilityUI.HidePossibilityUI();
        }

        private void OnTransactionFailed()
        {
            Debug.Log("Transaction failed.");
            // You can open a retry popup or show an error message
            BlockchainManager.Instance.connectionManager.ShowLoadingScreen(false);

        }

        private void OnDestroy()
        {
            foreach (var claimButton in claimButtons)
            {
                claimButton.onClick.RemoveAllListeners();
            }
        }
    }
}

