using PortalDefender.AavegotchiKit;
using PortalDefender.AavegotchiKit.GraphQL;
using System;
using System.Collections;
using System.Collections.Generic;
using Thirdweb;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace GotchiHub
{
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

        private int m_selectedGotchiId = 0;

        public string Status = "";

        // Event declaration
        public event Action<int> onSelectedGotchi;

        private void Awake()
        {
            Instance = this;
        }

        public int GetSelectedGotchiId() { return m_selectedGotchiId; }

        //private bool SetSelectedGotchiByIndex(int index)
        //{
        //    if (index >= gotchiData.Count)
        //    {
        //        Debug.Log("Can not set index " + index + " with gotchiData count of " + gotchiData.Count);
        //        return false;
        //    }

        //    m_selectedGotchiId = gotchiData[index].id;
        //    onSelectedGotchi?.Invoke(m_selectedGotchiId); // Trigger event
        //    return true;
        //}

        public bool SetSelectedGotchiById(int id)
        {
            for (int i = 0; i < gotchiData.Count; i++)
            {
                if (id == gotchiData[i].id)
                {
                    m_selectedGotchiId = id;
                    onSelectedGotchi?.Invoke(m_selectedGotchiId); // Trigger event
                    return true;
                }
            }

            Debug.Log("Gotchi with id " + id + " does not exist in GotchiDataManager");
            return false;
        }

        private int GetGotchiIndexById(int id)
        {
            for (int i = 0; i < gotchiData.Count; i++)
            {
                if (id == gotchiData[i].id)
                {
                    return i;
                }
            }

            return -1;
        }

        public GotchiData GetGotchiDataById(int id)
        {
            var index = GetGotchiIndexById(id);
            if (index < 0)
            {
                Debug.Log("Invalid id passed to GetGotchiDataById()");
                return null;
            }

            return gotchiData[index];
        }

        public GotchiData GetGotchiDataBySelected()
        {
            return GetGotchiDataById(m_selectedGotchiId);
        }

        public GotchiSvgs GetGotchiSvgsById(int id)
        {
            var index = GetGotchiIndexById(id);
            if (index < 0)
            {
                Debug.Log("Invalid id passed to GetGotchSvgsById()");
                return null;
            }

            return gotchiSvgs[index];
        }

        public GotchiSvgs GetGotchiSvgsBySelected()
        {
            return GetGotchiSvgsById(m_selectedGotchiId);
        }

        public async UniTask FetchGotchiData()
        {
            // 1. check thirdweb for connection
            try
            {
                gotchiData.Clear();
                gotchiSvgs.Clear();
                gotchiIds.Clear();

                // get wallet address
                Status = "Validating wallet address...";
                var walletAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
                walletAddress = walletAddress.ToLower();


                // fetch gotchis with aavegotchi kit
                Status = "Wallet validated. Fetching gotchi data...";
                var userAccount = await graphManager.GetUserAccount(walletAddress);

                // save base gotchi data
                foreach (var gotchi in userAccount.gotchisOwned)
                {
                    gotchiData.Add(gotchi);
                    gotchiIds.Add(gotchi.id.ToString());
                }

                // get svgs
                Status = userAccount.gotchisOwned.Length.ToString() + " gotchis found. Processing SVGs...";
                var svgs = await graphManager.GetGotchiSvgs(gotchiIds);
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

                // default to highest brs gotchi
                Status = "SVGs processed. Updating gotchi inventory...";
                if (gotchiData.Count > 0)
                {
                    SetSelectedGotchiById(GetGotchiIdOfHighestBRS());

                    // update canvas
                    gotchiSelectCanvas.UpdateGotchiList();
                }

            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        public int GetGotchiIdOfHighestBRS()
        {
            int highestBrs = 0;
            int highestBrsId = -1;

            for (int i = 0; i < gotchiData.Count; i++)
            {
                var brs = DroptStatCalculator.GetBRS(gotchiData[i].numericTraits);
                if (brs > highestBrs)
                {
                    highestBrs = brs;
                    highestBrsId = gotchiData[i].id;
                }
            }

            return highestBrsId;
        }
    }

    public class GotchiSvgs
    {
        public string svg;
        public string back;
        public string left;
        public string right;
    }
}
