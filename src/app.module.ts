import { Module } from '@nestjs/common';
import { ConfigModule } from '@nestjs/config';
import { ClientsModule, Transport } from '@nestjs/microservices';
import { AppController } from './app.controller';
import { AppService } from './app.service';

@Module({
    imports: [
        ConfigModule.forRoot({ isGlobal: true }),
        ClientsModule.register([
            {
                name: 'TRIP_SERVICE',
                transport: Transport.TCP,
                options: {
                    host: process.env.TRIP_SERVICE_HOST || '',
                    port: Number.parseInt(process.env.TRIP_SERVICE_TCP_PORT || ''),
                },
            }
        ])
    ],
    controllers: [AppController],
    providers: [AppService],
})
export class AppModule { }
