using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface ICarouselRepository : IDisposable
    {
        List<CarouselViewModel> GetCarouselsListAndCount(string whereCondition, string start, string length, string sorting);
        CarouselViewModel GetCarouselById(int id);
        StatusMessageViewModel InsertUpdateCarousel(CarouselViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteCarousel(int id, int loggedInUserId);
        List<CarouselViewModel> GetAllCarousels();
    }
}
