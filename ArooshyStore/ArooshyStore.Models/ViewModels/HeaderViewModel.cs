using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.Models.ViewModels
{
    public class HeaderViewModel
    {
        public List<CategoryViewModel> MasterCategory { get; set; }
        public List<CategoryViewModel> ChildCategory { get; set; }
    }
}
