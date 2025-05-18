using TMPro;
using UnityEngine;

namespace DD.Web3
{
    public class ClaimPossibilityUI : MonoBehaviour
    {

        //[SerializeField] private GameObject possibilitryCanvas;


        [Header("Not Possible UI")]
        [SerializeField] private TextMeshProUGUI claimDetailsTxt;
        [SerializeField] private GameObject notPossiblePanel;


        [Header("Possible UI")]
        [SerializeField] private GameObject possiblePanel;
        [SerializeField] private TextMeshProUGUI claimPointsTxt;



        public void ShowNotPossibleDetails(string _detailedTxt)
        {
            claimDetailsTxt.text = _detailedTxt;
            notPossiblePanel.SetActive(true);
            possiblePanel.SetActive(false);
            gameObject.SetActive(true);
        }

        public void ShowPossibleUI()
        {
            claimPointsTxt.text = "Points: "+ScoreManager.instance.currentScore.ToString();
            notPossiblePanel.SetActive(false);
            possiblePanel.SetActive(true);
            gameObject.SetActive(true);
        }


        public void HidePossibilityUI()
        {
            gameObject.SetActive(false);
        }
    }
}

