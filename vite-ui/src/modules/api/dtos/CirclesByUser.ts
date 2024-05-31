import { CircleDto } from "./CircleDto";

export interface CirclesByUser {
  id: string
  userName: string
  firstName: string
  familyName: string
  email: string
  circles: CircleDto[]
}