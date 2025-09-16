import type { ApiResponse } from "../types/ApiResponse";
import type { LoginRequest } from "../types/Auth/LoginRequest";
import type { LoginResponse } from "../types/Auth/LoginResponse";
import type { RefreshTokenRequest } from "../types/Token/ResfreshTokenRequest";
import type { TokenResponse } from "../types/Token/TokenResponse";
import type { UserResponse } from "../types/User/UserResponse";
import { BaseService } from "./BaseService";

export class AuthService {
  private static readonly APIURL = "http://localhost:5164/api";

  static async login(data: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    const response = await BaseService.requestWithoutToken<LoginResponse>(
      this.APIURL + "/Auth/login",
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
      }
    );

    if (!response.success) {
      throw new Error(response.message || "Giriş başarısız");
    }

    return response;
  }

  static async me(): Promise<ApiResponse<UserResponse>> {
    const response = await BaseService.requestWithToken<UserResponse>(
      this.APIURL + "/Auth/me"
    );
    return response;
  }

  static async refreshToken(
    data: RefreshTokenRequest
  ): Promise<ApiResponse<TokenResponse>> {
    const response = await BaseService.requestWithoutToken<TokenResponse>(
      this.APIURL + "/Auth/refresh-token",
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
      }
    );

    if (!response.success) {
      throw new Error(response.message || "Token yenileme başarısız");
    }

    return response;
  }
}
