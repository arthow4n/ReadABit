using AutoMapper;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Web.Contracts.Utils
{
    public class ViewModelMapperProfile : Profile
    {
        public ViewModelMapperProfile()
        {
            CreateMap<Article, ArticleViewModel>();
        }
    }
}
