/*
 * Copyright (c) 2015, Roger Lew (rogerlew.gmail.com)
 * Date: 4/25/2015
 * License: BSD (3-clause license)
 * 
 * The project described was supported by NSF award number IIA-1301792
 * from the NSF Idaho EPSCoR Program and by the National Science Foundation.
 * 
 */
 
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VTL.ListView
{

    public enum DataType { String, Bool, Int, Float, Double, DateTime, TimeSpan };
    public enum HorizontalAlignment {Left, Right};

    [System.Serializable]
    public class HeaderElementInfo
    {
        public string text = "Item0";
        public DataType dataType = DataType.String;
        public string formatString = null;
        public float preferredWidth = 150f;
        public HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
    }

    public class ListViewManager : MonoBehaviour
    {
        public List<HeaderElementInfo> headerElementInfo = new List<HeaderElementInfo>();

        public float rowHeight = 26f;

        public GameObject HeaderElementPrefab;
        public GameObject RowPrefab;
        public GameObject RowElementPrefab;

        private List<GameObject> headerElements = new List<GameObject>();
        private Dictionary<Guid, GameObject> rows = new Dictionary<Guid, GameObject>();

        public Dictionary<Guid, Dictionary<string, object>> listData = new Dictionary<Guid, Dictionary<string, object>>();

        GameObject header;
        GameObject listPanel;
        RectTransform listPanelRectTransform;

        // Use this for initialization
        void Awake()
        {
            header = transform.Find("Header").gameObject;
            listPanel = transform.Find("List/ListPanel").gameObject;
            listPanelRectTransform = listPanel.GetComponent<RectTransform>();
        }


        public void BuildHeader()
        {
            header = transform.Find("Header").gameObject;
            listPanel = transform.Find("List/ListPanel").gameObject;

            //headerElements.RemoveRange(0, headerElements.Count);
            //for (int i = transform.childCount - 1; i >= 0; --i)
            //{
            //    var child = transform.GetChild(i).gameObject;
            //    if (child.GetComponent<HeaderElement>() != null)
            //    {
            //        UnityEditor.EditorApplication.delayCall += () =>
            //        {
            //            DestroyImmediate(child);
            //        };
            //    }
            //} 

            HashSet<string> keys = new HashSet<string>();

            headerElements = new List<GameObject>();
            int indx = 0;
            foreach (HeaderElementInfo info in headerElementInfo)
            {
                if (keys.Contains(info.text))
                    throw new System.Exception("ListView header elements must have distinct Text properties.");

                keys.Add(info.text);

                headerElements.Add(Instantiate(HeaderElementPrefab));
                indx = headerElements.Count - 1;
                headerElements[indx].transform.SetParent(header.transform);
                headerElements[indx].GetComponent<HeaderElement>().Initialize(info);
            }
        }

        void SetListPanelHeight()
        {
            listPanelRectTransform.sizeDelta =
                new Vector2(listPanelRectTransform.sizeDelta.x, rows.Count * rowHeight);
        }

        public void AddRow(object[] fieldData)
        {
            if (fieldData.Length != headerElementInfo.Count)
                throw new System.Exception("fieldData does not match the size of the table!");

            Guid guid = Guid.NewGuid();

            rows.Add(guid, Instantiate(RowPrefab));
            rows[guid].transform.SetParent(listPanel.transform);
            rows[guid].GetComponent<Row>().Initialize(fieldData, guid, headerElementInfo, RowElementPrefab);
            SetListPanelHeight();

            listData.Add(guid, new Dictionary<string, object>());

            for (int i = 0; i < fieldData.Length; i++)
            {
                listData[guid].Add(headerElementInfo[i].text, fieldData[i]);
            }

            listData[guid].Add("__Selected__", false);
            listData[guid].Add("__Guid__", guid);

        }

        public void SetRowSelection(Guid guid, bool selectedState)
        {
            listData[guid]["__Selected__"] = selectedState;
        }

        public void Sort(string key)
        {
            Sort(key, true);
        }

        public void Sort(string key, bool sortAscending)
        {
            bool foundKey = false;
            foreach (HeaderElementInfo info in headerElementInfo)
                if (info.text.Equals(key))
                    foundKey = true;

            if (!foundKey)
                throw new System.Exception("Key not in listview: " + key);

            // Use Linq to sort the list of dictionaries
            IEnumerable<Dictionary<string, object>> query;


            if (sortAscending)
                query = listData.Values.OrderBy(x => x.ContainsKey(key) ? x[key] : string.Empty);
            else
                query = listData.Values.OrderByDescending(x => x.ContainsKey(key) ? x[key] : string.Empty);

            // Reorder the rows
            int i = 0;
            Guid guid;
            foreach (Dictionary<string, object> rowData in query)
            {
                guid = (Guid)rowData["__Guid__"];
                rows[guid].transform.SetSiblingIndex(i);
                i++;
            }

            // Set the arrow states for the header fields
            foreach (Transform child in header.transform)
            {
                HeaderElement headerElement = child.GetComponent<HeaderElement>();
                if (headerElement != null)
                    headerElement.SetSortState(headerElement.text == key ? sortAscending : (bool?)null);
            }

        }

        public Guid GetGuidAtIndex(int index)
        {
            return listPanel.transform.GetChild(index).GetComponent<Row>().guid;
        }

        public void UpdateRow(Guid guid, object[] fieldData)
        {
            if (fieldData.Length != headerElementInfo.Count)
                throw new System.Exception("fieldData does not match the size of the table!");

            for (int i = 0; i < fieldData.Length; i++)
            {
                listData[guid][headerElementInfo[i].text] = fieldData[i];
            }

            bool selected = (bool)listData[guid]["__Selected__"];
            rows[guid].GetComponent<Row>().SetFields(fieldData, guid, selected, headerElementInfo);
        }

        public void UpdateRow(int index, object[] fieldData)
        {
            UpdateRow(GetGuidAtIndex(index), fieldData);
        }

        public void UpdateRow(Guid guid, Dictionary<string, object> rowData)
        {
            foreach (var item in rowData)
                listData[guid][item.Key] = item.Value;

            bool selected = (bool)listData[guid]["__Selected__"];
            rows[guid].GetComponent<Row>().SetFields(listData[guid], guid, selected, headerElementInfo);
        }

        public void UpdateRow(int index, Dictionary<string, object> rowData)
        {
            UpdateRow(GetGuidAtIndex(index), rowData);
        }

        public void UpdateRowField(Guid guid, string key, object data)
        {
            listData[guid][key] = data;

            bool selected = (bool)listData[guid]["__Selected__"];
            rows[guid].GetComponent<Row>().SetFields(listData[guid], guid, selected, headerElementInfo);
        }

        public void UpdateRowField(int index, string key, object data)
        {
            UpdateRowField(GetGuidAtIndex(index), key, data);
        }

        public IEnumerator Selected()
        {
            var buffer = new List<Guid>();
            foreach (Dictionary<string, object> rowData in listData.Values)
            {
                if ((bool)rowData["__Selected__"])
                    buffer.Add((Guid)rowData["__Guid__"]);
            }

            foreach (Guid guid in buffer)
                yield return guid;
        }

        public void RemoveSelected()
        {
            IEnumerator ienObj = Selected();

            while (ienObj.MoveNext())
                Remove((Guid)ienObj.Current);
        }

        public void Remove(Guid guid)
        {
            Destroy(rows[guid]);
            rows.Remove(guid);
            listData.Remove(guid);
            SetListPanelHeight();
        }

        public void RemoveAt(int index)
        {
            Remove(GetGuidAtIndex(index));
        }

    }
}