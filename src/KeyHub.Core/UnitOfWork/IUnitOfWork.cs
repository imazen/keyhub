using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Core.UnitOfWork
{
    /// <summary>
    /// Represents a Unit of Work where specific logic can be grouped into one operation/class.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity this unit returns</typeparam>
    /// <remarks>
    ///  Units of work are one goal specific classes where logic can be applied to data.
    ///  Examples:
    /// 
    ///      - Applying a definition onto a File or object
    ///      - Adding a scene from a file
    /// </remarks>
    public interface IUnitOfWork<out TEntity> : IDisposable
    {
        /// <summary>
        /// Commits all the changes made to this unit
        /// </summary>
        TEntity Commit();
    }
}
