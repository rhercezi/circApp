import { UserDto } from "./UserDto"

export interface UsersByCircleDto {
    id: string
    name: string
    color: string
    users: UserDto[]
  }
  