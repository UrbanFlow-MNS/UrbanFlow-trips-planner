import { NestFactory } from '@nestjs/core';
import { MicroserviceOptions, Transport } from '@nestjs/microservices';
import { AppModule } from './app.module';

async function bootstrap() {
    const app = await NestFactory.create(AppModule);

    if (process.env.ENABLE_RABBITMQ === 'true') {
        app.connectMicroservice<MicroserviceOptions>({
            transport: Transport.RMQ,
            options: {
                urls: [process.env.RABBIT_MQ ?? ''],
                queue: 'TRIPS_PLANNER_QUEUE',
                queueOptions: {
                    durable: false,
                },
            },
        });

        // For gateway
        app.connectMicroservice<MicroserviceOptions>({
            transport: Transport.TCP,
            options: {
                host: '0.0.0.0',
                port: Number(process.env.TCP_PORT) || 0
            },
        });

        await app.startAllMicroservices()
    }

    await app.listen(process.env.API_PORT ?? 0);
}
bootstrap();
