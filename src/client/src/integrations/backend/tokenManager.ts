export type OidcTokenSet = {
  accessToken: string;
  accessTokenExpirationDate: Date;
  refreshToken: string;
};

export type OidcTokenSetStringified = {
  accessToken: string;
  accessTokenExpirationDate: string;
  refreshToken: string;
};

class TokenManager {
  #currentToken: OidcTokenSet | null = null;

  #tokenChangeListeners: Set<() => void> = new Set();

  get currentToken() {
    return this.#currentToken;
  }

  set currentToken(value) {
    this.#currentToken = value;
    this.#tokenChangeListeners.forEach((listener) => listener());
  }

  subscribeToTokenChange = (listener: () => void) => {
    this.#tokenChangeListeners.add(listener);
  };

  unsubscribeToTokenChange = (listener: () => void) => {
    this.#tokenChangeListeners.delete(listener);
  };
}

export const tokenManager = new TokenManager();
