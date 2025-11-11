import { IncidentModel } from "models/incident/incident.model"
import { PointModel } from "models/point/point.model"
import { TransportModel } from "models/transport/transport.model"

export class TripModel {
  id: number
  name: string
  points: PointModel[]
  incidents: IncidentModel[]
  transport?: TransportModel

  constructor(id: number, name: string, points: PointModel[], incidents: IncidentModel[], transport?: TransportModel) {
    this.id = id
    this.name = name
    this.points = points
    this.incidents = incidents
    this.transport = transport
  }

  get startDate(): Date | undefined {
    return this.points[0]?.date
  }

  get formattedStartDate(): string | undefined {
    if (!this.startDate) return undefined
    return this.startDate.toISOString()
  }

}