using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Contact1 { get; set; }
        public string Contact2 { get; set; }
        public string Email { get; set; }
        public string HouseNo { get; set; }
        public string Street { get; set; }
        public string ColonyOrVillageName { get; set; }
        public string PostalCode { get; set; }
        public int? CityId { get; set; }
        public string CityName { get; set; }
        public string Gender { get; set; }
        public int? DesignationId { get; set; }
        public string DesignationName { get; set; }
        public string MaritalStatus { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string SalaryType { get; set; }
        public bool? Status { get; set; }
        public string StatusString { get; set; }
        public string ImagePath { get; set; }
        public int DocumentId { get; set; }
        public int? InfoId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
        public List<InvoiceViewModel> Invoices { get; set; }


    }
}