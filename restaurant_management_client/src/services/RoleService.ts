import { BaseService } from "./BaseService";
import type { ApiResponse } from "../types/ApiResponse";
import type { RoleResponse } from "../types/Role/RoleResponse";
import type { CreateRoleRequest } from "../types/Role/CreateRoleRequest";
import type { DeleteRoleRequest } from "../types/Role/DeleteRoleRequest";

export class RoleService {
  private static readonly APIURL = "http://localhost:5164/api";

  static async createRole(
    role: CreateRoleRequest
  ): Promise<ApiResponse<RoleResponse>> {
    return BaseService.requestWithToken<RoleResponse>(
      `${this.APIURL}/Role/create-role`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(role),
      }
    );
  }

  static async deleteRole(role: DeleteRoleRequest): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Role/delete-role`,
      {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(role),
      }
    );
  }
}
