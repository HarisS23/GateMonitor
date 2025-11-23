# GateMonitor

**GateMonitor** is an IoT-based RFID access control system designed for managing and monitoring machines. It allows administrators to control user access using RFID cards, track activity in real-time, and manage workers through a web dashboard.

---

## **Features**

- **RFID-Based Access Control:** Grant or revoke access for users to operate machines using RFID cards.  
- **Web Dashboard:** Monitor activity, manage workers, and view access logs in real-time.  
- **User Management:** Add, edit, or remove workers and assign permissions.  
- **Access Logs:** Keep track of machine usage and user interactions.  
- **Responsive Design:** Optimized for desktop and mobile devices.  
- **Secure Operations:** Only authorized users can control machines and access the dashboard.

---

## **Technology Stack**

- **Backend:** ASP.NET Core Web Application with Razor Pages (`.cshtml`)  
- **Frontend:** Bootstrap 5 for responsive UI, integrated with icons from Bootstrap Icons  
- **Database:** SQL Server / LocalDB (for storing user, access, and machine data)  
- **IoT Integration:** RFID reader connected to the machine to control access  
- **Real-Time Updates:** SignalR used for live monitoring and notifications

---

## **Project Structure**

- `Pages/` — Razor pages for the web dashboard (cshtml + cshtml.cs)  
- `wwwroot/` — Static files (CSS, JS, images)  
- `Controllers/` — API endpoints for managing workers and RFID events  
- `Models/` — Entity classes for database tables  
- `Services/` — Business logic and helpers (e.g., notification service, RFID handling)

---

## **Setup Instructions**

1. Clone the repository:

```bash
git clone https://github.com/HarisS23/GateMonitor.git
