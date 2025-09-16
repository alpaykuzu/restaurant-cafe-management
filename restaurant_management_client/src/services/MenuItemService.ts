import type { ApiResponse } from "../types/ApiResponse";
import type { MenuItemResponse } from "../types/MenuItem/MenuItemResponse";
import { BaseService } from "./BaseService";
import type { CreateMenuItemRequest } from "../types/MenuItem/CreateMenuItemRequest";
import type { UpdateMenuItemRequest } from "../types/MenuItem/UpdateMenuItemRequest";

export class MenuItemService {
  private static readonly APIURL = "http://localhost:5164/api";

  static async getMenuItemsByRestaurantId(): Promise<
    ApiResponse<MenuItemResponse[]>
  > {
    return BaseService.requestWithToken<MenuItemResponse[]>(
      `${this.APIURL}/MenuItem/get-all-menu-items-by-restaurant-id`
    );
  }

  static async getmenuItemsByRestaurantIdAndCategoryId(
    categoryId: number
  ): Promise<ApiResponse<MenuItemResponse[]>> {
    return BaseService.requestWithToken<MenuItemResponse[]>(
      `${this.APIURL}/MenuItem/get-menu-items-by-category-id/${categoryId}`
    );
  }

  static async getMenuItemById(
    id: number
  ): Promise<ApiResponse<MenuItemResponse>> {
    return BaseService.requestWithToken<MenuItemResponse>(
      `${this.APIURL}/MenuItem/get-menu-item-by-id/${id}`
    );
  }

  static async createMenuItem(
    menuItem: CreateMenuItemRequest
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/MenuItem/create-menu-item`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(menuItem),
      }
    );
  }

  static async updateMenuItem(
    menuItem: UpdateMenuItemRequest
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/MenuItem/update-menu-item`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(menuItem),
      }
    );
  }

  static async updateMenuItemPrice(
    id: number,
    newPrice: number
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/MenuItem/update-price/${id}/${newPrice}`
    );
  }

  static async deleteMenuItem(id: number): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/MenuItem/delete-menu-item/${id}`,
      {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      }
    );
  }
}
