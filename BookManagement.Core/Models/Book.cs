using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookManagement.Core.Interfaces;

namespace BookManagement.Core.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Title { get; set; }

        public int PublicationYear { get; set; }

        public required string AuthorName { get; set; }

        public int ViewsCount { get; set; }

        public bool IsDeleted { get; set; }

        public double GetPopularityScore(IPopularityScore popularityScore)
        {
            return popularityScore.CalcualatePopularityScore(this);
        }
    }
}
