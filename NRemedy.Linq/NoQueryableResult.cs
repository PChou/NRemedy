using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy.Linq
{
    public class NoQueryableResult
    {

        //indicate if the linq expression contains skip clause
        public bool HasSkip { get; set; }
        //for skip clause store the skip number
        public int? Skip { get; set; }

        //indicate if the linq expression contains skip clause
        public bool HasTake { get; set; }
        public int? Take { get; set; }
    }
}
