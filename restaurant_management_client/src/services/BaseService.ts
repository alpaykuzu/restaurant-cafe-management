import Cookies from "js-cookie";
import type { ApiResponse } from "../types/ApiResponse";

export class BaseService {
  static async requestWithToken<T>(
    url: string,
    options: RequestInit = {}
  ): Promise<ApiResponse<T>> {
    const accessToken = Cookies.get("accessToken");

    const headers = new Headers({
      ...(options.headers instanceof Headers
        ? Object.fromEntries(options.headers.entries())
        : options.headers),
      Authorization: `Bearer ${accessToken}`,
    });

    const response = await fetch(url, { ...options, headers });

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.message || "Bir hata oluştu");
    }

    return response.json();
  }

  static async requestWithoutToken<T>(
    url: string,
    options: RequestInit = {}
  ): Promise<ApiResponse<T>> {
    const headers = new Headers({
      ...(options.headers instanceof Headers
        ? Object.fromEntries(options.headers.entries())
        : options.headers),
    });

    const response = await fetch(url, { ...options, headers });

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.message || "Bir hata oluştu");
    }

    return response.json();
  }
}
