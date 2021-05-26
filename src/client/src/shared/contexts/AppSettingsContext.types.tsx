import { z } from 'zod';

export const appSettingsSchema = z.object({
  languageCodes: z.object({
    // TODO: Use enum for language codes.
    studying: z.string().nonempty(),
    ui: z.string().nonempty(),
  }),
  /**
   * If true, mobile data will be used as well for downloading larger content.
   */
  saveDataUsage: z.boolean(),
  tts: z.object({
    autoSpeakWhenTapOnWord: z.boolean(),
  }),
});

export type AppSettings = z.infer<typeof appSettingsSchema>;
