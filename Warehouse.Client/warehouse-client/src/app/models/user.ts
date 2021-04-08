export interface User {
  id: string,
  fullName : string,
  userName : string,
  passwordHash : string,
  email : string,
  phone : string,
  registrationDateTime : Date,
  roles: []
}
