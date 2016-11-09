using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Marten.Linq;

namespace DinnerParty.Models.Marten
{
    public class FindPopularDinners : ICompiledListQuery<Dinner, Dinner>
    {
        public int Limit { get; set; } = 40;

        public Expression<Func<IQueryable<Dinner>, IEnumerable<Dinner>>> QueryIs()
        {
            return q => q.Take(Limit).OrderByDescending(x => x.RSVPCount);
        }
    }
}