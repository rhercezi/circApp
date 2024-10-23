import { UserDto } from "./UserDto";

export interface LoginDto {
    tokens: null
    user: UserDto
    exp: number
}