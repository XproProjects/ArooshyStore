using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IDeliveryChargesRepository : IDisposable
    {
        List<DeliveryChargesViewModel> GetDeliveryChargessListAndCount(string whereCondition, string start, string length, string sorting);
        DeliveryChargesViewModel GetDeliveryChargesById(int id);
        StatusMessageViewModel InsertUpdateDeliveryCharges(DeliveryChargesViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteDeliveryCharges(int id, int loggedInUserId);
    }
}
