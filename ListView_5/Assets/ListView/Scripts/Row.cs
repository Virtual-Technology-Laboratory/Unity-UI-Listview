using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace VTL.ListView
{
    public class Row : MonoBehaviour
    {

        private ListViewManager listViewManager;

        public bool IsSelected = false;

        public int Index = 0;

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

        public void Initialize(string[] fieldData, int index,
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
                rowElements[i].GetComponentInChildren<Text>().text = fieldData[i];
            }
        }

        public void SetFields(string[] fieldData, int index, bool selected)
        {
            Index = index;
            IsSelected = selected;
            image.color = IsSelected ? selectedColor : unselectedColor;

            for (int i = 0; i < fieldData.Length; i++)
            {
                rowElements[i].GetComponentInChildren<Text>().text = fieldData[i];
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
