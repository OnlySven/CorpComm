import * as signalR from '@microsoft/signalr';

class SignalRService {
    private connection: signalR.HubConnection;

    constructor() {
        // src/services/signalrService.ts
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5046/hubs/meeting") 
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();
    }

    public async connect(meetingId: string, userName: string) {
        try {
            // 1. Стартуємо тільки якщо ми повністю відключені
            if (this.connection.state === signalR.HubConnectionState.Disconnected) {
                await this.connection.start();
                console.log("SignalR: З'єднання встановлено.");
            }

            // 2. КРИТИЧНО: Перевіряємо стан ще раз ПЕРЕД тим, як викликати метод
            // Якщо React викликав disconnect() поки ми чекали на start(), стан не буде Connected
            if (this.connection.state === signalR.HubConnectionState.Connected) {
                await this.connection.invoke("JoinMeeting", meetingId, userName);
                console.log(`SignalR: Увійшли в групу ${meetingId}`);
            }
        } catch (err: any) {
            // Ігноруємо помилки переривання з'єднання
            if (err.name === 'AbortError' || err.message?.includes('stopped')) {
                return;
            }
            throw err; // Прокидаємо реальні помилки (наприклад, 404 або 500)
        }
    }

    public onUserJoined(callback: (user: string) => void) {
        this.connection.off("UserJoined");
        this.connection.on("UserJoined", callback);
    }

    public async disconnect() {
        // Зупиняємо тільки якщо ми не в стані Disconnected 
        if (this.connection.state !== signalR.HubConnectionState.Disconnected) {
            try {
                await this.connection.stop();
                console.log("SignalR: З'єднання розірвано.");
            } catch (err) {
                console.error("Помилка при відключенні:", err);
            }
        }
    }
}

export const signalrService = new SignalRService();