using GraphQL.Types;

namespace DinnerParty.Models.Schema
{
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
}