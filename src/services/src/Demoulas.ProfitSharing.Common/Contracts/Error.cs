
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Demoulas.ProfitSharing.Common.Contracts
{
    public sealed record Error
    {
        #region Boilerplate
        private Error(int code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Description { get; init; }

        public int Code { get; init; }
        #endregion

        public static Error EmployeeNotFound => new(100, "Employee not found");

        public static implicit operator ProblemDetails(Error error)
        {
            const string typeString = "https://www.shopmarketbasket.com/about-us/contact-us";
            return new ProblemDetails
            {
                Title = error.Code.ToString(),
                Detail = error.Description,
                Status = (int)HttpStatusCode.BadRequest,
                Type = typeString
            };
        }
    }
}
