import type { ResfreshTokenResponse } from "./Token/RefreshTokenResponse";
import type { AccessTokenResponse } from "./Token/AccessTokenResponse";

export interface TokenResponse {
  accessToken: AccessTokenResponse;
  refreshToken: ResfreshTokenResponse;
  roles: string[];
}
