import { Inject, Injectable } from '@nestjs/common';
import { ClientProxy } from '@nestjs/microservices';
import { firstValueFrom } from 'rxjs';

@Injectable()
export class AppService {

    constructor(
        @Inject('TRIP_SERVICE') private readonly client: ClientProxy
    ) { }

    async fetchAllTrips() {
        return await firstValueFrom(
            this.client.send({ cmd: 'trips-planner.requestTrips'}, { })
        )
    }
    
}
