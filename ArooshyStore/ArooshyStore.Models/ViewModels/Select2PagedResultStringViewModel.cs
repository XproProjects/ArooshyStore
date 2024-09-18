using System.Collections.Generic;

namespace ArooshyStore.Models.ViewModels
{
    public class Select2PagedResultStringViewModel
    {
        public int Total { get; set; }

        public List<SelectListStringViewModel> Results { get; set; }
    }
}