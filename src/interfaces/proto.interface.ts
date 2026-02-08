import { Observable } from "rxjs";

export interface TripData {
    id: number;
    destination: string;
}

export interface TripsList {
    trips: TripData[];
}

export interface TripService {
    findAll(data: {}): Observable<TripsList>;
    findOne(data: { id: number }): Observable<TripData>;
}

export interface AllCompleteRoute {
  routes: CompleteRoute[];
}

export interface CompleteRoute {
  route_id: number;
  route_short_name: string;
  route_long_name: string;
  route_type_name: string;
  trips: any[];
}