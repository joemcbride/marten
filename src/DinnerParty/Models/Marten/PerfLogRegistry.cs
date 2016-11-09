using DinnerParty.Models.Schema;
using Marten;

namespace DinnerParty.Models.Marten
{
    public class PerfLogRegistry : MartenRegistry
    {
        public PerfLogRegistry()
        {
            For<OperationPerfLog>().GinIndexJsonData();
            For<OperationPerfLog>().Duplicate(x => x.Report.Duration);
            For<OperationPerfLog>().Duplicate(x => x.Report.Start);
        }
    }
}