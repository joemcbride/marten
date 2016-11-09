using System.Threading;
using System.Threading.Tasks;
using GraphQL.Execution;
using Marten.Services.BatchQuerying;

namespace DinnerParty.Models.Schema
{
    public class ExecuteBatchListener : DocumentExecutionListenerBase<IBatchedQuery>
    {
        public override async Task BeforeExecutionAwaitedAsync(IBatchedQuery userContext, CancellationToken token)
        {
            await userContext.Execute(token);
        }
    }
}