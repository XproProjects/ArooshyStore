using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IDesignationRepository : IDisposable
    {
        List<DesignationViewModel> GetDesignationsListAndCount(string whereCondition, string start, string length, string sorting);
        DesignationViewModel GetDesignationById(int id);
        StatusMessageViewModel InsertUpdateDesignation(DesignationViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteDesignation(int id, int loggedInUserId);
    }
}
