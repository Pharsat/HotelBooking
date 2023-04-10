namespace HotelBooking.Persistence
{
    using System.Threading.Tasks;

    /// <summary>
    /// The base data manager, contains base implementations 
    /// </summary>
    /// <typeparam name="TIdT">The Id data type.</typeparam>
    public interface IBaseDataManager<in TIdT>
    {
        /// <summary>
        /// Checks if entity exists by its Id.
        /// </summary>
        /// <param name="id">The entity Id</param>
        /// <param name="entityName">The entity name.</param>
        /// <returns>A flag that indicates whether a value exists by its id.</returns>
        Task<bool> ExistsByIdAsync(TIdT id, string entityName);

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <param name="entityName"></param>
        Task RemoveById(TIdT id, string entityName);
    }
}
