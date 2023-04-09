namespace HotelBooking.Persistence.SqlServer
{
    using System;
    using System.Threading.Tasks;

    public class BaseDataManager<TIdT> : IBaseDataManager<TIdT>
    {
        /// <inheritdoc/>
        public Task<bool> ExistsByIdAsync(TIdT id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveById(TIdT id)
        {
            throw new NotImplementedException();
        }
    }
}
