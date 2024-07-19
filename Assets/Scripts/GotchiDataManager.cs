using PortalDefender.AavegotchiKit;
using PortalDefender.AavegotchiKit.GraphQL;
using System;
using System.Collections;
using System.Collections.Generic;
using Thirdweb;
using UnityEngine;

public class GotchiDataManager : MonoBehaviour
{
    public static GotchiDataManager Instance { get; private set; }

    public GraphManager graphManager;
    public GotchiSelectCanvas gotchiSelectCanvas;

    public GotchiSvgStyling stylingGame;
    public GotchiSvgStyling stylingUI;

    public List<GotchiData> gotchiData = new List<GotchiData>();
    public List<GotchiSvgs> gotchiSvgs = new List<GotchiSvgs>();
    public List<string> gotchiIds = new List<string>();

    private void Awake()
    {
        Instance = this;
    }

    public GotchiData GetSelectedGotchiData()
    {
        return gotchiData[0];
    }

    public async void FetchGotchiData()
    {
        // 1. check thirdweb for connection
        try
        {
            gotchiData.Clear();
            gotchiSvgs.Clear();
            gotchiIds.Clear();

            // get wallet address
            var walletAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
            walletAddress = walletAddress.ToLower();
            Debug.Log("Got wallet address: " + walletAddress);

            // fetch gotchis with aavegotchi kit
            var userAccount = await graphManager.GetUserAccount(walletAddress);
            Debug.Log("Got user account no. gotchis: " + userAccount.gotchisOwned.Length);

            // save base gotchi data
            foreach (var gotchi in userAccount.gotchisOwned)
            {
                gotchiData.Add(gotchi);
                gotchiIds.Add(gotchi.id.ToString());
                Debug.Log(gotchi.id);
            }

            // get svgs
            var svgs = await graphManager.GetGotchiSvgs(gotchiIds);
            Debug.Log("Got svgs, processing...");
            foreach (var svgSet in svgs)
            {
                gotchiSvgs.Add(new GotchiSvgs
                {
                    svg = svgSet.svg,
                    back = svgSet.back,
                    left = svgSet.left,
                    right = svgSet.right,
                });
            }

            // update canvas
            gotchiSelectCanvas.UpdateGotchiList();
            Debug.Log("Gotchi list updated");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

}

public struct GotchiSvgs
{
    public string svg;
    public string back;
    public string left;
    public string right;
}
