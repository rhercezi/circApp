export interface TaskDto {
    id: string
    ownerId: string
    title: string
    description: string
    isCompleted: boolean
    endDate: Date
    createdAt: Date
    updatedAt: Date | undefined
    parentTaskId: any
    userModels: TaskUserDto[] | undefined
    circleId: string | undefined
  }

export interface TaskUserDto {
    id: string
    userName: string
    isCompleted: boolean
    completedAt: string | undefined
  }
  