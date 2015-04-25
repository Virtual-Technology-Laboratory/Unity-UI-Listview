using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VTL.ListView
{
    [System.Serializable]
    public class HeaderElementInfo
    {
        public string Name = "Item0";
        public bool IsNumeric = false;
        public float preferredWidth = 150f;
    }

    public class ListViewManager : MonoBehaviour
    {
        public List<HeaderElementInfo> headerElementInfo = new List<HeaderElementInfo>();

        public GameObject HeaderElementPrefab;
        public GameObject RowPrefab;
        public GameObject RowElementPrefab;

        private List<GameObject> headerElements = new List<GameObject>();
        private List<GameObject> rows = new List<GameObject>();

        private List<Dictionary<string, string>> listData = new List<Dictionary<string, string>>();

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

        void FixedUpdate()
        {
            if (Input.GetKey("space"))
                AddRow(new string[] { RandomString(4), RandomString(4), RandomString(4), RandomString(4), RandomString(4) });
        }

        string RandomString(int length) 
        {
            string s = "";
            for (int i=0; i<length; i++)
            {
                s += "abcdefghijklmnopqrstuvwxyz"[Random.Range(0, 26)];
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

            headerElements = new List<GameObject>();
            int indx = 0;
            foreach (HeaderElementInfo info in headerElementInfo)
            {
                headerElements.Add(Instantiate(HeaderElementPrefab));
                indx = headerElements.Count - 1;
                headerElements[indx].transform.SetParent(header.transform);
                headerElements[indx].GetComponent<HeaderElement>().Initialize(info);
            }
        }

        void AddRow(string[] fieldData)
        {
            if (fieldData.Length != headerElementInfo.Count)
                Debug.Log("fieldData does not match the size of the table!");

            rows.Add(Instantiate(RowPrefab));
            int indx = rows.Count - 1;
            rows[indx].transform.SetParent(listPanel.transform);
            rows[indx].GetComponent<Row>().Initialize(fieldData, indx, headerElementInfo, RowElementPrefab);
            listPanelRectTransform.sizeDelta =
                new Vector2(listPanelRectTransform.sizeDelta.x, rows.Count * 30f);

            listData.Add(new Dictionary<string, string>());

            for (int i = 0; i < fieldData.Length; i++)
            {
                listData[indx].Add(headerElementInfo[i].Name, fieldData[i]);
            }
            listData[indx].Add("__Selected__", "false");
            listData[indx].Add("__Index__", indx.ToString());

        }

        public void SetRowSelection(int index, bool selectedState)
        {
            listData[index]["__Selected__"] = selectedState ? "true" : "false";
        }

        public void Sort(string key, bool sortAscending)
        {
            // Use Linq to sort the list of dictionaries
            IEnumerable<Dictionary<string, string>> query;


            if (sortAscending)
                query = listData.OrderBy(x => x.ContainsKey(key) ? x[key] : string.Empty);
            else
                query = listData.OrderByDescending(x => x.ContainsKey(key) ? x[key] : string.Empty);

            // Update the rows
            int i = 0;
            int indx;
            bool selected;
            foreach (Dictionary<string, string> rowDict in query)
            {
                // build the field data array
                string[] fieldData = new string[headerElementInfo.Count];
                for (int j=0; j<headerElementInfo.Count; j++)
                {
                    fieldData[j] = rowDict[headerElementInfo[j].Name];
                }
                indx = System.Convert.ToInt32(rowDict["__Index__"]);
                selected = rowDict["__Selected__"].Contains("true");
                rows[i].GetComponent<Row>().SetFields(fieldData, indx, selected);
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
    }
}