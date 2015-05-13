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

namespace VTL.ListView
{


    public enum DataType { String, Bool, Int, Float, Double, DateTime, TimeSpan };
    public enum HorizontalAlignment { Left, Right };
    public enum ListSelection { Many, One, None };

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
        public delegate void SelectionChangeAction();
        public static event SelectionChangeAction SelectionChangeEvent;

        public List<HeaderElementInfo> headerElementInfo = new List<HeaderElementInfo>();

        public float rowHeight = 26f;
        public Color unselectedColor = Color.white;
        public Color selectedColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
        public ListSelection listSelection = ListSelection.Many;

        public GameObject HeaderElementPrefab;
        public GameObject RowPrefab;
        public GameObject RowElementPrefab;

        List<GameObject> headerElements = new List<GameObject>();
        Dictionary<Guid, GameObject> rows = new Dictionary<Guid, GameObject>();

        [HideInInspector]
        public Dictionary<Guid, Dictionary<string, object>> listData = 
            new Dictionary<Guid, Dictionary<string, object>>();
        const string SELECTED = "__Selected__";
        const string GUID = "__Guid__";

        GameObject header;
        GameObject listPanel;
        RectTransform listPanelRectTransform;
        
        [HideInInspector] 
        public bool shiftDown = false;

        [HideInInspector]
        public List<int> shiftDownSelections = new List<int>();

        // Use this for initialization
        void Awake()
        {
            header = transform.Find("Header").gameObject;
            listPanel = transform.Find("List/ListPanel").gameObject;
            listPanelRectTransform = listPanel.GetComponent<RectTransform>();

            // Destroy unneeded header elements
            foreach (Transform child in header.transform)
                if (!child.gameObject.activeSelf)
                    Destroy(child.gameObject);
        }

        void Update()
        {
            shiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            // On shift keyup we can reset the selection
            if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                shiftDownSelections.Clear();
        }

        public void OnValidate()
        {
            if (headerElementInfo.Count > 32)
                throw new System.Exception("Add additional HeaderElement prefabs as children of Header");

            header = transform.Find("Header").gameObject;
            listPanel = transform.Find("List/ListPanel").gameObject;

            // reset all the header elements to inactive
            foreach (Transform child in header.transform)
                child.gameObject.SetActive(false);

            // Need to make sure that duplicate column names are not present
            // This HashSet is used to make sure duplicates do not exist.
            HashSet<string> keys = new HashSet<string>();

            // Loop through and setup the header elements
            headerElements.Clear();
            int i = 0;
            foreach (var info in headerElementInfo)
            {
                if (keys.Contains(info.text))
                    throw new System.Exception("ListView header elements must have distinct Text properties.");
                keys.Add(info.text);

                // For whatever reason it runs OnValidate when you hit play and it fails to 
                // find the children of header. At this point Application.isPlaying is still false
                // so it isn't clear how to cleanly detect this special state. Anyhoo, that is why
                // this try/catch is needed.
                try
                {
                    headerElements.Add(header.transform.GetChild(i).gameObject);
                    headerElements[i].SetActive(true);
                    headerElements[i].GetComponent<HeaderElement>().Initialize(info);
                }
                catch { return; }
                i++;
            }
        }

        void SetListPanelHeight()
        {
            listPanelRectTransform.sizeDelta =
                new Vector2(listPanelRectTransform.sizeDelta.x, rows.Count * rowHeight);
        }

        public void AddRow(object[] fieldData, Guid guid)
        {
            if (fieldData.Length != headerElementInfo.Count)
                throw new System.Exception("fieldData does not match the size of the table!");

            rows.Add(guid, Instantiate(RowPrefab));
            rows[guid].transform.SetParent(listPanel.transform);
            rows[guid].GetComponent<Row>().Initialize(fieldData, guid);
            SetListPanelHeight();

            listData.Add(guid, new Dictionary<string, object>());

            for (int i = 0; i < fieldData.Length; i++)
                listData[guid].Add(headerElementInfo[i].text, fieldData[i]);

            listData[guid].Add(SELECTED, false);
            listData[guid].Add(GUID, guid);
        }

        public void AddRow(object[] fieldData)
        {
            Guid guid = Guid.NewGuid();
            AddRow(fieldData, guid);
        }

        public void OnSelectionEvent(Guid guid, int index)
        {
            // The selection handling is a little convoluted. Basically each 
            // row element is a button. For each button the click event is
            // bound to their parent's Row component which passes the event
            // here.
            //
            // In this method we the selection logic and the SetRowSelection
            // method calls back to set the appearance of the row.
            if (listSelection == ListSelection.Many)
            {
                if (shiftDown)
                {
                    shiftDownSelections.Add(index);

                    if (shiftDownSelections.Count == 1)
                        SetRowSelection(guid, true);
                    else
                    {
                        int minIndx = Mathf.Min(shiftDownSelections.ToArray());
                        int maxIndx = Mathf.Max(shiftDownSelections.ToArray());
                        for (int i = minIndx; i < maxIndx + 1; i++)
                            SetRowSelection(i, true);
                    }
                }
                else
                    SetRowSelection(guid, !rows[guid].GetComponent<Row>().isSelected);
            }
            else if (listSelection == ListSelection.One)
            {
                bool newState = !rows[guid].GetComponent<Row>().isSelected;
                DeselectAll();
                SetRowSelection(guid, newState);
            }
            else
            {
                return;
            }

            if (SelectionChangeEvent != null)
                SelectionChangeEvent();
        }

        public void SelectAll()
        {
            foreach (var item in rows)
                SetRowSelection(item.Key, true);
        }

        public void DeselectAll()
        {
            foreach (var item in rows)
                SetRowSelection(item.Key, false);
        }

        public void SetRowSelection(int index, bool selectedState)
        {
            SetRowSelection(GetGuidAtIndex(index), selectedState);
        }

        public void SetRowSelection(Guid guid, bool selectedState)
        {
            listData[guid][SELECTED] = selectedState;

            Row row = rows[guid].GetComponent<Row>();
            row.isSelected = selectedState;
            row.UpdateSelectionAppearance();
        }

        public void Sort(string key)
        {
            Sort(key, true);
        }

        public void Sort(string key, bool sortAscending)
        {
            // Check that key is valid
            bool foundKey = false;
            foreach (var info in headerElementInfo)
                if (info.text.Equals(key))
                    foundKey = true;

            if (!foundKey)
                throw new System.Exception("Key not in listview: " + key);

            // Here we sort without Linq for maximum platform compatibility
            // We only need to sort unique elements of a column. So we create
            // a lookup dictionary to get all Guids that coorespond to a
            // particular unique element
            Dictionary<object, List<Guid>> lookup = new Dictionary<object, List<Guid>>();
            foreach (var item in listData)
            {
                if (!lookup.ContainsKey(item.Value[key]))
                    lookup[item.Value[key]] = new List<Guid>();

                lookup[item.Value[key]].Add(item.Key);
            }

            // Now sort the keys to the lookup table
            List<object> uniqueElements = new List<object>(lookup.Keys);
            uniqueElements.Sort(); // Sort in place

            if (!sortAscending)
                uniqueElements.Reverse(); // Reverse in place

            // Reorder the rows
            int i = 0;
            foreach (object objKey in uniqueElements)
            {
                foreach (Guid guid in lookup[objKey])
                    rows[guid].transform.SetSiblingIndex(i++);
            }

            // Set the arrow states for the header fields
            foreach (Transform child in header.transform)
            {
                var headerElement = child.GetComponent<HeaderElement>();
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
                listData[guid][headerElementInfo[i].text] = fieldData[i];

            bool selected = (bool)listData[guid][SELECTED];
            rows[guid].GetComponent<Row>().SetFields(fieldData, guid, selected);
        }

        public void UpdateRow(int index, object[] fieldData)
        {
            UpdateRow(GetGuidAtIndex(index), fieldData);
        }

        public void UpdateRow(Guid guid, Dictionary<string, object> rowData)
        {
            foreach (var item in rowData)
                listData[guid][item.Key] = item.Value;

            bool selected = (bool)listData[guid][SELECTED];
            rows[guid].GetComponent<Row>().SetFields(listData[guid], guid, selected);
        }

        public void UpdateRow(int index, Dictionary<string, object> rowData)
        {
            UpdateRow(GetGuidAtIndex(index), rowData);
        }

        public void UpdateRowField(Guid guid, string key, object data)
        {
            listData[guid][key] = data;

            bool selected = (bool)listData[guid][SELECTED];
            rows[guid].GetComponent<Row>().SetFields(listData[guid], guid, selected);
        }

        public void UpdateRowField(int index, string key, object data)
        {
            UpdateRowField(GetGuidAtIndex(index), key, data);
        }

        public IEnumerator Selected()
        {
            var buffer = new List<Guid>();
            foreach (var rowData in listData.Values)
                if ((bool)rowData[SELECTED])
                    buffer.Add((Guid)rowData[GUID]);

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