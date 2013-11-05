using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Defines the different objects types for rights distribution across tables.
    /// Also defines the order of inheritence between rights.
    /// </summary>
    public enum ObjectTypes
    {
        /// <summary>
        /// Vendors can sell SKU and have higher rights on all aspects of the application
        /// </summary>
        [Description("Vendor")] 
        Vendor = 0,

        /// <summary>
        /// Customers are the basic entities of KeyHub. They can either be resellers or owners.
        /// Rights defined on Customer level will propogate to the License level
        /// </summary>
        [Description("Customer")] 
        Customer = 1,

        /// <summary>
        /// Lowest level of right assignment. Will only be available for this License entry
        /// </summary>
        [Description("Licenses")] 
        License = 2
    }
}