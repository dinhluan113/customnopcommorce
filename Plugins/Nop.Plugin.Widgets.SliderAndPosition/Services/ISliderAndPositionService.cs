using Nop.Core;
using Nop.Plugin.Widgets.SliderAndPosition.Domain;

namespace Nop.Plugin.Widgets.SliderAndPosition.Services
{
    /// <summary>
    /// Store pickup point service interface
    /// </summary>
    public partial interface ISliderAndPositionService
    {
        /// <summary>
        /// Gets all pickup points
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Pickup points</returns>
        IPagedList<SliderAndPositionMd> GetAllSliderAndPositions(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a pickup point
        /// </summary>
        /// <param name="pickupPointId">Pickup point identifier</param>
        /// <returns>Pickup point</returns>
        SliderAndPositionMd GetSliderAndPositionById(int pickupPointId);

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        void InsertSliderAndPosition(SliderAndPositionMd pickupPoint);

        /// <summary>
        /// Updates a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        void UpdateSliderAndPosition(SliderAndPositionMd pickupPoint);

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        void DeleteSliderAndPosition(SliderAndPositionMd pickupPoint);
    }
}
