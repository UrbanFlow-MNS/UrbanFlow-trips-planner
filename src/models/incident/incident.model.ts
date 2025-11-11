export class IncidentModel {
  id: number
  name: string
  date: Date
  estimateTime: number
  status: string

  constructor(id: number, name: string, date: Date, estimateTime: number, status: string) {
    this.id = id
    this.name = name
    this.date = date
    this.estimateTime = estimateTime
    this.status = status
  }
}