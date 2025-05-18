using DD.Web3;
using UnityEngine;


namespace DD.Web3
{
    public class ClaimPossibilityUIHandler : MonoBehaviour
    {
        private async void OnEnable()
        {
            var possibilityData = await ClaimERC20.IsClaimPossible(ScoreManager.instance.currentScore);

            if (!possibilityData.isPossible)
            {
                string detailed = "Available Balance: " + possibilityData.nativeBalance +
                    "\n Required Balance: " + possibilityData.requiredAmount;

                Debug.Log("BlockchainManager.Instance :   " + BlockchainManager.Instance);
                Debug.Log("BlockchainManager.Instance.claimPossibilityUI :   " + BlockchainManager.Instance.claimPossibilityUI);

                BlockchainManager.Instance.claimPossibilityUI.ShowNotPossibleDetails(detailed);
            }
            else
            {
                BlockchainManager.Instance.claimPossibilityUI.ShowPossibleUI();

            }
        }
    }
}

