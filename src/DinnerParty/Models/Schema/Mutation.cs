using System;
using System.Collections.Generic;
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
}