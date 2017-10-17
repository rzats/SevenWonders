using System;

namespace SevenWonders.ViewModels
{
    public class ManagerReportViewModel
    {
        public ManagerReportViewModel(int managerId, string name, string lastName, int amountOfTours, int amountOfRegistedTours, decimal? totalProfit, bool isDeleted)
        {

            ManagerId = managerId;
            Name = name;
            LastName = lastName;
            AmountOfTours = amountOfTours;
            TotalProfit = totalProfit;
            AmountOfRegistedTours = amountOfRegistedTours;
            IsDeleted = isDeleted;
        }

        public int ManagerId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int AmountOfTours { get; set; }
        public int AmountOfRegistedTours { get; set; }
        public decimal? TotalProfit { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool IsDeleted { get; set; }
    }
}