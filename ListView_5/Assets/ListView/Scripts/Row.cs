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
    public class Row : MonoBehaviour
    {
        public bool isSelected = false;
        public Guid guid;

        public List<GameObject> rowElements = new List<GameObject>();

        ListViewManager listViewManager; 
        Image image;

        public void Initialize(object[] fieldData, Guid guid)
        {
            // Because these are instantiated this gets called before Start so
            // it is easier to just find the listViewManager here
            listViewManager = transform.parent.
                              transform.parent.
                              transform.parent.gameObject.GetComponent<ListViewManager>();

            transform.localScale = Vector3.one;

            // Need a reference to this to set the background color
            image = gameObject.GetComponent<Image>();

            this.guid = guid;

            // Build the row elements (cells)
            rowElements = new List<GameObject>();
            for (int i=0; i<fieldData.Length; i++)
            {
                // For each cell add a new RowElementPrefab and set the row as its parent
                rowElements.Add(Instantiate(listViewManager.RowElementPrefab));
                rowElements[i].transform.SetParent(transform);
                rowElements[i].transform.localScale = Vector3.one;

                // Set the text
                Text rowElementText = rowElements[i].GetComponentInChildren<Text>();
                rowElementText.text = 
                    StringifyObject(fieldData[i],
                                    listViewManager.headerElementInfo[i].formatString,
                                    listViewManager.headerElementInfo[i].dataType);

                // Set the preferred width
                rowElements[i].GetComponentInChildren<LayoutElement>()
                              .preferredWidth = listViewManager.headerElementInfo[i].preferredWidth;

                // Set the horizontal alignment
                if (listViewManager.headerElementInfo[i].horizontalAlignment == HorizontalAlignment.Left)
                    rowElementText.alignment = TextAnchor.MiddleLeft;
                else
                    rowElementText.alignment = TextAnchor.MiddleRight;
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

        public void UpdateSelectionAppearance()
        {
            image.color = isSelected ? listViewManager.selectedColor : 
                                       listViewManager.unselectedColor;
        }

        public void SetFields(object[] fieldData, Guid guid, bool selected)
        {
            this.guid = guid;
            isSelected = selected;
            UpdateSelectionAppearance();

            for (int i = 0; i < listViewManager.headerElementInfo.Count; i++)
                rowElements[i].GetComponentInChildren<Text>().text =
                    StringifyObject(fieldData[i],
                                    listViewManager.headerElementInfo[i].formatString,
                                    listViewManager.headerElementInfo[i].dataType); 
        }

        public void SetFields(Dictionary<string, object> rowData, Guid guid, bool selected)
        {
            this.guid = guid;
            isSelected = selected;
            UpdateSelectionAppearance();

            for (int i = 0; i < listViewManager.headerElementInfo.Count; i++)
                rowElements[i].GetComponentInChildren<Text>().text =
                    StringifyObject(rowData[listViewManager.headerElementInfo[i].text],
                                    listViewManager.headerElementInfo[i].formatString,
                                    listViewManager.headerElementInfo[i].dataType);
        }

        public void OnSelectionEvent()
        {
            listViewManager.OnSelectionEvent(guid, transform.GetSiblingIndex());
        }
    }
}
