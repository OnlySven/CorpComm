import React, { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';

export const LobbyPage: React.FC = () => {
    const [userName, setUserName] = useState('');
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();

    // Читаємо параметр ?room= з URL (як ми робили в JS)
    const roomId = searchParams.get('room');

    const handleJoin = () => {
        if (!userName.trim()) {
            alert('Будь ласка, введіть своє ім\'я');
            return;
        }

        if (roomId) {
            // Якщо кімната є в URL, переходимо в неї, передаючи ім'я
            navigate(`/room/${roomId}`, { state: { userName } });
        } else {
            // Якщо кімнати немає, для тесту генеруємо випадкову
            const newRoomId = crypto.randomUUID();
            navigate(`/room/${newRoomId}`, { state: { userName } });
        }
    };

    return (
        <div style={{ padding: '40px', maxWidth: '400px', margin: '0 auto', textAlign: 'center' }}>
            <h1>CorpComm</h1>
            <h2>{roomId ? `Запрошення у кімнату: ${roomId}` : 'Створити нову зустріч'}</h2>
            
            <div style={{ marginTop: '20px', display: 'flex', flexDirection: 'column', gap: '10px' }}>
                <input 
                    type="text" 
                    placeholder="Ваше ім'я..." 
                    value={userName}
                    onChange={(e) => setUserName(e.target.value)}
                    style={{ padding: '10px', fontSize: '16px' }}
                />
                <button 
                    onClick={handleJoin}
                    style={{ padding: '10px 20px', fontSize: '16px', cursor: 'pointer' }}
                >
                    {roomId ? 'Приєднатися' : 'Створити кімнату'}
                </button>
            </div>
        </div>
    );
};