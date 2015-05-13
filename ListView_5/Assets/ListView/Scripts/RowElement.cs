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
    public class RowElement : MonoBehaviour
    {
        void Start()
        {
            // Need to push click event to the parent Row component
            gameObject.GetComponent<Button>()
                      .onClick
                      .AddListener(transform.parent
                                            .gameObject
                                            .GetComponent<Row>().OnSelectionEvent);

        }

    }
}
