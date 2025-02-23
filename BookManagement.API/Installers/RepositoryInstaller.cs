using BookManagement.Data.Interfaces;
using BookManagement.Data.Repositories;

namespace BookManagement.API.Installers
{
    public class RepositoryInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBookRepository, BookRepository>();
        }
    }
}
