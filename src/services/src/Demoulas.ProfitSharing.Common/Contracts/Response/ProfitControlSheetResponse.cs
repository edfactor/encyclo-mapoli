using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response
{
    public sealed record ProfitControlSheetResponse
    {
        public decimal EmployeeContributionProfitSharingAmount { get; set; }
        public decimal NonEmployeeProfitSharingAmount { get; set; }
        public decimal EmployeeBeneficiaryAmount { get; set; }
        public decimal ProfitSharingAmount 
        {   
            get
            {
                return EmployeeContributionProfitSharingAmount + NonEmployeeProfitSharingAmount + EmployeeBeneficiaryAmount;
            } 
        }

        public static ProfitControlSheetResponse ResponseExample()
        {
            return new ProfitControlSheetResponse()
            {
                EmployeeContributionProfitSharingAmount = 21991510m,
                NonEmployeeProfitSharingAmount = 150102m,
                EmployeeBeneficiaryAmount = 12502m
            };
        }
    }
}
