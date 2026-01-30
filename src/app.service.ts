import { Inject, Injectable, OnModuleInit } from '@nestjs/common';
import { ClientGrpc } from '@nestjs/microservices';
import { TripService, TripsList } from 'interfaces/proto.interface';
import { map } from 'rxjs';

@Injectable()
export class AppService implements OnModuleInit {
    private tripService: TripService

    constructor(@Inject('TRIP_SERVICE') private readonly client: ClientGrpc) { }

    onModuleInit() {
        this.tripService = this.client.getService<TripService>('TripService');
    }

    getAllTrips() {
        return this.tripService.findAll({}).pipe(
            map((response: TripsList) => response.trips)
        );
    }
}