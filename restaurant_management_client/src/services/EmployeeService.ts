import type { ApiResponse } from "../types/ApiResponse";
import type { CreateEmployeeRequest } from "../types/Employee/CreateEmployeeRequest";
import type { EmployeeResponse } from "../types/Employee/EmployeeResponse";
import type { UpdateEmployeeRequest } from "../types/Employee/UpdateEmployeeRequest";
import { BaseService } from "./BaseService";

export class EmployeeService {
  private static readonly APIURL = "http://localhost:5164/api";

  static async getEmployeesAll(): Promise<ApiResponse<EmployeeResponse[]>> {
    return BaseService.requestWithToken<EmployeeResponse[]>(
      `${this.APIURL}/Employee/get-all-employees`
    );
  }

  static async getEmployeesByRestaurantId(
    restaurantId: number
  ): Promise<ApiResponse<EmployeeResponse[]>> {
    return BaseService.requestWithToken<EmployeeResponse[]>(
      `${this.APIURL}/Employee/get-employees-by-restaurant-id/${restaurantId}`
    );
  }
  static async getEmployeesOwnRestaurant(): Promise<
    ApiResponse<EmployeeResponse[]>
  > {
    return BaseService.requestWithToken<EmployeeResponse[]>(
      `${this.APIURL}/Employee/get-employees-own-restaurant`
    );
  }

  static async createEmployee(
    employee: CreateEmployeeRequest
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Employee/create-employee`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(employee),
      }
    );
  }

  static async updateEmployee(
    employee: UpdateEmployeeRequest
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Employee/update-employee`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(employee),
      }
    );
  }

  static async deleteEmployee(id: number): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Employee/delete-employee/${id}`,
      {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      }
    );
  }
}
