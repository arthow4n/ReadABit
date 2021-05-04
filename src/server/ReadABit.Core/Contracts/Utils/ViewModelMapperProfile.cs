using System;
using System.Linq;
using AutoMapper;
using ReadABit.Core.Commands;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Contracts.Utils
{
    public class ViewModelMapperProfile : Profile
    {
        public ViewModelMapperProfile()
        {
            var userId = default(Guid);

            CreateMap<Article, ArticleViewModel>()
                .ForMember(
                    vm => vm.LanguageCode,
                    conf => conf.MapFrom(a => a.ArticleCollection.LanguageCode)
                )
                .ForMember(
                    vm => vm.ReadingProgress,
                    conf => conf.MapFrom(a =>
                        a.ArticleReadingProgress
                            .Where(arp => arp.UserId == userId)
                            .Select(arp => new ArticleViewModel.ArticleReadingProgressViewModel
                            {
                                ConlluTokenPointer = arp.ConlluTokenPointer,
                                ReadRatio = arp.ReadRatio,
                                CreatedAt = arp.CreatedAt,
                                UpdatedAt = arp.UpdatedAt,
                            })
                            .SingleOrDefault()
                    )
                );

            CreateMap<Article, ArticleListItemViewModel>()
                .ForMember(
                    vm => vm.ReadRadio,
                    conf => conf.MapFrom(a =>
                        a.ArticleReadingProgress
                            .Where(arp => arp.UserId == userId)
                            .Select(arp => arp.ReadRatio)
                            .SingleOrDefault())
                );

            CreateMap<ArticleCollection, ArticleCollectionViewModel>();
            CreateMap<ArticleCollection, ArticleCollectionListItemViewModel>();

            CreateMap<WordSelector, Word>()
                .ForMember(
                    w => w.Expression,
                    conf => conf.MapFrom(ws => ws.Expression.Normalize())
                );

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
