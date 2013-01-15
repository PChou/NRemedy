using System.Collections.Generic;

namespace NRemedy
{
    public class ARSelectionType
    {
        public List<SelectionItem> item { get; set; }
    }
    public class SelectionItem
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
