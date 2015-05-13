using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The ListView is contained in its own name space
using VTL.ListView;

public class SelectedDatabaseController : MonoBehaviour
{
    public ListViewManager listViewManager;
    private ListViewManager thisListViewManager;

    // Use this for initialization
    void Start()
    {
        thisListViewManager = GetComponent<ListViewManager>();
        ListViewManager.SelectionChangeEvent += OnSelectionChange;
    }

    // Update is called once per frame
    public void OnSelectionChange()
    {
        IEnumerator ienObj = listViewManager.Selected();
        var inListView = new List<System.Guid>();

        while (ienObj.MoveNext())
        {
            var guid = (System.Guid)ienObj.Current;
            inListView.Add(guid);

            if (!thisListViewManager.listData.ContainsKey(guid))
                thisListViewManager.AddRow(new object[] { listViewManager.listData[guid]["Name"] }, guid);

        }

        foreach (var item in thisListViewManager.listData)
            if (!inListView.Contains(item.Key))
                thisListViewManager.Remove(item.Key);
    }
}
