using System;
using System.Globalization;
using System.Linq;
using AutoMapper;
using ReadABit.Core.Commands;
using ReadABit.Core.Integrations.Contracts.Conllu;
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
                )
                .ForMember(
                    vm => vm.ColluDocumentInternal,
                    conf => conf.MapFrom(a => a.ConlluDocument)
                )
                .ForMember(
                    vm => vm.ConlluDocument,
                    conf => conf.MapFrom(_ => new ConlluDocumentViewModel { })
                );

            CreateMap<Conllu.Document, ConlluDocumentViewModel>();
            CreateMap<Conllu.Paragraph, ConlluParagraphViewModel>();
            CreateMap<Conllu.Sentence, ConlluSentenceViewModel>();
            CreateMap<Conllu.Token, ConlluTokenViewModel>()
                .ForMember(
                    vm => vm.LanguageCode,
                    conf => conf.MapFrom((_, _, _, context) => (string)context.Items["LanguageCode"])
                )
                .ForMember(
                    vm => vm.NormalisedToken,
                    conf => conf.MapFrom((t, _, _, context) => new ConlluNormalisedTokenViewModel
                    {
                        Form =
                            ConlluNormalisedTokenViewModel.NormaliseString(
                                t.Form,
                                CultureInfo.GetCultureInfo((string)context.Items["LanguageCode"])
                            ),
                        Lemma =
                            ConlluNormalisedTokenViewModel.NormaliseString(
                                t.Lemma,
                                CultureInfo.GetCultureInfo((string)context.Items["LanguageCode"])
                            ),
                        LanguageCode = (string)context.Items["LanguageCode"],
                    })
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
