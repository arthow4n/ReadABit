import Constants from "expo-constants";
import { Backend } from "./types";

// TODO: Make this compatible with production env.
export const backendBaseUrl = `http://${
  (Constants.manifest.debuggerHost ?? "localhost").split(":")[0]
}:5000`;

export const api = new Backend.Client(backendBaseUrl);
