using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IAttributeRepository : IDisposable
    {
        List<AttributeViewModel> GetAttributesListAndCount(string whereCondition, string start, string length, string sorting);
        AttributeViewModel GetAttributeById(int id);
        StatusMessageViewModel InsertUpdateAttribute(AttributeViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteAttribute(int id, int loggedInUserId);

        AttributeViewModel GetAttributeDetailById(int id,int attributeId);
        StatusMessageViewModel DeleteAttributeDetail(int id, int loggedInUserId);

        StatusMessageViewModel InsertUpdateAttributeDetail(AttributeViewModel model, int loggedInUserId);
        List<AttributeViewModel> GetAttributeDetails(int attributeId);

    }
}
