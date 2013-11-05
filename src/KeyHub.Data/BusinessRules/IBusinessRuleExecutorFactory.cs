using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using KeyHub.Model;

namespace KeyHub.Data.BusinessRules
{
    public interface IBusinessRuleExecutorFactory
    {
        /// <summary>
        /// Creates a new instance of the busineness rules factory
        /// </summary>
        /// <returns></returns>
        IBusinessRuleExecutor Create();
    }
}