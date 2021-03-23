export interface Message {
    id: number;
    senderId: number;
    senderUserName: string;
    senderPhotoUrl: string;
    recipinetId: number;
    recipientUserName: string;
    recipientPhotoUrl: string;
    content: string;
    dateRead?: Date;
    messageSent: Date ;
  }