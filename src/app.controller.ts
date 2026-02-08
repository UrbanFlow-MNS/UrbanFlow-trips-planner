import { Controller, Get } from '@nestjs/common';
import { CompleteRoute } from 'interfaces/proto.interface';
import { firstValueFrom } from 'rxjs';
import { AppService } from './app.service';

@Controller("trips-planner")
export class AppController {
    constructor(private readonly appService: AppService) { }

    @Get("fetch-trips")
    async fetchTrips(): Promise<CompleteRoute[]> {
        return await firstValueFrom(this.appService.getAllTrips());
    }
}