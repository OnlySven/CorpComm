# CorpComm — Платформа для корпоративного спілкування :rocket:

CorpComm — це сучасний веб-застосунок для корпоративного спілкування, орієнтований на високі навантаження та real-time взаємодію. [cite: 1]
Проєкт побудований на принципах **Clean Architecture** (Чистої архітектури) для бекенду та **Feature-Sliced Design** для фронтенду. [cite: 2]

## :dart: Поточний стан (MVP Phase 1) [cite: 3]

На даному етапі реалізовано ключовий функціонал:
1. **Масштабовані відеозустрічі (до 30 осіб):** Інтегровано SFU-сервер **LiveKit** для стабільного відеозв'язку. [cite: 3]
2. **Адаптивна відео-сітка:** Фронтенд автоматично адаптує відео-сітку, підтримує керування мікрофоном/камерою та демонстрацію екрана. [cite: 4]
3. **Real-time чат та сигналінг:** Використовується **SignalR** для миттєвого обміну системними повідомленнями та текстового чату в кімнатах. [cite: 5]
4. **Система запрошень:** Можливість генерувати унікальні посилання на кімнати та відправляти реальні email-запрошення через SMTP (імплементовано патерн Template Method). [cite: 6]

## :tools: Технологічний стек [cite: 7]

### **Backend:**
* .NET 8 (C# 12), ASP.NET Core Web API [cite: 7]
* Clean Architecture, CQRS (MediatR), Domain-Driven Design (DDD) [cite: 7]
* PostgreSQL + Entity Framework Core [cite: 7]
* ASP.NET Core SignalR + Redis (backplane) [cite: 7]
* JWT генерація (ручна реалізація для LiveKit) [cite: 7]

### **Frontend:**
* React 18 + TypeScript [cite: 7]
* Vite (збірка) [cite: 7]
* React Router (маршрутизація) [cite: 7]
* `@livekit/components-react` (UI для відеоконференцій) [cite: 7]
* `@microsoft/signalr` (клієнт для вебсокетів) [cite: 7]

### **Infrastructure:**
* Docker & Docker Compose (PostgreSQL, Redis, LiveKit Server) [cite: 7]

---

## :rocket: Як почати роботу (Local Setup)

### Передумови
* Встановлений [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Встановлений [Node.js](https://nodejs.org/) (рекомендовано v18+)
* Встановлений та запущений [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Крок 1. Запуск інфраструктури (База даних, Redis, LiveKit)
Відкрийте термінал у корені проєкту та підніміть контейнери:
```bash
docker compose up -d
```
**Зверніть увагу:** сервер LiveKit налаштований з прапорцем `--node-ip=127.0.0.1` для коректної маршрутизації WebRTC-пакетів при локальній розробці. [cite: 8]

### Крок 2. Налаштування бекенду [cite: 9]
1. Застосуйте міграції бази даних:
```bash
dotnet ef database update --project CorpComm.Infrastructure --startup-project CorpComm.WebAPI
```
2. Переконайтеся, що у файлі `CorpComm.WebAPI/appsettings.json` налаштовано блок `SmtpSettings` для відправки імейлів. [cite: 9]
3. Запустіть WebAPI: [cite: 10]
```bash
dotnet run --project CorpComm.WebAPI
```
Бекенд запуститься за адресою `http://localhost:5000` або `https://localhost:5001`. [cite: 10]

### Крок 3. Запуск фронтенду [cite: 11]
Відкрийте новий термінал, перейдіть до папки клієнтського застосунку, встановіть залежності та запустіть сервер розробки:
```bash
cd corpcomm-ui
npm install
npm run dev
```
Відкрийте у браузері адресу, яку видасть Vite (зазвичай `http://localhost:5173`). [cite: 11]

## 🏗 Структура проєкту

* `CorpComm.Domain/`, `CorpComm.Application/`, `CorpComm.Infrastructure/`, `CorpComm.WebAPI/` — класичні шари бекенду за правилами Dependency Rule. [cite: 12]
* `corpcomm-ui/` — React-застосунок. [cite: 13]
    * `src/features/` — ізольовані модулі екранів (наприклад, lobby, meetings). [cite: 13]
    * `src/services/` — сервіси для роботи з API та вебсокетами (наприклад, `signalrService.ts`). [cite: 14]

## 🤝 Завдання для команди (Що робити далі) [cite: 15]

Поточні технічні та бізнесові задачі:
* **[Tech Debt] Винесення ключів LiveKit:** У класі `GetMeetingTokenQueryHandler.cs` ключі `devkey` та `secret` захардкоджені. [cite: 15] Їх потрібно винести в `appsettings.json` через патерн `IOptions`. [cite: 16]
* **Аутентифікація:** Налаштування загальної системи користувачів (ASP.NET Core Identity + JWT для доступу до WebAPI). [cite: 16]
* **Обмін файлами:** Додавання можливості надсилати файли у текстовий чат кімнати (інтеграція AWS S3 / Azure Blob). [cite: 17]
* **Тестування:** Написання Unit-тестів для MediatR хендлерів та сервісу генерації JWT. [cite: 18]

---
Розроблено та підтримується командою архітекторів CorpComm.
