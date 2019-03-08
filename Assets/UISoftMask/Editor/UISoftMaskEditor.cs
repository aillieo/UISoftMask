using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace AillieoUtils
{
    [CustomEditor(typeof(UISoftMask))]
    public class UISoftMaskEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Separator();

            UISoftMask mask = (UISoftMask)target;
            PlaceButton("Mask All Children", mask.MaskAllChildren);
            PlaceButton("Mask All Managed", mask.MaskAllManaged);
            PlaceButton("Reset All Children", mask.ResetAllChildren);
            PlaceButton("Reset All Managed", mask.ResetAllManaged);
        }

        void PlaceButton(string buttonText, Action action)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(buttonText, GUILayout.MaxWidth(160)))
            {
                action.Invoke();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
