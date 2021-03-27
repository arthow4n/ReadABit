using AutoMapper;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Contracts.Utils
{
    public class ViewModelMapperProfile : Profile
    {
        public ViewModelMapperProfile()
        {
            CreateMap<Article, ArticleViewModel>()
                .ForMember(
                    vm => vm.LanguageCode,
                    conf => conf.MapFrom(a => a.ArticleCollection.LanguageCode)
                );

            CreateMap<Article, ArticleListItemViewModel>();
            CreateMap<ArticleCollection, ArticleCollectionViewModel>();

            CreateMap<WordFamiliarity, WordFamiliarityListItemViewModel>()
                .ForMember(
                    vm => vm.WordLanguageCode,
                    conf => conf.MapFrom(wf => wf.Word.LanguageCode)
                )
                .ForMember(
                    vm => vm.WordExpression,
                    conf => conf.MapFrom(wf => wf.Word.Expression)
                );
        }
    }
}
