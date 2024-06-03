import { CircleDto } from "./CircleDto";

export interface CirclesByUserDto {
  id: string
  userName: string
  firstName: string
  familyName: string
  email: string
  circles: CircleDto[]
}