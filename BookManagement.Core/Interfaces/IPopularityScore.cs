using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookManagement.Core.Models;

namespace BookManagement.Core.Interfaces
{
    public interface IPopularityScore
    {
        double CalcualatePopularityScore(Book book);
    }
}
