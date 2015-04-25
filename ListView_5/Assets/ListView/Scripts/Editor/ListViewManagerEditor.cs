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

            //GUILayout.Label("Reference Path");
            //GUILayout.TextField(obj.ref_path);

            //obj.overwrite_ref = GUILayout.Toggle(obj.overwrite_ref, "Overwrite Reference");

            //GUILayout.Label("Destination Path");

            //if (obj.overwrite_ref)
            //{
            //    obj.dst_path = obj.ref_path;
            //    EditorGUILayout.SelectableLabel(obj.dst_path,
            //                              EditorStyles.textField,
            //                              GUILayout.Height(EditorGUIUtility.singleLineHeight));
            //}
            //else
            //{
            //    obj.dst_path = GUILayout.TextField(obj.dst_path);
            //}

            if (GUILayout.Button("BuildHeader"))
            {
                obj.BuildHeader();
            }

        }
    }
}
