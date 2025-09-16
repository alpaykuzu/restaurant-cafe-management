export interface UpdateTableRequest {
  id: number;
  number: number;
  capacity: number;
  status: string; //Available, Occupied, Reserved
}
