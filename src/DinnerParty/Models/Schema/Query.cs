using System.Linq;
using DinnerParty.Models.Marten;
using GraphQL;
using GraphQL.Types;
using Marten;
using Marten.Services.BatchQuerying;

namespace DinnerParty.Models.Schema
{
    public class Query : ObjectGraphType
    {
        public Query(IDocumentSession session)
        {
            Field<DinnerType>(
                "dinner",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" }),
                resolve: context =>
                {
                    var batch = context.UserContext.As<IBatchedQuery>();
                    var id = context.GetArgument<int>("id");
                    return batch.Load<Dinner>(id);
                });

            Field<ListGraphType<DinnerType>>(
                "popularDinners",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "limit", DefaultValue = 40 }),
                resolve: context =>
                {
                    var batch = context.UserContext.As<IBatchedQuery>();
                    var limit = context.GetArgument<int>("limit");

                    return batch.Query(new FindPopularDinners { Limit = limit });
                });

            Field<ListGraphType<OperationPerfType>>(
                "stats",
                resolve: context =>
                {
                    var logs = session
                        .Query<OperationPerfLog>()
                        .OrderByDescending(x => x.Report.Start)
                        .ToList()
                        .Select(x => x.Report)
                        .ToList();
                    return logs;
                });
        }
    }
}