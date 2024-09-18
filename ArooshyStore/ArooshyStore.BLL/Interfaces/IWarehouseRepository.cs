using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IWarehouseRepository : IDisposable
    {
        List<WarehouseViewModel> GetWarehousesListAndCount(string whereCondition, string start, string length, string sorting);
        WarehouseViewModel GetWarehouseById(int id);
        StatusMessageViewModel InsertUpdateWarehouse(WarehouseViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteWarehouse(int id, int loggedInUserId);
    }
}
