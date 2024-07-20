using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;

public class GotchiSelect_ListItem : MonoBehaviour
{
    public int Id = 0;

    private Button m_button;
    private TMPro.TextMeshProUGUI m_nameText;
    private SVGImage m_svgImage;
    private Image m_selectedHighlightImage;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_nameText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        m_svgImage = GetComponentInChildren<SVGImage>();
        m_selectedHighlightImage = GetComponentInChildren<Image>();

        m_button.onClick.AddListener(HandleOnClick);
    }

    private void Start()
    {
        m_selectedHighlightImage.enabled = false;
    }

    public void InitById(int id)
    {
        Id = id;
        var gotchiSvg = GotchiDataManager.Instance.GetGotchiSvgsById(id);
        var gotchiData = GotchiDataManager.Instance.GetGotchiDataById(id);
        m_svgImage.sprite = SvgLoader.CreateSvgSprite(GotchiDataManager.Instance.stylingUI.CustomizeSVG(gotchiSvg.svg), Vector2.zero);
        m_nameText.text = gotchiData.name;
    }

    private void HandleOnClick()
    {
        GotchiDataManager.Instance.SetSelectedGotchiById(Id);

        GotchiSelectCanvas.Instance.SetAvatarById(Id);

        // unhighlight all other list items
        var parent = transform.parent;
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).GetComponent<GotchiSelect_ListItem>().m_selectedHighlightImage.enabled = false;
        }
        m_selectedHighlightImage.enabled = true;
    }

}
