import { Module } from '@nestjs/common';
import { ConfigModule } from '@nestjs/config';
import { ClientsModule, Transport } from '@nestjs/microservices';
import { join } from 'node:path';
import { AppController } from './app.controller';
import { AppService } from './app.service';

@Module({
    imports: [
        ConfigModule.forRoot({ isGlobal: true }),
        ClientsModule.register([
            {
                name: 'TRIP_SERVICE',
                transport: Transport.GRPC,
                options: {
                    package: 'trip',
                    protoPath: join(__dirname, './proto/trip.proto'),
                    url: `${process.env.TRIP_SERVICE_HOST}:${process.env.TRIP_SERVICE_GRPC_PORT}`,
                },
            }
        ])
    ],
    controllers: [AppController],
    providers: [AppService],
})
export class AppModule { }
