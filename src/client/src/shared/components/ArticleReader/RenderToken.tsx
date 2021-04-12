import React from 'react';

import { Text, View } from 'native-base';

import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';
import { wordFamiliarityLevelColorCodeMapping } from '@src/shared/constants/colorCode';

import { useWordTokenHandle } from './ArticleReaderRenderContext';
import { isWord } from './TokenUtils';

export const RenderToken: React.FC<{
  token: Backend.Token;
  articleId: string;
  documentId: string;
  paragraphId: string;
  sentenceId: string;
  readRatio: number;
}> = ({ token, articleId, documentId, paragraphId, sentenceId, readRatio }) => {
  const { wordFamiliarityItem, updateSelectedToken } = useWordTokenHandle(
    token,
  );

  const spacesAfter = (token.misc.match(/\|?Spaces?After=(.+)\|?/)?.[1] ?? ' ')
    .replace(/^No$/, '')
    .replace(/\\s/g, ' ')
    .replace(/\\n/g, '\n');

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
                  documentId,
                  paragraphId,
                  sentenceId,
                  tokenId: token.id,
                },
                readRatio,
              },
            });

            updateSelectedToken(token);
          }}
          style={{
            fontSize: 28,
          }}
        >
          {token.form}
        </Text>
      </View>
      {/* To catch the edge case where tokenizer marks `?` as SpaceAfter */}
      {spacesAfter.trim() ? (
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
      )}
    </Text>
  );
};
