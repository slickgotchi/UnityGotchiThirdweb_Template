using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using Thirdweb;
using GotchiHub;

namespace GotchiHub
{
    public class GotchiSelectCanvas : MonoBehaviour
    {
        public static GotchiSelectCanvas Instance { get; private set; }

        public GotchiDataManager gotchiDataManager;
        public Button SelectGotchiButton;
        public Button VisitAavegotchiButton;
        public GameObject gotchiList;
        public GameObject gotchiListItemPrefab;

        public SVGImage AvatarSvgImage;
        public GotchiStatsCard GotchiStatsCard;
        public GameObject GotchiSelect_Menu;
        public GameObject GotchiSelect_Loading;
        public GameObject GotchiSelect_NoGotchis;

        public TMPro.TextMeshProUGUI LoadingInfoText;

        private string m_walletAddress = "";

        private bool m_isShowButtonJustClicked = false;

        public enum ReorganizeMethod
        {
            BRSLowToHigh,
            BRSHighToLow,
            IdLowToHigh,
            IdHighToLow
        }

        private void Awake()
        {
            Instance = this;
            SelectGotchiButton.onClick.AddListener(HandleOnClick_GotchiSelect_ShowButton);
            VisitAavegotchiButton.onClick.AddListener(HandleOnClick_VisitAavegotchiButton);
        }

        private void Start()
        {
            GotchiSelect_Menu.SetActive(false);
            GotchiSelect_Loading.SetActive(false);
            GotchiSelect_NoGotchis.SetActive(false);

            // Clear out gotchi list children
            ClearGotchiListChildren();
        }

        void HandleOnClick_VisitAavegotchiButton()
        {
            Application.OpenURL("dapp.aavegotchi.com");
        }

        async void HandleOnClick_GotchiSelect_ShowButton()
        {
            try
            {
                // Show loading
                GotchiSelect_Loading.SetActive(true);
                GotchiSelect_Menu.SetActive(false);

                var address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();

                if (address != m_walletAddress)
                {
                    m_walletAddress = address;
                    await gotchiDataManager.FetchGotchiData();

                    if (gotchiDataManager.gotchiData.Count > 0)
                    {
                        // Highlight the highest brs gotchi
                        HighlightById(gotchiDataManager.GetSelectedGotchiId());
                    }
                }

                // Show menu
                GotchiSelect_Loading.SetActive(false);
                GotchiSelect_Menu.SetActive(gotchiDataManager.gotchiData.Count > 0);
                GotchiSelect_NoGotchis.SetActive(gotchiDataManager.gotchiData.Count <= 0);

            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }

            m_isShowButtonJustClicked = true;
        }

        private void Update()
        {
            // Check for clicks outside menu box
            if (Input.GetMouseButtonDown(0) && !m_isShowButtonJustClicked && GotchiSelect_Menu.activeSelf)
            {
                var menuRect = GotchiSelect_Menu.GetComponent<RectTransform>();
                var isMenuClick = RectTransformUtility.RectangleContainsScreenPoint(menuRect, Input.mousePosition);

                // Hide menus
                if (!isMenuClick)
                {
                    GotchiSelect_Loading.SetActive(false);
                    GotchiSelect_Menu.SetActive(false);
                }
            }

            m_isShowButtonJustClicked = false;

            // update loading info text (if its active)
            if (LoadingInfoText.enabled)
            {
                LoadingInfoText.text = gotchiDataManager.Status;
            }
        }

        private void InitAvatarById(int id)
        {
            var gotchiSvg = GotchiDataManager.Instance.GetGotchiSvgsById(id);
            if (gotchiSvg == null) return;

            AvatarSvgImage.sprite =
                SvgLoader.CreateSvgSprite(GotchiDataManager.Instance.stylingUI.CustomizeSVG(gotchiSvg.svg), Vector2.zero);
            GotchiStatsCard.UpdateStatsCard();
        }

        public void HighlightById(int id)
        {
            var gotchiData = GotchiDataManager.Instance.GetGotchiDataById(id);
            if (gotchiData == null) return;

            // Deselect all selections except our chosen
            for (int i = 0; i < gotchiList.transform.childCount; i++)
            {
                var listItem = gotchiList.transform.GetChild(i).GetComponent<GotchiSelectListItem>();
                listItem.SetSelected(listItem.Id == id);
            }

            // Set our avatar
            InitAvatarById(id);
        }

        public void UpdateGotchiList()
        {
            // Clear out existing children
            ClearGotchiListChildren();

            // Create new instance of gotchi list item and set parent to gotchi list
            var gotchiSvgs = gotchiDataManager.gotchiSvgs;
            var gotchiData = gotchiDataManager.gotchiData;
            for (int i = 0; i < gotchiSvgs.Count; i++)
            {
                var newListItem = Instantiate(gotchiListItemPrefab);
                newListItem.transform.SetParent(gotchiList.transform, false);

                var listItem = newListItem.GetComponent<GotchiSelectListItem>();
                listItem.InitById(gotchiData[i].id);
            }

            // Organize list by BRS
            ReorganizeList(ReorganizeMethod.BRSHighToLow); // Example usage, you can change the method as needed
            HighlightById(gotchiDataManager.GetSelectedGotchiId());

        }

        void ClearGotchiListChildren()
        {
            // Create a list of children to destroy
            List<GameObject> children = new List<GameObject>();
            for (int i = 0; i < gotchiList.transform.childCount; i++)
            {
                children.Add(gotchiList.transform.GetChild(i).gameObject);
            }

            // Destroy all children
            foreach (var child in children)
            {
                Destroy(child);
            }
        }

        public void ReorganizeList(ReorganizeMethod method)
        {
            List<GotchiSelectListItem> gotchiListItems = new List<GotchiSelectListItem>();

            // Get all GotchiSelect_ListItem components under the parent
            foreach (Transform child in gotchiList.transform)
            {
                GotchiSelectListItem listItem = child.GetComponent<GotchiSelectListItem>();
                if (listItem != null)
                {
                    gotchiListItems.Add(listItem);
                }
            }

            // Sort the list based on the selected method
            switch (method)
            {
                case ReorganizeMethod.BRSLowToHigh:
                    gotchiListItems.Sort((item1, item2) => item1.BRS.CompareTo(item2.BRS));
                    break;
                case ReorganizeMethod.BRSHighToLow:
                    gotchiListItems.Sort((item1, item2) => item2.BRS.CompareTo(item1.BRS));
                    break;
                case ReorganizeMethod.IdLowToHigh:
                    gotchiListItems.Sort((item1, item2) => item1.Id.CompareTo(item2.Id));
                    break;
                case ReorganizeMethod.IdHighToLow:
                    gotchiListItems.Sort((item1, item2) => item2.Id.CompareTo(item1.Id));
                    break;
            }

            // Reorder the child transforms based on the sorted list
            for (int i = 0; i < gotchiListItems.Count; i++)
            {
                gotchiListItems[i].transform.SetSiblingIndex(i);
            }

            Debug.Log("Gotchi list has been reorganized by " + method.ToString());
        }
    }
}