using System.Threading.Tasks;

namespace DDD.Core.Application
{
    public interface IEventStoreRepository<TId, TRoot> 
        where TRoot : AggregateRoot<TId>
    {
        /// <summary>
        /// Creates a new aggregate root object, with a unique id.
        /// </summary>
        /// <returns>the aggregate root object</returns>
        Task<TRoot> CreateAsync();
        
        /// <summary>
        /// Loads the aggregate root object, together with all its events, from the persistent store,
        /// or return null, if such object cannot be found
        /// </summary>
        /// <param name="id">the id of the aggregate root object</param>
        /// <returns>the aggregate root object, or null if none was found</returns>
        Task<TRoot> FindAsync(TId id);

        /// <summary>
        /// Saves the aggregate root object, together with its events, to the persistent store.
        /// It only saves the object if the current 'Version' in the persistent store is identical to
        /// the 'OriginalVersion' of the AggregateRoot. It returns an Exception otherwise.
        /// </summary>
        /// <param name="root">the aggregate root object</param>
        /// <returns></returns>
        Task SaveAsync(TRoot root);
    }
}