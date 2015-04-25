using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


namespace VTL.ListView
{
    public class RowElement : MonoBehaviour
    {
        void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(transform.parent.gameObject.GetComponent<Row>().OnSelectEvent);
        }

    }
}
