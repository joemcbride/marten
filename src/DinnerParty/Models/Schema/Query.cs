using System.Linq;
using DinnerParty.Models.Marten;
using DinnerParty.Modules;
using GraphQL;
using GraphQL.Types;
using Marten;

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
                    var userContext = context.UserContext.As<GraphQLUserContext>();
                    var id = context.GetArgument<int>("id");

                    return userContext.Batch.Load<Dinner>(id);
                });

            Field<ListGraphType<DinnerType>>(
                "popularDinners",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "limit", DefaultValue = 40 }),
                resolve: context =>
                {
                    var userContext = context.UserContext.As<GraphQLUserContext>();
                    var limit = context.GetArgument<int>("limit");

                    return userContext.Batch.Query(new FindPopularDinners { Limit = limit });
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