import React from 'react';

import { Text, View } from 'native-base';

import { Backend } from '@src/integrations/backend/types';
import { wordFamiliarityLevelColorCodeMapping } from '@src/shared/constants/colorCode';

import { useWordTokenHandle } from './ArticleReaderRenderContext';
import { getSpacesAfter, isWord } from './TokenUtils';

const fontSize = 20;

export const RenderToken: React.FC<{
  token: Backend.Token;
  documentIndex: number;
  paragraphIndex: number;
  sentenceIndex: number;
  tokenIndex: number;
  readRatio: number;
  isLastTokenInSentence: boolean;
}> = ({
  token,
  documentIndex,
  paragraphIndex,
  sentenceIndex,
  tokenIndex,
  readRatio,
  isLastTokenInSentence,
}) => {
  const {
    wordFamiliarityItem,
    updateSelectedToken,
    updateReadingProgress,
    ttsSpeak,
  } = useWordTokenHandle(token);

  const spacesAfter = getSpacesAfter(token);

  const renderSpaceAfter = () => (
    <Text style={{ fontSize }}>{spacesAfter}</Text>
  );

  return (
    <Text key={token.id}>
      {/* TODO: Render and highlight multiple words token,
          for example, "på grund av" */}
      {/* TODO: Support selected multiple words */}
      <View
        style={
          wordFamiliarityLevelColorCodeMapping[wordFamiliarityItem.level]
            ? {
                borderBottomWidth: 4,
                borderColor: !isWord(token)
                  ? 'rgba(0,0,0,0)'
                  : wordFamiliarityLevelColorCodeMapping[
                      wordFamiliarityItem.level
                    ],
              }
            : {}
        }
      >
        <Text
          onPress={() => {
            console.log('Pressed in RenderToken: ', token);
            if (!isWord(token)) {
              return;
            }

            updateReadingProgress({
              conlluTokenPointer: {
                documentIndex,
                paragraphIndex,
                sentenceIndex,
                tokenIndex,
              },
              readRatio,
            });

            updateSelectedToken(token);
            ttsSpeak(token.form);
          }}
          style={{
            fontSize,
          }}
        >
          {token.form}
        </Text>
      </View>
      {/* To catch the edge case where tokenizer marks `?` as SpaceAfter */}
      {!isLastTokenInSentence &&
        (spacesAfter.trim() ? (
          <View
            style={{
              borderBottomWidth: 4,
              borderColor: 'rgba(0,0,0,0)',
            }}
          >
            {renderSpaceAfter()}
          </View>
        ) : (
          renderSpaceAfter()
        ))}
    </Text>
  );
};
