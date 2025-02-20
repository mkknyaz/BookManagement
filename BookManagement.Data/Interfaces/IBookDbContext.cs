using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Data.Interfaces
{
    internal interface IBookDbContext
    {
        DbSet<Book> Books { get; set; }

        Task<int> SaveAsync();
    }
}
