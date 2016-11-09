using System.Linq;
using GraphQL.Instrumentation;
using GraphQL.Types;

namespace DinnerParty.Models.Schema
{
    public class OperationPerfType : ObjectGraphType<StatsReport>
    {
        public OperationPerfType()
        {
            Field<StringGraphType>("name", resolve: ctx =>
            {
                var record = ctx.Source.PerSignature.Single();
                return record.Key;
            });
            Field(x => x.Start);
            Field(x => x.End);
            Field(x => x.Duration);
            Field<ListGraphType<OperationTypeStatsType>>("types", resolve: ctx =>
            {
                var record = ctx.Source.PerSignature.Single();
                return record.Value.PerType;
            });
        }
    }

    public class OperationTypeStatsType : ObjectGraphType<TypeStat>
    {
        public OperationTypeStatsType()
        {
            Field(x => x.Name);
            Field(x => x.Fields, type: typeof(ListGraphType<OperationFieldStatsType>));
        }
    }

    public class OperationFieldStatsType : ObjectGraphType<FieldStat>
    {
        public OperationFieldStatsType()
        {
            Field(x => x.Name);
            Field(x => x.ReturnType);
            Field(x => x.Latency);
        }
    }

    public class OperationPerfLog
    {
        public int Id { get; set; }
        public StatsReport Report { get; set; }
    }
}