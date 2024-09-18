using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.BusinessInfo
{
    public sealed class BusinessInfo
    {
        //private string documentSiteName = ConfigurationManager.AppSettings["DocumentSiteName"].ToString();
        private static BusinessInfo instance = null;
        private static readonly object lockThread = new object();
        public static BusinessInfo GetInstance
        {
            get
            {
                //It will check if instance is null then will go further
                if (instance == null)
                {
                    //It will lock all other threads if they are accessing this class at the same time to prevent multiple instances in different threads
                    lock (lockThread)
                    {
                        //It will check if instance is null then create an instance
                        if (instance == null)
                        {
                            instance = new BusinessInfo();
                        }
                    }
                }
                return instance;
            }
        }
        public List<string> GetBusinessInfo()
        {
            List<string> list = new List<string>();
            list.Add("Arooshy Store");
            list.Add("/Areas/Admin/Content/img/Logo.jpg");
            //list.Add("/Content/img/card-backgrounds/cover-1-lg.png");
            list.Add("/Areas/Admin/Content/img/Banner4.jpg");
            list.Add("#584677");
            return list;
        }
        public StatusMessageViewModel UserLoggedOut()
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            response.Status = false;
            response.Message = "You have been logged out. Please login again.";
            response.Id = 0;
            return response;
        }
    }
}
