using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.BusinessLogic.Basket
{
    /// <summary>
    /// Steps a basket can take during purchase
    /// </summary>
    public enum BasketSteps
    {
        None = 0,
        Create,
        Remind,
        Checkout,
        Complete
    }
}
