using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IDiscountOfferRepository : IDisposable
    {
        List<DiscountOfferViewModel> GetDiscountOffersListAndCount(string whereCondition, string start, string length, string sorting);
        DiscountOfferViewModel GetDiscountOfferById(int id);
        StatusMessageViewModel InsertUpdateDiscountOffer(DiscountOfferViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteDiscountOffer(int id, int loggedInUserId);
    }
}
