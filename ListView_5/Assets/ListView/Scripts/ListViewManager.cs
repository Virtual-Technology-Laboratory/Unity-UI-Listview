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

        public GameObject HeaderElementPrefab;
        public GameObject RowPrefab;
        public GameObject RowElementPrefab;

        private List<GameObject> headerElements = new List<GameObject>();
        private List<GameObject> rows = new List<GameObject>();

        private List<Dictionary<string, object>> listData = new List<Dictionary<string, object>>();

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


        // Test code

        void FixedUpdate()
        {
            if (Input.GetKey("space"))
                AddRow(new object[] { RandomString(4), 
                                      RandomBool(), 
                                      UnityEngine.Random.Range((int)0, (int)100), 
                                      UnityEngine.Random.Range(0, 1f), 
                                      (double)UnityEngine.Random.Range(1e3f, 1e9f), 
                                      RandomDateTime() });
        }

        DateTime RandomDateTime()
        {
            return new DateTime(UnityEngine.Random.Range((int)1900, 2012),
                                UnityEngine.Random.Range((int)1, 13),
                                UnityEngine.Random.Range((int)1, 28));
        }

        bool RandomBool()
        {
            return UnityEngine.Random.Range(0f, 1f) > 0.5f;
        }

        string RandomString(int length) 
        {
            string s = "";
            for (int i=0; i<length; i++)
            {
                s += "abcdefghijklmnopqrstuvwxyz"[UnityEngine.Random.Range(0, 26)];
            }
            return s;
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

        void AddRow(object[] fieldData)
        {
            if (fieldData.Length != headerElementInfo.Count)
                throw new System.Exception("fieldData does not match the size of the table!");

            rows.Add(Instantiate(RowPrefab));
            int indx = rows.Count - 1;
            rows[indx].transform.SetParent(listPanel.transform);
            rows[indx].GetComponent<Row>().Initialize(fieldData, indx, headerElementInfo, RowElementPrefab);
            listPanelRectTransform.sizeDelta =
                new Vector2(listPanelRectTransform.sizeDelta.x, rows.Count * 30f);

            listData.Add(new Dictionary<string, object>());

            for (int i = 0; i < fieldData.Length; i++)
            {
                listData[indx].Add(headerElementInfo[i].text, fieldData[i]);
            }

            listData[indx].Add("__Selected__", false);
            listData[indx].Add("__Index__", indx);

        }

        public void SetRowSelection(int index, bool selectedState)
        {
            listData[index]["__Selected__"] = selectedState;
        }

        public void Sort(string key, bool sortAscending)
        {
            // Use Linq to sort the list of dictionaries
            IEnumerable<Dictionary<string, object>> query;


            if (sortAscending)
                query = listData.OrderBy(x => x.ContainsKey(key) ? x[key] : string.Empty);
            else
                query = listData.OrderByDescending(x => x.ContainsKey(key) ? x[key] : string.Empty);

            // Update the rows
            int i = 0;
            int indx;
            bool selected;
            foreach (Dictionary<string, object> rowDict in query)
            {
                // build the field data array
                object[] fieldData = new object[headerElementInfo.Count];
                for (int j=0; j<headerElementInfo.Count; j++)
                {
                    fieldData[j] = rowDict[headerElementInfo[j].text];
                }
                indx = System.Convert.ToInt32(rowDict["__Index__"]);
                selected = (bool)rowDict["__Selected__"];
                rows[i].GetComponent<Row>().SetFields(fieldData, indx, selected, headerElementInfo);
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

        public void RemoveAt(int index)
        {

        }
    }
}