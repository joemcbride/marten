using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DinnerParty.Models;
using DinnerParty.Modules;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Http;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation;
using Marten;
using Marten.Linq;
using Marten.Services.BatchQuerying;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using Type = System.Type;
using TS = GraphQL.TaskExtensions;

namespace DinnerParty.Modules
{
    public class GraphQLModule : BaseModule
    {
        public GraphQLModule(
            IDocumentExecuter executer,
            IDocumentWriter writer,
            IDocumentSession session,
            NerdDinnerSchema schema)
            : base("/graphql")
        {
            Post["/", true] = async (p, ct) =>
            {
                var options = JsonConvert.DeserializeObject<GraphQLQuery>(Request.Body.AsString());

                var inputs = new Inputs(options.Variables ?? new Dictionary<string, object>());

                var start = DateTime.UtcNow;

                var batch = session.CreateBatchQuery();

                var result = await executer.ExecuteAsync(_ =>
                {
                    _.Query = options.Query;
                    _.Schema = schema;
                    _.Inputs = inputs;
                    _.UserContext = batch;
                    _.Listeners.Add(new ExecuteBatchListener());
                });

                if (result.Operation != null)
                {
                    var report = StatsReport.From(schema, result.Operation, result.Perf, start);

                    var history = new OperationPerfLog
                    {
                        Report = report
                    };

                    if (!string.Equals(result.Operation.Name, "stats"))
                    {
                        session.Store(history);
                        session.SaveChanges();
                    }
                }

                var json = writer.Write(result);
                System.Diagnostics.Debug.WriteLine(json);

                var response = (Response)json;
                response.ContentType = "application/json";
                return response;
            };

            Get["/create", true] = async (p, c) =>
            {
                var query = @"
mutation {
  createDinner {
    dinnerID,
    title
    description
  }
}
";
                var result = await executer.ExecuteAsync(schema, null, query, null);
                var json = writer.Write(result);
                System.Diagnostics.Debug.WriteLine(json);
                return Response.AsJson(json);
            };

        }
    }

    public class ExecuteBatchListener : DocumentExecutionListenerBase<IBatchedQuery>
    {
        public override async Task BeforeExecutionAwaitedAsync(IBatchedQuery userContext, CancellationToken token)
        {
            await userContext.Execute(token);
        }
    }

    public class GraphiQL : BaseModule
    {
        public GraphiQL() :
            base("_graphql")
        {
            Get["/"] = parameters =>
            {
                return View["GraphQL/GraphiQL"];
            };
        }
    }

    public class GraphQLQuery
    {
        public string Query { get; set; }
        public string OperationName { get; set; }
        public Dictionary<string, object> Variables { get; set; }
    }

    public class PerfLogRegistry : MartenRegistry
    {
        public PerfLogRegistry()
        {
            For<OperationPerfLog>().GinIndexJsonData();
            For<OperationPerfLog>().Duplicate(x => x.Report.Duration);
            For<OperationPerfLog>().Duplicate(x => x.Report.Start);
        }
    }

    public class FindDinnerById : ICompiledQuery<Dinner>
    {
        public int Id { get; set; }

        public Expression<Func<IQueryable<Dinner>, Dinner>> QueryIs()
        {
            return q => q.SingleOrDefault(x => x.Id == Id);
        }
    }

    public class FindPopularDinners : ICompiledListQuery<Dinner, Dinner>
    {
        public int Limit { get; set; } = 40;

        public Expression<Func<IQueryable<Dinner>, IEnumerable<Dinner>>> QueryIs()
        {
            return q => q.Take(Limit).OrderByDescending(x => x.RSVPCount);
        }
    }

    public class NerdDinnerSchema : Schema
    {
        public NerdDinnerSchema(Func<Type, IGraphType> resolve)
            : base(resolve)
        {
            Query = (IObjectGraphType)resolve(typeof(Query));
            Mutation = (IObjectGraphType)resolve(typeof(Mutation));
        }
    }

    public class Query : ObjectGraphType
    {
        public Query(IDocumentSession session)
        {
            Field<DinnerType>(
                "dinner",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id"}),
                resolve: context =>
                {
                    var batch = context.UserContext.As<IBatchedQuery>();
                    var id = context.GetArgument<int>("id");
                    return batch.Load<Dinner>(id);
                });

            Field<ListGraphType<DinnerType>>(
                "popularDinners",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "limit", DefaultValue = 40}),
                resolve: context =>
                {
                    var batch = context.UserContext.As<IBatchedQuery>();
                    var limit = context.GetArgument<int>("limit");

                    return batch.Query(new FindPopularDinners {Limit = limit});
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

    public class Mutation : ObjectGraphType
    {
        public Mutation(IDocumentSession session)
        {
            Field<DinnerType>(
                "createDinner",
                arguments: new QueryArguments(new QueryArgument<DinnerInputType> {Name = "dinner"}),
                resolve: context =>
                {
                    var dinner = context.GetArgument<Dinner>("dinner");

                    if (dinner == null)
                    {
                        dinner = new Dinner();
                        dinner.Title = "A dinner";
                        dinner.Description = "A short description";
                        dinner.Address = "123 street";
                        dinner.ContactPhone = "888-555-5555";
                        dinner.EventDate = DateTime.UtcNow;
                        dinner.RSVPs = new List<RSVP>();
                    }

                    if (dinner.RSVPs == null)
                    {
                        dinner.RSVPs = new List<RSVP>();
                    }

                    session.Store(dinner);
                    session.SaveChanges();

                    return dinner;
                });
        }
    }

    public class DinnerType : ObjectGraphType<Dinner>
    {
        public DinnerType()
        {
            Field(x => x.Id);
            Field("url", x => x.DinnerID);
            Field(x => x.Title);
            Field(x => x.Description);
            Field(x => x.EventDate);
            Field(x => x.Latitude);
            Field(x => x.Longitude);
            Field("rsvpCount", x => x.RSVPCount);
        }
    }

    public class DinnerInputType : InputObjectGraphType
    {
        public DinnerInputType()
        {
            Field<NonNullGraphType<StringGraphType>>("title");
            Field<NonNullGraphType<StringGraphType>>("description");
            Field<NonNullGraphType<DateGraphType>>("eventDate");
            Field<NonNullGraphType<StringGraphType>>("address");
            Field<NonNullGraphType<StringGraphType>>("contactPhone");
        }
    }

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