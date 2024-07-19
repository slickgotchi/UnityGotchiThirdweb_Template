using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class GotchiSelectCanvas : MonoBehaviour
{
    public GotchiDataManager gotchiDataManager;
    public Button SelectGotchiButton;
    public GameObject gotchiList;
    public GameObject gotchiListItemPrefab;

    private void Awake()
    {
        SelectGotchiButton.onClick.AddListener(() => gotchiDataManager.FetchGotchiData());
    }

    public void UpdateGotchiList()
    {
        // 1. clear out existing children
        ClearGotchiListChildren();

        Debug.Log(gotchiDataManager.gotchiSvgs.Count);

        // 2. create new instance of gotchi list item and set parent to gotchi list
        var gotchiSvgs = gotchiDataManager.gotchiSvgs;
        foreach (var gotchiSvgSet in gotchiSvgs)
        {
            var newListItem = Instantiate(gotchiListItemPrefab);
            newListItem.transform.SetParent(gotchiList.transform, false);
            newListItem.GetComponentInChildren<SVGImage>().sprite = SvgLoader.CreateSvgSprite(gotchiDataManager.stylingUI.CustomizeSVG(gotchiSvgSet.svg), Vector2.zero);
        }
        Debug.Log("Added UI sprites");
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
        Debug.Log("Destroyed existing gotchi select list");
    }
}
