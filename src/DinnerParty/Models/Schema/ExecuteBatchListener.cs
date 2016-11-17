using System.Threading;
using System.Threading.Tasks;
using DinnerParty.Modules;
using GraphQL.Execution;

namespace DinnerParty.Models.Schema
{
    public class ExecuteBatchListener : DocumentExecutionListenerBase<GraphQLUserContext>
    {
        public override async Task BeforeExecutionAwaitedAsync(
            GraphQLUserContext userContext,
            CancellationToken token)
        {
            await userContext.Batch.Execute(token);
        }
    }
}