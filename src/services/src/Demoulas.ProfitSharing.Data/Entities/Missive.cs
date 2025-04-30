using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Demoulas.ProfitSharing.Data.Entities;
public class Missive
{
    public static class Constants 
    {
        public const int VestingIncreasedOnCurrentBalance = 1;
    }

    public int Id { get; set; }
    public required string Message { get; set; }
}
