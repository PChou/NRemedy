using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;

namespace NRemedy
{
    public class ARSelectionTypeConvert
    {
        public static ARSelectionType GetSelection(ArrayList limitValues)
        {
            if (limitValues == null) throw new ArgumentNullException("limitValues");
            ARSelectionType selection = new ARSelectionType();
            Type type = limitValues[0].GetType();
            if (type.FullName == "System.Object[]")
            {
                List<SelectionItem> item = new List<SelectionItem>();
                for (int i = 0; i < limitValues.Count; i++)
                {
                    Object[] objects = (Object[])limitValues[i];
                    SelectionItem selectionitem = new SelectionItem();
                    selectionitem.key = objects[1].ToString();
                    string value = Regex.Replace(objects[0].ToString(), @"[^A-Za-z0-9_]", "");
                    if (Regex.IsMatch(value, @"^\d+"))
                    {
                        value = "_" + value;
                    }
                    selectionitem.value = value;//objects[0].ToString();
                    item.Add(selectionitem);
                }
                selection.item = item;
            }
            else if (type.FullName == "System.String")
            {
                List<SelectionItem> item = new List<SelectionItem>();
                for (int i = 0; i < limitValues.Count; i++)
                {
                    String str = limitValues[i].ToString();
                    SelectionItem selectionitem = new SelectionItem();
                    selectionitem.key = i.ToString();
                    string value = Regex.Replace(str, @"[^A-Za-z0-9_]", "");
                    if (Regex.IsMatch(value, @"^\d+"))
                    {
                        value = "_" + value;
                    }
                    selectionitem.value = value;//str;
                    item.Add(selectionitem);
                }
                selection.item = item;
            }
            return selection;
        }
    }
}
