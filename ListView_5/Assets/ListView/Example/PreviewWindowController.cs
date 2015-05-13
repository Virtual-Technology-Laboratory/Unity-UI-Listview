using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using VTL.ListView;

public class PreviewWindowController : MonoBehaviour
{
    private ListViewManager selectedListViewManager;
    
    Text text;

   
    // Use this for initialization
    void Start()
    {
        text = transform.FindChild("Text").GetComponent<Text>();

        selectedListViewManager = transform.parent.FindChild("LoadList_ListView").GetComponent<ListViewManager>();
        ListViewManager.SelectionChangeEvent += OnSelectionChange;
    }

    // Update is called once per frame
    public void OnSelectionChange()
    {
        IEnumerator ienObj = selectedListViewManager.Selected();

        int count = 0;
        while (ienObj.MoveNext())
        {
            var guid = (System.Guid)ienObj.Current;
            text.text = string.Format("Loading {0}...", selectedListViewManager.listData[guid]["Name"]);
            count++;
        }

        if (count == 0)
            text.text = "";
    }
}
