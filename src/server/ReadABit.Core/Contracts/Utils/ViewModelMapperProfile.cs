using AutoMapper;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Contracts.Utils
{
    public class ViewModelMapperProfile : Profile
    {
        public ViewModelMapperProfile()
        {
            CreateMap<Article, ArticleViewModel>();
            CreateMap<Article, ArticleListItemViewModel>();
            CreateMap<ArticleCollection, ArticleCollectionViewModel>();
        }
    }
}
