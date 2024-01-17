export interface Message {
  id: number
  senderId: number
  senderUsername: string
  senderPhotoUrl: string
  receipentId: number
  receipentUsername: string
  receipentPhotoUrl: string
  content: string
  dateRead?: Date
  messageSent: Date
}
  