export interface TaskDto {
    id: string
    title: string
    description: string
    isCompleted: boolean
    endDate: string
    createdAt: string
    updatedAt: string | undefined
    parentTaskId: any
    userModels: TaskUserDto[] | undefined
    circleId: string | undefined
  }

export interface TaskUserDto {
    id: string
    username: string
    isCompleted: boolean
    completedAt: string | undefined
  }
  