import type { ApiResponse } from "../types/ApiResponse";
import { BaseService } from "./BaseService";
import type { CategoryResponse } from "../types/Category/CategoryResponse";
import type { UpdateCategoryRequest } from "../types/Category/UpdateCategoryRequest";
import type { CreateCategoryRequest } from "../types/Category/CreateCategoryRequest";

export class CategoryService {
  private static readonly APIURL = "http://localhost:5164/api";

  static async getCategoryByRestaurantId(): Promise<
    ApiResponse<CategoryResponse[]>
  > {
    return BaseService.requestWithToken<CategoryResponse[]>(
      `${this.APIURL}/Category/get-all-category-by-restaurant-id`
    );
  }

  static async getCategoryById(
    categoryId: number
  ): Promise<ApiResponse<CategoryResponse>> {
    return BaseService.requestWithToken<CategoryResponse>(
      `${this.APIURL}/Category/get-category-by-id/${categoryId}`
    );
  }

  static async createCategory(
    category: CreateCategoryRequest
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Category/create-category`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(category),
      }
    );
  }

  static async updateCategory(
    category: UpdateCategoryRequest
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Category/update-category`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(category),
      }
    );
  }

  static async deleteCategory(categoryId: number): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Category/delete-category/${categoryId}`,
      {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      }
    );
  }
}
