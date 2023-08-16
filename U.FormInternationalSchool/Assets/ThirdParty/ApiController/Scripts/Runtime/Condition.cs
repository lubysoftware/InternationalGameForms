using System;

namespace API
{
    public enum Condition
    {
        Eq,
        Ne,
        Gt,
        Lt,
        Gte,
        Lte,
        Starts,
        Ends,
        Cont,
        Excl,
        In,
        IsNull,
        NotIn,
        NotNull,
        Between,
        EqL,
        NeL,
        StartsL,
        EndsL,
        ContL,
        ExclL,
        InL,
        NotInL
    }

    internal static class OperatorExtensions
    {
        internal static string ToFriendlyString(this Condition op)
        {
            return op switch
            {
                Condition.Eq => "$eq",
                Condition.Ne => "$ne",
                Condition.Gt => "$gt",
                Condition.Lt => "$lt",
                Condition.Gte => "$gte",
                Condition.Lte => "$lte",
                Condition.Starts => "$starts",
                Condition.Ends => "$ends",
                Condition.Cont => "$cont",
                Condition.Excl => "$excl",
                Condition.In => "$in",
                Condition.NotIn => "$notin",
                Condition.IsNull => "$isnull",
                Condition.NotNull => "$notnull",
                Condition.Between => "$between",
                Condition.EqL => "$eql",
                Condition.NeL => "$nel",
                Condition.StartsL => "$startsL",
                Condition.EndsL => "$endsL",
                Condition.ContL => "$contL",
                Condition.ExclL => "$exclL",
                Condition.InL => "$inL",
                Condition.NotInL => "$notinL",
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }
    }
}