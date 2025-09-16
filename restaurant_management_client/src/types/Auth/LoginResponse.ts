import type { AccessTokenResponse } from "./Token/AccessTokenResponse";
import type { ResfreshTokenResponse } from "./Token/RefreshTokenResponse";
import type { UserResponse } from "./User/UserResponse";
export interface LoginResponse {
  userInfo: UserResponse;
  accessToken: AccessTokenResponse;
  refreshToken: ResfreshTokenResponse;
}
