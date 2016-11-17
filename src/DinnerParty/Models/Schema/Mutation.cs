using System;
using System.Collections.Generic;
using DinnerParty.Modules;
using GraphQL;
using GraphQL.Types;
using Marten;

namespace DinnerParty.Models.Schema
{
    public class Mutation : ObjectGraphType
    {
        public Mutation(IDocumentSession session)
        {
            Field<DinnerType>(
                "createDinner",
                arguments: new QueryArguments(new QueryArgument<DinnerInputType> { Name = "dinner" }),
                resolve: context =>
                {
                    var userContext = context.UserContext.As<GraphQLUserContext>();

                    var dinner = context.GetArgument<Dinner>("dinner");
                    dinner.HostedBy = context.GetArgument<string>("hostName");

                    var validationResult = userContext.Validate(dinner);

                    if (!validationResult.IsValid)
                    {
                        throw new Exception("Dinner input not valid");
                    }

                    dinner.HostedById = userContext.User.UserName;
                    dinner.HostedBy = string.IsNullOrWhiteSpace(dinner.HostedBy)
                        ? userContext.User.FriendlyName
                        : dinner.HostedBy;

                    var rsvp = new RSVP
                    {
                        AttendeeNameId = userContext.User.UserName,
                        AttendeeName = userContext.User.FriendlyName
                    };

                    dinner.RSVPs = new List<RSVP> { rsvp };

                    session.Store(dinner);
                    session.SaveChanges();

                    return dinner;
                });
        }
    }
}