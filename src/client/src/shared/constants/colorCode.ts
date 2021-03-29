/**
 * Key is `WordFamiliarity.Level`.
 */
export const wordFamiliarityLevelColorCodeMapping: Record<string, string> = {
  // No colour code for -1 because it's treated as ignored.
  0: '#E8AEE1', // red
  1: '#FFD4B0', // orange
  2: '#A7D9C0', // green
  // No colour code for 3 because it's the max level, in other words "known word".

  // yellow '#EBE9A2',
  // blue '#C2D1FF',
};
