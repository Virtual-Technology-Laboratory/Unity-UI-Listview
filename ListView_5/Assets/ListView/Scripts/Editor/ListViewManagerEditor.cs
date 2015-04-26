using UnityEngine;
using UnityEditor;
using System.Collections;

namespace VTL.ListView
{
    [CustomEditor(typeof(ListViewManager))]
    public class ListViewManagerEditor : Editor {
        
        public override void OnInspectorGUI()
        {
            var obj = target as ListViewManager;

            base.OnInspectorGUI();

            if (GUILayout.Button("BuildHeader"))
            {
                obj.BuildHeader();
            }

        }
    }
}
