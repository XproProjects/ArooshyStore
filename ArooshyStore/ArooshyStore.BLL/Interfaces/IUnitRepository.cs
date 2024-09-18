using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IUnitRepository : IDisposable
    {
        List<UnitViewModel> GetUnitsListAndCount(string whereCondition, string start, string length, string sorting);
        UnitViewModel GetUnitById(int id);
        StatusMessageViewModel InsertUpdateUnit(UnitViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteUnit(int id, int loggedInUserId);
    }
}
