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