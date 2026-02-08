import { Inject, Injectable, OnModuleInit } from '@nestjs/common';
import { ClientGrpc } from '@nestjs/microservices';
import { AllCompleteRoute, CompleteRoute } from 'interfaces/proto.interface';
import { map, Observable } from 'rxjs';

interface TripperService {
  findAll(data: {}): Observable<AllCompleteRoute>;
  findById(data: { id: number }): Observable<CompleteRoute>;
}

@Injectable()
export class AppService implements OnModuleInit {
    private tripperService: TripperService;

    constructor(@Inject('TRIP_SERVICE') private readonly client: ClientGrpc) { }

    onModuleInit() {
        this.tripperService = this.client.getService<TripperService>('Tripper');
    }

    getAllTrips() {
        return this.tripperService.findAll({}).pipe(
            map((response: AllCompleteRoute) => response.routes)
        );
    }
}