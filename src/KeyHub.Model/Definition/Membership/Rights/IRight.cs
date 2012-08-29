using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace KeyHub.Model
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    [InheritedExport]
    public interface IRight
    {
        /// <summary>
        /// 
        /// </summary>
        Guid RightId { get; }
        
        /// <summary>
        /// 
        /// </summary>
        string DisplayName { get; }
    }
}
