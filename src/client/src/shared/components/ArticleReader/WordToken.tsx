import React from 'react';

import { Text, View } from 'native-base';

import { Backend } from '@src/integrations/backend/types';
import { wordFamiliarityLevelColorCodeMapping } from '@src/shared/constants/colorCode';

import { useWordTokenHandle } from './ArticleReaderRenderContext';

export const WordToken: React.FC<{
  token: Backend.Token;
}> = ({ token }) => {
  const { wordFamiliarityItem, updateSelectedToken } = useWordTokenHandle(
    token.form,
  );

  const spacesAfter = (token.misc.match(/\|?Spaces?After=(.+)\|?/)?.[1] ?? ' ')
    .replace(/^No$/, '')
    .replace(/\\s/g, ' ')
    .replace(/\\n/g, '\n');

  const isPunct = token.upos === 'PUNCT';

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
                borderColor: isPunct
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
            if (isPunct) {
              return;
            }

            updateSelectedToken(token);
          }}
          style={{
            fontSize: 28,
          }}
        >
          {token.form}
        </Text>
      </View>
      <Text style={{ fontSize: 28 }}>{spacesAfter}</Text>
    </Text>
  );
};
