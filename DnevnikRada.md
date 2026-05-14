# 📋 Dnevnik Rada – GateMonitor

> **Projekat:** GateMonitor – IoT sistem za kontrolu pristupa industrijskim mašinama  
> **Autor:** Haris Sejmenović
> **Period implementacije:** 16. novembar – 22. novembar 2025.  

---

## O projektu

GateMonitor je IoT sistem koji kontroliše industrijsku mašinu pomoću ESP32 mikrokontrolera i RFID autentifikacije. Cilj projekta je omogućiti da samo ovlašteni korisnici pokrenu mašinu, dok se stanje uređaja prati i upravlja putem web aplikacije u realnom vremenu.

---

## Dan 1 – Nedjelja, 16. novembar 2025.

### Prikupljanje materijala i komponenti

- Nabavka i priprema svih hardverskih komponenti potrebnih za projekat:
  - ESP32 mikrokontroler
  - RFID čitač (MFRC522) sa karticama/tagovima
  - OLED ekran (0.96", I2C)
  - LED diode (crvena i zelena)
  - Buzzer
  - Relej modul (5V)
  - Breadboard, žice, otpornici
  - Kućište / kutija za montažu
- Pregled i testiranje ispravnosti svake komponente prije ugradnje.
- Definisanje konačnog plana rasporeda komponenti unutar kućišta.
- Instalacija potrebnih alata: Arduino IDE, Visual Studio 2022, SQL Server.

---

## Dan 2 – Ponedjeljak, 17. novembar 2025.

### Fizičko sklapanje – Priprema kućišta i osnovnih elemenata

- Rezanje otvora na kućištu za:
  - Utičnicu (napojna priključnica)
  - Prekidač (glavni naponski)
  - Sijalično grlo
- Montaža utičnice, prekidača i sijalicnog grla u kućište.
- Žičanje naponskog dijela: spajanje utičnice s prekidačem i sijalicnim grlom preko releja.
- Provjera sigurnosti i ispravnosti naponske instalacije multimetrom.
- Postavljanje ESP32 na nosač unutar kućišta.

---

## Dan 3 – Utorak, 18. novembar 2025.

### Fizičko sklapanje – Integracija elektronskih komponenti

- Montaža RFID čitača (MFRC522) i spajanje na ESP32 putem SPI protokola:
  - SDA → GPIO 5
  - SCK → GPIO 18
  - MOSI → GPIO 23
  - MISO → GPIO 19
  - RST → GPIO 27
- Spajanje OLED ekrana na ESP32 putem I2C:
  - SDA → GPIO 21
  - SCL → GPIO 22
- Povezivanje LED dioda (zelena i crvena) s odgovarajućim GPIO pinovima i otpornicima.
- Spajanje buzzera na GPIO pin.
- Integracija relej modula koji kontroliše naponski krug mašine.
- Finalno uredivanje kablova i pričvršćivanje komponenti unutar kućišta.
- Inicijalni power-on test: provjera da sve komponente dobijaju napajanje.

---

## Dan 4 – Srijeda, 19. novembar 2025.

### Razvoj softvera – Programiranje ESP32

- Kreiranje novog Arduino projekta za ESP32 u Arduino IDE.
- Instalacija potrebnih biblioteka:
  - `MFRC522` – za RFID čitač
  - `Adafruit SSD1306` + `Adafruit GFX` – za OLED ekran
  - `WiFi` – za mrežnu konekciju
  - `WebSocketsClient` ili `HTTPClient` – za komunikaciju sa serverom
- Implementacija čitanja RFID kartica: ESP32 očitava UID i priprema ga za slanje.
- Implementacija WiFi konekcije i slanja HTTP POST zahtjeva na backend server s UID-om kartice.
- Implementacija logike prikaza na OLED ekranu: poruke `ACCEPTED` / `DENIED` / `WAITING`.
- Implementacija upravljanja LED diodama i buzzerom na osnovu odgovora servera.
- Implementacija upravljanja relejom: uključivanje/isključivanje mašine.
- Osnovno testiranje firmware-a sa simuliranim odgovorima (mock server).

---

## Dan 5 – Četvrtak, 20. novembar 2025.

### Razvoj softvera – Backend i Frontend (ASP.NET Core / Razor Pages)

- Postavljanje ASP.NET Core Web projekta s Razor Pages.
- Kreiranje baze podataka u MS SQL Server:
  - Tabela `Korisnici` (ID, Ime, UID kartice, Aktiviran)
  - Tabela `Logovi` (ID, KorisnikID, Timestamp, StatusPristupa, IzvorPromjene)
- Implementacija Entity Framework Core i migracija za kreiranje baza.
- Razvoj API endpoint-a `/api/rfid/validate`:
  - Prima UID s ESP32.
  - Provjerava postoji li korisnik s tim UID-om i ima li dozvolu pristupa.
  - Vraća JSON odgovor (`granted: true/false`).
- Razvoj administratorskog panela (Razor Pages):
  - Prikaz liste korisnika s mogućnošću dodavanja, uređivanja i brisanja.
  - Ručna kontrola mašine (dugmad "Uključi" / "Isključi").
  - Pregled historije pristupa (log tabela).
- Integracija SignalR huba za real-time ažuriranje dashboarda:
  - Emitovanje događaja pri svakom skeniranju kartice.
  - Emitovanje događaja pri promjeni statusa mašine.
- Testiranje komunikacije između ESP32 i backend servera.

---

## Dan 6 – Petak, 21. novembar 2025.

### Polishing i integracijsko testiranje

- Spajanje svih dijelova sistema: ESP32 ↔ Backend ↔ Baza ↔ Frontend.
- Testiranje kompletnog toka:
  - Skeniranje ovlaštene kartice → `ACCEPTED`, zelena LED, aktivacija releja (mašina se uključuje).
  - Skeniranje neovlaštene kartice → `DENIED`, crvena LED, mašina ostaje u trenutnom stanju.
  - Ručno uključivanje/isključivanje mašine s admin panela.
- Otklanjanje bugova pronađenih tokom integracijskog testiranja:
  - Popravljanje nestabilne WiFi konekcije ESP32 (dodano automatsko ponovno spajanje).
  - Ispravljanje grešaka u prikazu realnog vremena u web aplikaciji.
- Poboljšanje UI/UX admin panela: stilizacija stranica, responzivnost.
- Pisanje README.md fajla s opisom projekta i uputama za pokretanje.

---

## Dan 7 – Subota, 22. novembar 2025.

### Finalno testiranje i priprema za prezentaciju

- Kompletno end-to-end testiranje sistema u realnim uvjetima.
- Testiranje rubnih slučajeva:
  - Što se dešava ako server nije dostupan (ESP32 prikazuje grešku na OLED).
  - Što se dešava pri višestrukom brzom skeniranju.
- Finalne korekcije koda i čišćenje (uklanjanje debug ispisa, komentarisanje koda).
- Kreiranje testnih korisničkih naloga i RFID kartica za demonstraciju.
- Organizacija GitHub repozitorija: struktura foldera, `.gitignore`, finalni commit.
- Priprema demonstracionog scenarija za prezentaciju projekta žiriju.
- Finalna provjera ispravnosti kompletnog sistema.

---

## Sažetak tehnološkog stack-a

| Komponenta | Tehnologija |
|---|---|
| Mikrokontroler | ESP32 |
| RFID čitač | MFRC522 (SPI) |
| Ekran | OLED 0.96" (I2C) |
| Firmware | Arduino IDE (C++) |
| Backend | ASP.NET Core (C#) |
| Frontend | Razor Pages |
| Baza podataka | MS SQL Server |
| Real-time | SignalR / WebSocket |
| Verzionisanje | Git / GitHub |

---

## Napomene

Svi commitovi na GitHub repozitoriju hronološki prate navedene aktivnosti i služe kao dodatna verifikacija autorstva i toka razvoja projekta.
