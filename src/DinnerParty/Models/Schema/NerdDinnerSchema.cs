using System;
using GraphQL.Types;

namespace DinnerParty.Models.Schema
{
    public class NerdDinnerSchema : GraphQL.Types.Schema
    {
        public NerdDinnerSchema(Func<Type, IGraphType> resolve)
            : base(resolve)
        {
            Query = (IObjectGraphType)resolve(typeof(Query));
            Mutation = (IObjectGraphType)resolve(typeof(Mutation));
        }
    }

    public class NerdDinnerAuthenticatedSchema : GraphQL.Types.Schema
    {
        public NerdDinnerAuthenticatedSchema(Func<Type, IGraphType> resolve)
            : base(resolve)
        {
            Mutation = (IObjectGraphType)resolve(typeof(Mutation));
        }
    }
}