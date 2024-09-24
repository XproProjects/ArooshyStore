using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IDeliveryInfoRepository : IDisposable
    {
        List<DeliveryInfoViewModel> GetDeliveryInfosListAndCount(string whereCondition, string start, string length, string sorting);
        DeliveryInfoViewModel GetDeliveryInfoById(int id);
        StatusMessageViewModel InsertUpdateDeliveryInfo(DeliveryInfoViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteDeliveryInfo(int id, int loggedInUserId);
    }
}
