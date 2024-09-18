using System.Collections.Generic;

namespace ArooshyStore.Models.ViewModels
{
    public class Select2PagedResultViewModel
    {
        public int Total { get; set; }

        public List<SelectListViewModel> Results { get; set; }
    }
}