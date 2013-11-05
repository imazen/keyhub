using System;
using System.ComponentModel;

namespace KeyHub.Core.Dependency
{
    /// <summary>
    /// Represents a sort order property for MEF importmany collections
    /// </summary>
    public interface IGenericImportOrder
    {
        /// <summary>
        /// Gets the sort Order property
        /// </summary>
        [DefaultValue(Int32.MaxValue)]
        int Order { get; }
    }
}