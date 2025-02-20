using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookManagement.Core.Interfaces;
using BookManagement.Core.Models;

namespace BookManagement.Core.Sevices
{
    public class PopularityScore : IPopularityScore
    {
        private const double alpha = 0.8;

        public double CalcualatePopularityScore(Book book)
        {
            int yearsSincePublished = DateTime.Now.Year - book.PublicationYear;
            double yearScore = Math.Pow(alpha, yearsSincePublished);
            double viewScore = Math.Log(book.ViewsCount);

            double popularity = viewScore * yearScore;

            return popularity;
        }
    }
}
