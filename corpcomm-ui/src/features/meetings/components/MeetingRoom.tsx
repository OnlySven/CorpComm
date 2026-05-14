import React, { useEffect, useState, useRef } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import { signalrService } from '../../../services/signalrService';

// Імпортуємо магію LiveKit
import { LiveKitRoom, VideoConference, RoomAudioRenderer } from '@livekit/components-react';
import '@livekit/components-styles';

export const MeetingRoom: React.FC = () => {
    const { id: roomId } = useParams<{ id: string }>();
    const location = useLocation();
    const navigate = useNavigate();
    
    const [messages, setMessages] = useState<string[]>([]);
    const [liveKitToken, setLiveKitToken] = useState<string>(''); // Стан для токена
    
    const userName = location.state?.userName;
    const connectionStarted = useRef(false);

    // 1. Ефект для підключення до текстового чату (SignalR)
    useEffect(() => {
        if (!userName || !roomId) {
            navigate(`/?room=${roomId}`);
            return;
        }

        if (connectionStarted.current) return;
        connectionStarted.current = true;

        const connectToRoom = async () => {
            try {
                signalrService.onUserJoined((user) => {
                    setMessages((prev) => [...prev, `🟢 ${user} приєднався до кімнати`]);
                });

                await signalrService.connect(roomId, userName);
                setMessages((prev) => [...prev, `✅ Ви успішно підключилися як ${userName}`]);
            } catch (error: any) {
                if (error.name === 'AbortError' || error.message?.includes('stopped')) return;
                setMessages((prev) => [...prev, `❌ Помилка підключення до чату`]);
                connectionStarted.current = false;
            }
        };

        connectToRoom();

        return () => {
            signalrService.disconnect();
            connectionStarted.current = false;
        };
    }, [roomId, userName, navigate]);

    // 2. Ефект для отримання токена відеозв'язку (LiveKit)
    useEffect(() => {
        if (!roomId || !userName) return;

        const fetchToken = async () => {
            try {
                // ЗМІНА ТУТ: Забираємо http://localhost... залишаємо тільки /api/...
                const response = await fetch(`/api/meetings/${roomId}/token?userName=${userName}`);
                
                if (response.ok) {
                    const data = await response.json();
                    setLiveKitToken(data.token);
                    console.log("Токен успішно отримано!"); // Додамо лог для перевірки
                } else {
                    console.error(`Помилка сервера: ${response.status} ${response.statusText}`);
                }
            } catch (error) {
                console.error("Мережева помилка запиту токена:", error);
            }
        };

        fetchToken();
    }, [roomId, userName]);

    return (
        <div style={{ display: 'flex', flexDirection: 'column', height: '100vh', padding: '10px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '10px' }}>
                <h2>Кімната: {roomId}</h2>
                <button onClick={() => navigate('/')} style={{ padding: '8px 16px' }}>Вийти з кімнати</button>
            </div>

            <div style={{ display: 'flex', gap: '20px', flex: 1, minHeight: 0 }}>
                {/* ЛІВА ЧАСТИНА: Відео-сітка LiveKit */}
                <div style={{ flex: 3, borderRadius: '8px', overflow: 'hidden', background: '#111' }}>
                    {liveKitToken ? (
                        <LiveKitRoom
                            video={true} // Автоматично вмикати камеру при вході
                            audio={true} // Автоматично вмикати мікрофон при вході
                            token={liveKitToken}
                            serverUrl="ws://localhost:7880" // Локальний сервер LiveKit з Docker
                            data-lk-theme="default" // Стандартна красива тема LiveKit
                            style={{ height: '100%' }}
                        >
                            <VideoConference />
                            <RoomAudioRenderer />
                        </LiveKitRoom>
                    ) : (
                        <div style={{ display: 'flex', height: '100%', alignItems: 'center', justifyContent: 'center' }}>
                            <h3>Отримання доступу до відео...</h3>
                        </div>
                    )}
                </div>

                {/* ПРАВА ЧАСТИНА: Текстовий чат SignalR */}
                <div style={{ flex: 1, display: 'flex', flexDirection: 'column', background: '#222', padding: '15px', borderRadius: '8px' }}>
                    <h3>Системні події:</h3>
                    <ul style={{ color: '#fff', padding: '0', listStyle: 'none', overflowY: 'auto' }}>
                        {messages.map((msg, index) => (
                            <li key={index} style={{ marginBottom: '10px', paddingBottom: '5px', borderBottom: '1px solid #444', fontSize: '14px' }}>
                                {msg}
                            </li>
                        ))}
                    </ul>
                </div>
            </div>
        </div>
    );
};