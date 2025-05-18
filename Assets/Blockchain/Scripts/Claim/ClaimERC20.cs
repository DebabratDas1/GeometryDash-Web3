using System.Threading.Tasks;
using System;
using Thirdweb;
using UnityEngine;
using System.Numerics;
using DD.Web3;
using Thirdweb.Unity;

public static class ClaimERC20
{
    /*public static async Task<ThirdwebTransactionReceipt> DropERC20_Claim_Custom(this ThirdwebContract contract, IThirdwebWallet wallet, string receiverAddress, string amount)
    {
        if (contract == null)
        {
            throw new ArgumentNullException("contract");
        }

        if (wallet == null)
        {
            throw new ArgumentNullException("wallet");
        }

        if (string.IsNullOrEmpty(receiverAddress))
        {
            throw new ArgumentException("Receiver address must be provided");
        }

        if (string.IsNullOrEmpty(amount))
        {
            throw new ArgumentException("Amount must be provided");
        }

        Drop_ClaimCondition activeClaimCondition = await contract.DropERC20_GetActiveClaimCondition();
        int toDecimals = await contract.ERC20_Decimals();
        BigInteger amountInBigInt = BigInteger.Parse(amount);
        BigInteger bigInteger = BigInteger.Parse(amount.ToWei()).AdjustDecimals(18, toDecimals);
        BigInteger weiValue = ((activeClaimCondition.Currency == "0xEeeeeEeeeEeEeeEeEeEeeEEEeeeeEeeeeeeeEEeE") ? (amountInBigInt * activeClaimCondition.PricePerToken) : BigInteger.Zero);
        object[] array = new object[4]
        {
            Array.Empty<byte>(),
            BigInteger.Zero,
            BigInteger.Zero,
            "0x0000000000000000000000000000000000000000"
        };
        object[] parameters = new object[6]
        {
            receiverAddress,
            bigInteger,
            activeClaimCondition.Currency,
            activeClaimCondition.PricePerToken,
            array,
            Array.Empty<byte>()
        };
        return await ThirdwebContract.Write(wallet, contract, "claim", weiValue, parameters);
    }*/


    /*public static async Task<TotalCosts> GetTotalCosts(this ThirdwebContract contract, IThirdwebWallet wallet, string receiverAddress, string amount)
    {
        if (contract == null)
        {
            throw new ArgumentNullException("contract");
        }

        if (wallet == null)
        {
            throw new ArgumentNullException("wallet");
        }

        if (string.IsNullOrEmpty(receiverAddress))
        {
            throw new ArgumentException("Receiver address must be provided");
        }

        if (string.IsNullOrEmpty(amount))
        {
            throw new ArgumentException("Amount must be provided");
        }

        Drop_ClaimCondition activeClaimCondition = await contract.DropERC20_GetActiveClaimCondition();
        int toDecimals = await contract.ERC20_Decimals();
        BigInteger bigInteger = BigInteger.Parse(amount.ToWei()).AdjustDecimals(18, toDecimals);
        BigInteger weiValue = ((activeClaimCondition.Currency == "0xEeeeeEeeeEeEeeEeEeEeeEEEeeeeEeeeeeeeEEeE") ? (bigInteger * activeClaimCondition.PricePerToken / BigInteger.Pow(10, 18)) : BigInteger.Zero);
        object[] array = new object[4]
        {
            Array.Empty<byte>(),
            BigInteger.Zero,
            BigInteger.Zero,
            "0x0000000000000000000000000000000000000000"
        };
        object[] parameters = new object[6]
        {
            receiverAddress,
            bigInteger,
            activeClaimCondition.Currency,
            activeClaimCondition.PricePerToken,
            array,
            Array.Empty<byte>()
        };

        var transaction = await ThirdwebContract.Prepare(wallet, contract, "claim", weiValue, parameters).ConfigureAwait(continueOnCapturedContext: false);


        TotalCosts totalCosts = await ThirdwebTransaction.EstimateGasCosts(transaction).ConfigureAwait(continueOnCapturedContext: false);
        BigInteger bigInteger1 = transaction.Input.Value?.Value ?? ((BigInteger)0);
        TotalCosts result = default(TotalCosts);
        result.Ether = (bigInteger1 + totalCosts.Wei).ToString().ToEth(18);
        result.Wei = bigInteger1 + totalCosts.Wei;

        Debug.Log("Result.Wei = "+result.Wei);
        Debug.Log("Result.Ether = " + result.Ether);

        return result;

        //return await ThirdwebContract.Write(wallet, contract, "claim", weiValue, parameters);
    }*/


    private static ClaimPossibilityData possibilityData = new ClaimPossibilityData();

    public static async Task<ClaimPossibilityData> IsClaimPossible(int amt)
    {
        BlockchainManager.Instance.connectionManager.ShowLoadingScreen(true, "Checking claim requirements...");


        var _chainDetails = await Utils.GetChainMetadata(client: ThirdwebManager.Instance.Client, chainId: BlockchainManager.Instance.currentConfig.ChainId);
        // Calculate native balance
        //var nativeSymbol = await dropErc20Contract.Chain.na


        var balance = await BlockchainManager.Instance.wallet.GetBalance(chainId: BlockchainManager.Instance.currentConfig.ChainId);
        var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
        Debug.Log($"Balance: {balanceEth} {_chainDetails.NativeCurrency.Symbol}");


        var drop_ClaimCondition = await BlockchainManager.Instance.dropErc20Contract.DropERC20_GetActiveClaimCondition();
        Debug.Log("Currency: " + drop_ClaimCondition.Currency);
        Debug.Log("Price per token: " + drop_ClaimCondition.PricePerToken);
        Debug.Log("Quantity: " + amt);


        // Calculate total cost in Wei
        BigInteger totalPrice = BigInteger.Parse(amt.ToString()) * drop_ClaimCondition.PricePerToken;

        Debug.Log($"Total price in Wei: {totalPrice}");
        Debug.Log($"Total price in {_chainDetails.NativeCurrency.Symbol}: {Utils.ToEth(totalPrice.ToString(), 4, true)}");

        BlockchainManager.Instance.connectionManager.ShowLoadingScreen(false);


        // Only compare balance if currency is native (ETH/MATIC)
        if (drop_ClaimCondition.Currency == "0xEeeeeEeeeEeEeeEeEeEeeEEEeeeeEeeeeeeeEEeE")
        {
            possibilityData.nativeBalance = balance;
            possibilityData.requiredAmount = totalPrice;
            possibilityData.isPossible = balance >= totalPrice;

            return possibilityData;
        }
        else
        {
            Debug.Log("Claim does not require native token.TODO");
            return possibilityData; // Token claim cost is in ERC20, not native
        }
    }


    public class ClaimPossibilityData
    {
        public BigInteger nativeBalance;
        public BigInteger requiredAmount;
        public bool isPossible;
    }

}
