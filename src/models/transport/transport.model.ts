export class TransportModel {
  id: number
  name: string
  number: string
  type: string

  constructor(id: number, name: string, number: string, type: string) {
    this.id = id
    this.name = name
    this.number = number
    this.type = type
  }
}