﻿using System;
using System.Collections.Generic;
using System.Linq;
using DinnerParty.Models.Schema;
using GraphQL;
using GraphQL.Http;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation;
using Marten;
using Marten.Services.BatchQuerying;
using Nancy;
using Nancy.Extensions;
using Nancy.Validation;
using Newtonsoft.Json;

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
                var start = DateTime.UtcNow;

                var options = JsonConvert.DeserializeObject<GraphQLQuery>(Request.Body.AsString());

                var inputs = new Inputs(options.Variables ?? new Dictionary<string, object>());

                var batch = session.CreateBatchQuery();

                var userContext = new GraphQLUserContext
                {
                    User = Context.CurrentUser as UserIdentity,
                    Batch = batch,
                    Validate = model => this.Validate(model)
                };

                var result = await executer.ExecuteAsync(_ =>
                {
                    _.Query = options.Query;
                    _.Schema = schema;
                    _.Inputs = inputs;
                    _.UserContext = userContext;
                    _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
                    _.Listeners.Add(new ExecuteBatchListener());
                    _.ValidationRules = new[] {new RequiresAuthValidationRule()}.Concat(DocumentValidator.CoreRules());
                });

                LogStats(session, schema, result, start);

                var json = writer.Write(result);
                System.Diagnostics.Debug.WriteLine(json);

                var response = (Response)json;
                response.ContentType = "application/json";
                return response;
            };
        }

        private void LogStats(IDocumentSession session, ISchema schema, ExecutionResult result, DateTime start)
        {
            if (result.Operation != null && !string.Equals(result.Operation.Name, "stats"))
            {
                var report = StatsReport.From(schema, result.Operation, result.Perf, start);

                var history = new OperationPerfLog
                {
                    Report = report
                };

                session.Store(history);
                session.SaveChanges();
            }
        }
    }

    public class GraphiQL : BaseModule
    {
        public GraphiQL() :
            base("_graphql")
        {
            Get["/"] = p => View["GraphQL/GraphiQL"];
        }
    }

    public class GraphQLQuery
    {
        public string Query { get; set; }
        public string OperationName { get; set; }
        public Dictionary<string, object> Variables { get; set; }
    }

    public class GraphQLUserContext
    {
        public UserIdentity User { get; set; }
        public IBatchedQuery Batch { get; set; }

        public Func<object, ModelValidationResult> Validate { get; set; }
    }
}