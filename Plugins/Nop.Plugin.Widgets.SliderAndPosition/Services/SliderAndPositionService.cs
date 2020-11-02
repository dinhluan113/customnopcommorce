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
    public partial class SliderAndPositionService : ISliderAndPositionService
    {
        #region Constants

        private const string PICKUP_POINT_ALL_KEY = "Nop.SliderAndPosition.all-{0}-{1}";
        private const string PICKUP_POINT_PATTERN_KEY = "Nop.SliderAndPosition.";
       
        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<SliderAndPositionMd> _SliderAndPositionRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="SliderAndPositionRepository">Store pickup point repository</param>
        public SliderAndPositionService(ICacheManager cacheManager,
            IRepository<SliderAndPositionMd> SliderAndPositionRepository)
        {
            this._cacheManager = cacheManager;
            this._SliderAndPositionRepository = SliderAndPositionRepository;
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
        public virtual IPagedList<SliderAndPositionMd> GetAllSliderAndPositions(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(PICKUP_POINT_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = _SliderAndPositionRepository.Table;
                query = query.OrderBy(point => point.Name);

                return new PagedList<SliderAndPositionMd>(query, pageIndex, pageSize);
            });
        }

        /// <summary>
        /// Gets a pickup point
        /// </summary>
        /// <param name="sliderId"></param>
        /// <returns>SliderAndPositionMd</returns>
        public virtual SliderAndPositionMd GetSliderAndPositionById(int sliderId)
        {
            if (sliderId == 0)
                return null;

           return _SliderAndPositionRepository.GetById(sliderId);
        }

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        public virtual void InsertSliderAndPosition(SliderAndPositionMd model)
        {
            if (model == null)
                throw new ArgumentNullException("SliderAndPositionMd");

            _SliderAndPositionRepository.Insert(model);
            _cacheManager.RemoveByPattern(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the pickup point
        /// </summary>
        public virtual void UpdateSliderAndPosition(SliderAndPositionMd model)
        {
            if (model == null)
                throw new ArgumentNullException("SliderAndPositionMd");

            _SliderAndPositionRepository.Update(model);
            _cacheManager.RemoveByPattern(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        public virtual void DeleteSliderAndPosition(SliderAndPositionMd model)
        {
            if (model == null)
                throw new ArgumentNullException("SliderAndPositionMd");

            _SliderAndPositionRepository.Delete(model);
            _cacheManager.RemoveByPattern(PICKUP_POINT_PATTERN_KEY);
        }

        #endregion
    }
}
