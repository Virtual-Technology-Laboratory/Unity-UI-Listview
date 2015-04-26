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
using System.Collections;

namespace VTL.ListView
{
    public class HeaderElement : MonoBehaviour
    {

        public string text = "Item1";
        public DataType dataType = DataType.String;
        public float preferredWidth = 25f;

        public bool? sortAscending = null;

        private ListViewManager listViewManager;

        private GameObject ascendIcon;
        private GameObject descendIcon;

        public void Initialize(HeaderElementInfo info)
        {
            text = info.text;
            dataType = info.dataType;
            preferredWidth = info.preferredWidth;

            OnValidate();
        }

        void OnValidate()
        {
            transform.Find("Text").gameObject.GetComponent<Text>().text = text;
            gameObject.GetComponent<LayoutElement>().preferredWidth = preferredWidth;
        }

        // Use this for initialization
        void Start()
        {
            listViewManager = transform.parent.transform.parent.gameObject.GetComponent<ListViewManager>();
            gameObject.GetComponent<Button>().onClick.AddListener(SortHandler);

            ascendIcon = transform.Find("SortAscending").gameObject;
            descendIcon = transform.Find("SortDescending").gameObject;
        }

        public void SortHandler()
        {
            if (sortAscending == null || sortAscending == false)
                sortAscending = true;
            else
                sortAscending = false;
            listViewManager.Sort(text, (bool)sortAscending);
           
        }

        public void SetSortState(bool? theState)
        {
            sortAscending = theState;

            if (theState == true)
            {
                ascendIcon.SetActive(true);
                descendIcon.SetActive(false);
            }
            else if (theState == false)
            {
                ascendIcon.SetActive(false);
                descendIcon.SetActive(true);
            }
            else
            {
                ascendIcon.SetActive(false);
                descendIcon.SetActive(false);
            }
        }
    }
}
