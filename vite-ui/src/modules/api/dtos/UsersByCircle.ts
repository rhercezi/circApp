import { UserDto } from "./UserDto"

export interface UsersByCircle {
    id: string
    name: string
    color: string
    users: UserDto[]
  }
  