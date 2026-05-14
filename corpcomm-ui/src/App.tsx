import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { LobbyPage } from './features/lobby/LobbyPage';
import { MeetingRoom } from './features/meetings/components/MeetingRoom';

const App: React.FC = () => {
    return (
        <Routes>
            {/* Головна сторінка (Лобі) */}
            <Route path="/" element={<LobbyPage />} />
            
            {/* Динамічний роут для кімнати */}
            <Route path="/room/:id" element={<MeetingRoom />} />
        </Routes>
    );
};

export default App;