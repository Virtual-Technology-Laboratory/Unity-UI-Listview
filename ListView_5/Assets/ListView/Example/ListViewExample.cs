using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The ListView is contained in its own name space
using VTL.ListView;

public class ListViewExample : MonoBehaviour
{
    public ListViewManager listViewManager;

    // gets called (less frequently) on physics updates
    void FixedUpdate()
    {
        if (Input.GetKey("space"))
            listViewManager.AddRow(RandomRowData());

        if (Input.GetKeyDown("backspace"))
            listViewManager.RemoveSelected();

        if (Input.GetKeyDown("0"))
            listViewManager.RemoveAt(0);

        if (Input.GetKeyDown("p"))
            PrintSelectedRows();

        if (Input.GetKeyDown("u"))
            UpdateFirstRow();

    }

    void PrintSelectedRows()
    {
        // The ListViewManager contains a Dictionary<Guid, Dictionary<string, object>>
        // instance storing the data. This variable is public and called listData.
        // If you want to iterate over all the key, value pairs you can just manipulate
        // this directly

        IEnumerator ienObj = listViewManager.Selected();

        while (ienObj.MoveNext())
        {
            Dictionary<string, object> rowData = listViewManager.listData[(System.Guid)ienObj.Current];

            string s = "";
            foreach (var item in rowData)
            {
                s += string.Format("{0}={1}, ", item.Key, item.Value);
            }
            Debug.Log(s);
        }
    }

    void UpdateFirstRow()
    {
        listViewManager.UpdateRow(0, RandomRowData());
    }

    // Assumes you have specified the datatypes for the header elements in the ListView Prefab
    object[] RandomRowData()
    {
        return new object[] { RandomString(4), 
                              RandomString(4), 
                              RandomString(4), 
                              RandomDateTime(),  
                              RandomDateTime(),
                              RandomString(4) };
    }

    System.DateTime RandomDateTime()
    {
        return new System.DateTime(Random.Range((int)1900, 2012),
                                   Random.Range((int)1, 13),
                                   Random.Range((int)1, 28));
    }

    bool RandomBool()
    {
        return Random.Range(0f, 1f) > 0.5f;
    }

    string RandomString(int length)
    {
        string s = "";
        for (int i = 0; i < length; i++)
        {
            s += "abcdefghijklmnopqrstuvwxyz"[Random.Range(0, 26)];
        }
        return s;
    }

}
