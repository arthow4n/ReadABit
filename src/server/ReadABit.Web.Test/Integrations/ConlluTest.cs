using Newtonsoft.Json;
using ReadABit.Core.Integrations.Contracts.Conllu;
using Shouldly;
using Xunit;

namespace ReadABit.Web.Test.Integrations
{
    public class ConlluTest
    {
        [Fact]
        public void ToConlluDocument_OutputsCorrectResult()
        {
            var input =
@"# newdoc
# newpar
# sent_id = 1
# text = Hallå värld!
1	Hallå	Hallå	ADP	PP	_	2	case	_	_
2	värld	värld	NOUN	NN|UTR|SIN|IND|NOM	Case=Nom|Definite=Ind|Gender=Com|Number=Sing	0	root	_	SpaceAfter=No
3	!	!	PUNCT	MAD	_	2	punct	_	SpacesAfter=\n\n

# newpar
# sent_id = 2
# text = Hur mår du?
1	Hur	hur	ADV	HA	_	2	advmod	_	_
2	mår	må	VERB	VB|PRS|AKT	Mood=Ind|Tense=Pres|VerbForm=Fin|Voice=Act	0	root	_	_
3	du	du	PRON	PN|UTR|SIN|DEF|SUB	Case=Nom|Definite=Def|Gender=Com|Number=Sing	2	nsubj	_	SpaceAfter=No
4	?	?	PUNCT	MAD	_	2	punct	_	SpacesAfter=\n

# sent_id = 3
# text = Jag mår bra, tack!
1	Jag	jag	PRON	PN|UTR|SIN|DEF|SUB	Case=Nom|Definite=Def|Gender=Com|Number=Sing	2	nsubj	_	_
2	mår	må	VERB	VB|PRS|AKT	Mood=Ind|Tense=Pres|VerbForm=Fin|Voice=Act	0	root	_	_
3	bra	bra	ADV	AB|POS	Degree=Pos	2	advmod	_	SpaceAfter=No
4	,	,	PUNCT	MID	_	2	punct	_	_
5	tack	tack	NOUN	NN|NEU|SIN|IND|NOM	Case=Nom|Definite=Ind|Gender=Neut|Number=Sing	2	conj	_	SpaceAfter=No
6	!	!	PUNCT	MAD	_	2	punct	_	SpacesAfter=\n

";

            var document = Conllu.ToConlluDocument(input);

            JsonConvert.SerializeObject(document, Formatting.Indented).ShouldMatchApproved();
        }
    }
}
