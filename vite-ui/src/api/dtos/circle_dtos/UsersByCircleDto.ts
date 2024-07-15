import { UserDto } from "../user_dtos/UserDto"

export interface UsersByCircleDto {
    id: string
    name: string
    color: string
    users: UserDto[]
  }
  