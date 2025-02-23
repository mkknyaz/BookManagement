using BookManagement.API.DTOs;
using BookManagement.Application.Models;
using Mapster;
using System.Reflection;

namespace BookManagement.API.MappingProfiles
{
    public class MapsterConfiguration
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<BookReadModel, BookDetailsDTO>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.PublicationYear, src => src.PublicationYear)
                .Map(dest => dest.AuthorName, src => src.AuthorName)
                .Map(dest => dest.ViewsCount, src => src.ViewsCount)
                .Map(dest => dest.PopularityScore, src => src.PopularityScore);
        }
    }
}
