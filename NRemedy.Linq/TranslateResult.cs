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
    public class TranslateResult : ConditionResult
    {
        public ConditionResult ConditionResult { get; set; }

        public SelectResult SelectResult { get; set; }

        public NoQueryableResult NoQueryableResult { get; set; }

        public OrderByResult OrderByResult { get; set; }







        //indicate if the linq expression contains where clause
        public bool HasWhere { get; set; }
        //build result of qulification for where clause,inherit from base class
        //public StringBuilder Qulification { get; set; }

        //indicate if the linq expression contains select clause
        public bool HasSelect { get; set; }
        //select expression for build result T
        public LambdaExpression SelectExpression { get; set; }
        //for store select property name
        public List<MemberMap> SelectedProperties { get; set; }
        //target type of select
        public Type TargetType { get; set; }

        //indicate if the linq expression contains skip clause
        public bool HasSkip { get; set; }
        //for skip clause store the skip number
        public int? Skip { get; set; }

        //indicate if the linq expression contains skip clause
        public bool HasTake { get; set; }
        //for skip clause store the skip number
        public int? Take { get; set; }

        //indicate if the linq expression contains OrderBy clause
        public bool HasOrderBy { get; set; }
        //for order by clause store the order by list
        //the sortlist order will be same as the declare order
        public List<SortInfo> OrderByList { get; set; }

        //for count/sum/min/max/avg clause
        public bool HasStatictisc { get; set; }
        //Count || Sum || Min || Max || Average
        public string StatictiscVerb { get; set; }
        //statictisc target field
        public MemberMap StatictiscTarget { get; set; }

        //for group by clause
        public bool HasGroupBy { get; set; }
        //group expression
        public LambdaExpression GroupExpression { get; set; }
        //for store group property name
        public List<MemberMap> GroupedProperties { get; set; }
        //group type
        public Type GroupType { get; set; }


        //public LambdaExpression OrderByExpression { get; set; }

        public TranslateResult()
        {
            Qulification = new StringBuilder();
            SelectedProperties = new List<MemberMap>();
            GroupedProperties = new List<MemberMap>();
            OrderByList = new List<SortInfo>();
        }
    }

    public class MemberMap
    {
        public string SourceMemberName { get; set; }
    }

    public class SortInfo
    {
        public string Property { get; set; }
        //OrderByDescending || OrderBy
        public string Method { get; set; }
    }
}
