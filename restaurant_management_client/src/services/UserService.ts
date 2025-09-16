import { BaseService } from "./BaseService";
import type { ApiResponse } from "../types/ApiResponse";
import type { UserResponse } from "../types/User/UserResponse";
import type { CreateUserRequest } from "../types/User/CreateUserRequest";

export class UserService {
  private static readonly APIURL = "http://localhost:5164/api";

  static async getUsers(): Promise<ApiResponse<UserResponse[]>> {
    return BaseService.requestWithToken<UserResponse[]>(
      `${this.APIURL}/User/get-all-user`
    );
  }
  static async getUserById(): Promise<ApiResponse<UserResponse>> {
    return BaseService.requestWithToken<UserResponse>(
      `${this.APIURL}/User/get-user-by-id`
    );
  }

  static async getManagers(): Promise<ApiResponse<UserResponse[]>> {
    return BaseService.requestWithToken<UserResponse[]>(
      `${this.APIURL}/User/get-managers`
    );
  }

  static async createUser(
    user: CreateUserRequest
  ): Promise<ApiResponse<UserResponse>> {
    return BaseService.requestWithToken<UserResponse>(
      `${this.APIURL}/Auth/register`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(user),
      }
    );
  }
}
