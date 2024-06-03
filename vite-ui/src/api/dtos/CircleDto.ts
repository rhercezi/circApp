export interface CircleDto {
    circleId: string
    creatorId: string | undefined
    userId: string | undefined
    inviterId: string | undefined
    name: string | undefined
    color: string | undefined
    users: string[] | undefined
  }