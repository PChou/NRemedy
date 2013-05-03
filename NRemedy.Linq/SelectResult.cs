using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NRemedy.Linq
{
    public class SelectResult
    {
        //for store select property name
        public List<String> SelectedProperties { get; set; }

        //select expression for build result T
        public LambdaExpression SelectExpression { get; set; }

        //select target type,sometimes may anonymous type
        public Type TargetType { get; set; }
    }
}
