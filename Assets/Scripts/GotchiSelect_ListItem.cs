using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GotchiSelect_ListItem : MonoBehaviour
{
    public int Id = 0;

    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(HandleOnClick);
    }

    private void HandleOnClick()
    {
        //GotchiDataManager.Instance.
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
