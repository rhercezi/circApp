export interface AppointmentDto {
    creatorId: string
    date: string
    detailsInCircles: string[]
    circles: string[]
    details: DetailsDto
  }
  
  export interface DetailsDto {
    appointmentId: string
    note: string
    address: AddressDto
    reminders: ReminderDto[]
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
    time: string
    message: string
  }