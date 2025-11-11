export class PointModel {
  id: number
  name: string
  latitude: number
  longitude: number
  date: Date

  constructor(id: number, name: string, latitude: number, longitude: number, date: Date) {
    this.id = id
    this.name = name
    this.latitude = latitude
    this.longitude = longitude
    this.date = date
  }
}