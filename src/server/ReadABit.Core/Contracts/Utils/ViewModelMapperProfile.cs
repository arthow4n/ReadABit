using AutoMapper;
using ReadABit.Core.Commands;
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

            CreateMap<WordSelector, Word>();

            CreateMap<WordFamiliarity, WordFamiliarityListItemViewModel>()
                .ForMember(
                    vm => vm.Word,
                    conf => conf.MapFrom(wf => new WordSelector
                    {
                        LanguageCode = wf.Word.LanguageCode,
                        Expression = wf.Word.Expression,
                    })
                );
        }
    }
}
