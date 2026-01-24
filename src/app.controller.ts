import { Controller, Get } from '@nestjs/common';
import { AppService } from './app.service';

@Controller("trips-planner")
export class AppController {
    constructor(private readonly appService: AppService) { }

    @Get("fetchTrips") // TODO: Just for testing
    async fetchTrips() {
        return await this.appService.fetchAllTrips()
    }

}