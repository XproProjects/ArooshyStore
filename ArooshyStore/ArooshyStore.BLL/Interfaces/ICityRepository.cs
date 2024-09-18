using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface ICityRepository : IDisposable
    {
        List<CityViewModel> GetCitysListAndCount(string whereCondition, string start, string length, string sorting);
        CityViewModel GetCityById(int id);
        StatusMessageViewModel InsertUpdateCity(CityViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteCity(int id, int loggedInUserId);
    }
}
