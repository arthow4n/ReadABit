import React from 'react';

import { Text, View } from 'native-base';

import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';
import { wordFamiliarityLevelColorCodeMapping } from '@src/shared/constants/colorCode';

import { useWordTokenHandle } from './ArticleReaderRenderContext';
import { getSpacesAfter, isWord } from './TokenUtils';

export const RenderToken: React.FC<{
  token: Backend.Token;
  articleId: string;
  documentIndex: number;
  paragraphIndex: number;
  sentenceIndex: number;
  tokenIndex: number;
  readRatio: number;
  isLastTokenInSentence: boolean;
}> = ({
  token,
  articleId,
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
    ttsSpeak,
  } = useWordTokenHandle(token);

  const spacesAfter = getSpacesAfter(token);

  const renderSpaceAfter = () => (
    <Text style={{ fontSize: 28 }}>{spacesAfter}</Text>
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

            api().articles_UpsertReadingProgress({
              id: articleId,
              request: {
                conlluTokenPointer: {
                  documentIndex,
                  paragraphIndex,
                  sentenceIndex,
                  tokenIndex,
                },
                readRatio,
              },
            });

            updateSelectedToken(token);
            ttsSpeak(token.form);
          }}
          style={{
            fontSize: 28,
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
