export interface AppointmentDto {
  id: string
  creatorId: string
  startDate: Date
  endDate: Date
  title: string
  detailsInCircles: string[]
  circles: string[]
  details?: DetailsDto | undefined
}

export interface DetailsDto {
  appointmentId: string
  note: string
  address?: AddressDto | undefined
  reminders?: ReminderDto[] | undefined
}

export interface AddressDto {
  country: string
  city: string
  street: string
  housenumber: string
  postCode: string
  longitude: number
  latitude: number
}

export interface ReminderDto {
  time: Date
  message: string
  justForUser: boolean
}

export interface DetachedDetailsDto {
  userId: string
  details: DetailsDto
}