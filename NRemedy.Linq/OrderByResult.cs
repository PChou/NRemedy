using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy.Linq
{
    public class OrderByResult
    {
        //the sortlist order will be same as the declare order
        public List<SortInfo> OrderByList { get; set; }
    }

    public class SortInfo
    {
        public string Property { get; set; }
        //OrderByDescending || OrderBy
        public string Method { get; set; }
    }
}
