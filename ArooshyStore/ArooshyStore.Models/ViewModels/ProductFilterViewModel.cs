using System;
using System.Collections.Generic;

namespace ArooshyStore.Models.ViewModels
{
    public class ProductFilterViewModel
    {
        public List<AttributeViewModel> AttributesList { get; set; }
        public List<DiscountOfferViewModel> DiscountOffer { get; set; }
        public List<CategoryViewModel> Categories { get; set; }

    }
}
