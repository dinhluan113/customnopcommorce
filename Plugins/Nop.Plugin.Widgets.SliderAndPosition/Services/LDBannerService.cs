using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Widgets.SliderAndPosition.Domain;

namespace Nop.Plugin.Widgets.SliderAndPosition.Services
{
    /// <summary>
    /// Store pickup point service
    /// </summary>
    public partial class LDBannerService : ILDBannerService
    {
        #region Constants

        private const string PICKUP_POINT_ALL_KEY = "Nop.LDBanner.all-{0}-{1}";
        private const string PICKUP_POINT_PATTERN_KEY = "Nop.LDBanner.";
       
        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<BannersMd> _LDBannerRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="LDBannerRepository">Store pickup point repository</param>
        public LDBannerService(ICacheManager cacheManager,
            IRepository<BannersMd> LDBannerRepository)
        {
            this._cacheManager = cacheManager;
            this._LDBannerRepository = LDBannerRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all pickup points
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Pickup points</returns>
        public virtual IPagedList<BannersMd> GetAllBannerByPositionId(int positionId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(PICKUP_POINT_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = _LDBannerRepository.Table;
                query = query.Where(c => c.PositionId == positionId).OrderBy(point => point.DisplayOrder);
                if (query != null && query.Any())
                    return new PagedList<BannersMd>(query, pageIndex, pageSize);
                else
                    return null;
            });
        }

        public virtual BannersMd GetBannerById(int sliderId)
        {
            if (sliderId == 0)
                return null;

            return _LDBannerRepository.GetById(sliderId);
        }

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        public virtual void InsertLDBanner(BannersMd model)
        {
            if (model == null)
                throw new ArgumentNullException("BannersMd");

            _LDBannerRepository.Insert(model);
            _cacheManager.RemoveByPattern(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the pickup point
        /// </summary>
        public virtual void UpdateLDBanner(BannersMd model)
        {
            if (model == null)
                throw new ArgumentNullException("BannersMd");

            _LDBannerRepository.Update(model);
            _cacheManager.RemoveByPattern(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        public virtual void DeleteLDBanner(BannersMd model)
        {
            if (model == null)
                throw new ArgumentNullException("BannersMd");

            _LDBannerRepository.Delete(model);
            _cacheManager.RemoveByPattern(PICKUP_POINT_PATTERN_KEY);
        }
        #endregion
    }
}
