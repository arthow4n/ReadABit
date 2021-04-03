/**
 * Key is `WordFamiliarity.Level`.
 */
export const wordFamiliarityLevelColorCodeMapping: Record<string, string> = {
  // Transparent is needed because `undefined` border colour will cause the border to collapse
  '-1': 'rgba(0,0,0,0)',
  0: '#E8AEE1', // red
  1: '#FFD4B0', // orange
  2: '#A7D9C0', // green
  3: 'rgba(0,0,0,0)',

  // yellow '#EBE9A2',
  // blue '#C2D1FF',
};
