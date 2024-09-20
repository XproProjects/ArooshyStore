using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IAboutRepository : IDisposable
    {
        List<AboutViewModel> GetAboutsListAndCount(string whereCondition, string start, string length, string sorting);
        AboutViewModel GetAboutById(int id);
        StatusMessageViewModel InsertUpdateAbout(AboutViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteAbout(int id, int loggedInUserId);
    }
}
