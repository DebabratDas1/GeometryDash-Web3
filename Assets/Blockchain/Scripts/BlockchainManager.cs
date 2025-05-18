using System;
using System.Collections.Generic;
using Thirdweb;
using UnityEngine;


namespace DD.Web3
{
    public class BlockchainManager : MonoBehaviour
    {
        public static BlockchainManager Instance { get; private set; }


        [Header("Other Scripts")]
        [SerializeField] public ConnectionManager connectionManager;
        [SerializeField] public WalletConnected walletConnected;
        [SerializeField] public ClaimPossibilityUI claimPossibilityUI;
        [Header("Network Settings")]
        [SerializeField] private BuildNetworkSO buildNetworkSO;
        [SerializeField] private List<NetworkConfigSO> networkConfigs;

        [HideInInspector]public NetworkConfigSO currentConfig;

        //Wallet
        public IThirdwebWallet wallet;
        public string walletAddress;

        // DropErc20 For Claiming
        public ThirdwebContract dropErc20Contract = null;
        public string dropErc20Balance;


        // Other Tokens

        public ThirdwebContract dropErc20LifeContract = null;

        public ThirdwebContract dropErc721Contract = null;



        //Actions
        public Action<string> onDropErc20BalanceUpdated;

        private void Awake()
        {

            if(Instance!=null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;



            currentConfig = networkConfigs.Find(config => config.network == buildNetworkSO.selectedNetwork);
            if (currentConfig == null)
            {
                Debug.LogError("No matching network config found!");
            }
            else
            {
                Debug.Log("Current Selected Network is " + currentConfig.name);
            }
        }
    }
}

