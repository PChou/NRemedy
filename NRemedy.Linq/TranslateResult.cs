using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NRemedy.Linq
{
    /// <summary>
    /// translate result which is returned by the QueryVisitor 
    /// which can be access from the API invoke 
    /// </summary>
    public class TranslateResult
    {
        public ConditionResult ConditionResult { get; set; }

        public SelectResult SelectResult { get; set; }

        public NoQueryableResult NoQueryableResult { get; set; }

        public OrderByResult OrderByResult { get; set; }

        //for count/sum/min/max/avg clause
        public bool HasStatictisc { get; set; }
        ////Count || Sum || Min || Max || Average
        public string StatictiscVerb { get; set; }

    }
}
