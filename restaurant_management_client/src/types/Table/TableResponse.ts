export interface TableResponse {
  id: number;
  restaurantId: number;
  number: number;
  capacity: number;
  status: string; //Available, Occupied, Reserved
}
