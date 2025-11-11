export class UserModel {
  id: number
  firstName: string
  email: string

  constructor(id: number, firstName: string, email: string) {
    this.id = id
    this.firstName = firstName
    this.email = email
  }
}