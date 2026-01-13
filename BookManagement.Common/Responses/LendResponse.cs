using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement.Common.Responses
{
    public class LendResponse
    {
        public string NameOfTheBook { get; set; } = string.Empty;
        public int NewAmount { get; set; }
    }
}
