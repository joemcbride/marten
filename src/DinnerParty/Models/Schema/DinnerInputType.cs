using GraphQL.Types;

namespace DinnerParty.Models.Schema
{
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
}