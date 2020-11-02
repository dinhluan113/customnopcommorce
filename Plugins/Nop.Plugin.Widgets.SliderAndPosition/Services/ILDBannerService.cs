using Nop.Core;
using Nop.Plugin.Widgets.SliderAndPosition.Domain;

namespace Nop.Plugin.Widgets.SliderAndPosition.Services
{
    /// <summary>
    /// Store pickup point service interface
    /// </summary>
    public partial interface ILDBannerService
    {
        IPagedList<BannersMd> GetAllBannerByPositionId(int posId, int pageIndex = 0, int pageSize = int.MaxValue);
        BannersMd GetBannerById(int pickupPointId);

        void InsertLDBanner(BannersMd pickupPoint);

        void UpdateLDBanner(BannersMd pickupPoint);

        void DeleteLDBanner(BannersMd pickupPoint);
    }
}
