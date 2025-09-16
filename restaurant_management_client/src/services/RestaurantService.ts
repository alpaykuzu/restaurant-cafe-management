import type { ApiResponse } from "../types/ApiResponse";
import type { CreateRestaurantRequest } from "../types/Restaurant/CreateRestaurantRequest";
import type { RestaurantResponse } from "../types/Restaurant/RestaurantResponse";
import type { UpdateRestaurantRequest } from "../types/Restaurant/UpdateRestaurantRequest";
import { BaseService } from "./BaseService";

export class RestaurantService {
  private static readonly APIURL = "http://localhost:5164/api";

  static async getRestaurants(): Promise<ApiResponse<RestaurantResponse[]>> {
    return BaseService.requestWithToken<RestaurantResponse[]>(
      `${this.APIURL}/Restaurant/get-all-restaurants`
    );
  }

  static async getRestaurantByUserId(): Promise<
    ApiResponse<RestaurantResponse>
  > {
    return BaseService.requestWithToken<RestaurantResponse>(
      `${this.APIURL}/Restaurant/get-restaurant-by-user-id`
    );
  }

  static async createRestaurant(
    restaurant: CreateRestaurantRequest
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Restaurant/create-restaurant`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(restaurant),
      }
    );
  }
  static async updateRestaurant(
    restaurant: UpdateRestaurantRequest
  ): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Restaurant/update-restaurant`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(restaurant),
      }
    );
  }
  static async deleteRestaurant(id: number): Promise<ApiResponse<null>> {
    return BaseService.requestWithToken<null>(
      `${this.APIURL}/Restaurant/delete-restaurant/${id}`,
      {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      }
    );
  }
}
