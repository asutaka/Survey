using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Job
{
    [DisallowConcurrentExecution]
    public class UpdateDataJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (SubcribeJob._binanceTicks == null
                || !SubcribeJob._binanceTicks.Any())
                return;

            //throw new NotImplementedException();
        }
    }
}
