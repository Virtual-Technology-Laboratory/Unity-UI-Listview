using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


namespace VTL.ListView
{
    public class Row : MonoBehaviour
    {

        private ListViewManager listViewManager;

        public bool IsSelected = false;

        public int Index = 0; // Row stores index to track cooresponding reference in 

        private Color unselectedColor = Color.white; //new Color(0, 0, 0, 100 / 255);
        private Color selectedColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);

        private Image image;

        public List<GameObject> rowElements = new List<GameObject>();

        void Start()
        {
            listViewManager = transform.parent.
                              transform.parent.
                              transform.parent.gameObject.GetComponent<ListViewManager>();

            image = gameObject.GetComponent<Image>();
        }

        public void Initialize(object[] fieldData, int index,
                               List<HeaderElementInfo> headerElementInfo, 
                               GameObject RowElementPrefab)
        {
            Index = index;
            transform.localScale = Vector3.one;

            rowElements = new List<GameObject>();

            for (int i=0; i<fieldData.Length; i++)
            {
                rowElements.Add(Instantiate(RowElementPrefab));
                rowElements[i].transform.SetParent(transform);
                rowElements[i].transform.localScale = Vector3.one;
                rowElements[i].GetComponentInChildren<LayoutElement>().preferredWidth = 
                    headerElementInfo[i].preferredWidth;
                Text rowElementText = rowElements[i].GetComponentInChildren<Text>();
                rowElementText.text = 
                    StringifyObject(fieldData[i], headerElementInfo[i].formatString, headerElementInfo[i].dataType); 
                rowElementText.alignment = 
                    headerElementInfo[i].horizontalAlignment == HorizontalAlignment.Left ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
            }
        }

        private static string StringifyObject(object obj, string formatString, DataType dataType)
        {
            if (dataType == DataType.String)
                return (string)obj;
            else if (formatString != null)
            {
                if (dataType == DataType.Int)
                    return ((int)obj).ToString(formatString);
                else if (dataType == DataType.Float)
                    return ((float)obj).ToString(formatString);
                else if (dataType == DataType.Double)
                    return ((double)obj).ToString(formatString);
                else if (dataType == DataType.DateTime)
                    return ((DateTime)obj).ToString(formatString);
                else if (dataType == DataType.TimeSpan)
                    return ((TimeSpan)obj).ToString();
                else
                    return obj.ToString();
            }
            else
                return obj.ToString();
        }

        public void SetFields(object[] fieldData, int index, bool selected,
                              List<HeaderElementInfo> headerElementInfo)
        {
            Index = index;
            IsSelected = selected;
            image.color = IsSelected ? selectedColor : unselectedColor;

            for (int i = 0; i < fieldData.Length; i++)
            {
                rowElements[i].GetComponentInChildren<Text>().text =
                    StringifyObject(fieldData[i], headerElementInfo[i].formatString, headerElementInfo[i].dataType); 
            }
        }

        public void OnSelectEvent()
        {
            IsSelected = !IsSelected;
            image.color = IsSelected ? selectedColor : unselectedColor;
            listViewManager.SetRowSelection(Index, IsSelected);
        }
    }
}
