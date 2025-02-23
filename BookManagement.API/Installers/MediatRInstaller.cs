using MediatR;

namespace BookManagement.API.Installers
{
    public class MediatRInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(typeof(BookManagement.Application.Books.Queries.GetBookQueryHandler).Assembly);
        }
    }
}
