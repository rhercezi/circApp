export interface AddUsersDto {
    circleId: string
    inviterId: string | undefined
    users: string[] | undefined
  }