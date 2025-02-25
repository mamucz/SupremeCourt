# SupremeCourt - Alice in Borderland

**SupremeCourt** is an online multiplayer game inspired by the *Alice in Borderland* series. Players participate in a game where their choices determine their survival in the brutal world of the Supreme Court.

## 🎮 Game Rules

1. **Number of Players:** 5
2. **Time Limit:** 3 minutes to choose a number (0 to -100).
3. **Result Calculation:** The average of chosen numbers is multiplied by ×0.8 and rounded.
4. **Winning Condition:** The player with the closest number to the result wins the round.
5. **Penalties:**
   - Other players lose 1 point.
   - If a player reaches **-10 points**, they are eliminated.
6. **Special Rules:**  
   - If two players choose the same number, both lose a point.
   - If someone hits the exact result, all others lose 2 points.
   - If a player selects **0**, another player can win by selecting **100**.
7. **Waiting Room:** After creating a game, players have **1 minute** to join.

---

## 🏛 Application Architecture

### **🛠 Clean Architecture**
The application is designed following **Clean Architecture** principles with separate layers:

📂 **1. Domain Layer**  
   - Contains **game rules**, **entities**, **interfaces**, and **calculations**.  
   - **No dependencies** on the database or external services.  
   - **Used design patterns:**  
     - **Factory Pattern** (creating game objects)  
     - **Repository Pattern** (entity management)

📂 **2. Application Layer**  
   - Contains **services, validations, and business logic**.  
   - Communicates with the domain and infrastructure layers.  
   - **Used design patterns:**  
     - **CQRS (Command-Query Responsibility Segregation)** - separates read/write operations  
     - **Dependency Injection** - all services are injected  

📂 **3. Infrastructure Layer**  
   - Implements **repository pattern** for the database.  
   - Uses **Entity Framework Core** for **MS SQL / MySQL**.  
   - **SignalR** for **real-time communication**.

📂 **4. Presentation Layer**  
   - REST API in **ASP.NET Core 8**  
   - **SignalR Hubs** for sending messages to players  
   - JWT authentication

---

## ✅ SOLID Principles

- **S - Single Responsibility Principle**: Each class has a single responsibility.
- **O - Open/Closed Principle**: Game rules are extendable without modifying existing code.
- **L - Liskov Substitution Principle**: `IPlayerRepository` allows different database implementations.
- **I - Interface Segregation Principle**: Large interfaces are divided (`IGameRepository`, `IAuthService`).
- **D - Dependency Inversion Principle**: `IGameService` and `IWaitingRoomService` are abstractions.

---

## 🚀 How to Run the Project?

### **1️⃣ Clone the Repository**
```sh
git clone https://github.com/user/SupremeCourt.git
cd SupremeCourt
```

### **2️⃣ Setup the Environment**
- Create **appsettings.json** and configure database connection.
- Supported databases: **MS SQL Server / MySQL**.

### **3️⃣ Apply Database Migrations**
```sh
dotnet ef database update --project SupremeCourt.Infrastructure --startup-project SupremeCourt.Presentation
```

### **4️⃣ Run the Backend**
```sh
cd SupremeCourt.Presentation
dotnet run
```

### **5️⃣ Run the Angular Frontend** (if completed)
```sh
cd SupremeCourt.Frontend
ng serve
```

---

## 🛠 Technologies Used

| Technology        | Version |
|--------------------|--------|
| .NET 8            | ✅ |
| ASP.NET Core      | ✅ |
| Entity Framework  | ✅ |
| MS SQL / MySQL    | ✅ |
| SignalR           | ✅ |
| Angular           | ✅ |

📌 **License:** MIT  
📌 **Authors:** [mamucz(Petr Ondra - Back End && Tobias Ondra - Front End)]
