// src/features/meetings/MeetingRoom.tsx
import React, { useEffect, useState } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import { signalrService } from '../../../services/signalrService';

export const MeetingRoom: React.FC = () => {
    const { id: roomId } = useParams<{ id: string }>();
    const location = useLocation();
    const navigate = useNavigate();
    
    const [messages, setMessages] = useState<string[]>([]);
    const userName = location.state?.userName;

    useEffect(() => {
        if (!userName || !roomId) {
            navigate(`/?room=${roomId}`);
            return;
        }

        let isMounted = true; // Запобігає оновленню стану демонтованого компонента

        const connectToRoom = async () => {
            try {
                // Підписка
                signalrService.onUserJoined((user) => {
                    if (isMounted) {
                        setMessages((prev) => [...prev, `🟢 ${user} приєднався до кімнати`]);
                    }
                });

                // Підключення
                await signalrService.connect(roomId, userName);
                
                if (isMounted) {
                    setMessages((prev) => [...prev, `✅ Ви успішно підключилися як ${userName}`]);
                }
            } catch (error: any) {
                if (error.name === 'AbortError' || error.message?.includes('stopped')) return;
                
                if (isMounted) {
                    setMessages((prev) => [...prev, `❌ Помилка підключення`]);
                }
            }
        };

        connectToRoom();

        return () => {
            isMounted = false;
            // Коли виходимо з кімнати — розриваємо з'єднання
            signalrService.disconnect();
        };
    }, [roomId, userName, navigate]);

    return (
        <div style={{ padding: '20px' }}>
            <h2>Кімната: {roomId}</h2>
            <button onClick={() => navigate('/')}>Вийти з кімнати</button>

            <div style={{ marginTop: '20px' }}>
                <h3>Системні повідомлення (SignalR):</h3>
                <ul style={{ background: '#222', color: '#fff', padding: '20px', borderRadius: '8px', listStyle: 'none' }}>
                    {messages.map((msg, index) => (
                        <li key={index} style={{ 
                            marginBottom: '10px', 
                            paddingBottom: '5px', 
                            borderBottom: '1px solid #444' 
                        }}>
                            {msg}
                        </li>
                    ))}
                </ul>
            </div>

            <div style={{ marginTop: '40px', padding: '20px', border: '2px dashed #ccc', textAlign: 'center' }}>
                <p>Тут буде відео-сітка LiveKit (на 30 осіб)</p>
            </div>
        </div>
    );
};