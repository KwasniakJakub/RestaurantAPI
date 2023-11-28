# Projekt Interfejsu API ASP.NET

Witaj w repozytorium zawierającym kod źródłowy mojego projektu interfejsu API napisanego w technologii ASP.NET. Ten projekt został stworzony w ramach kursu na Udemy "Praktyczny kurs ASP.NET Core REST Web API od podstaw (C#)
" autorstwa "Jakub Kozyra".

## Opis

Projekt ten to interfejs API do obsługi operacji na danych restauracji. Umożliwia dodawanie, edytowanie, usuwanie oraz pobieranie informacji o restauracjach i ich potrawach.

## Funkcje

- **Aktualizacja Restauracji:**
  - Endpoint: `PUT /api/restaurant/{id}`
  - Metoda: `Update`
  - Aktualizuje informacje o restauracji na podstawie przekazanych danych.

- **Usuwanie Restauracji:**
  - Endpoint: `DELETE /api/restaurant/{id}`
  - Metoda: `Delete`
  - Usuwa restaurację na podstawie przekazanego identyfikatora.

- **Tworzenie Restauracji:**
  - Endpoint: `POST /api/restaurant`
  - Metoda: `CreateRestaurant`
  - Tworzy nową restaurację na podstawie przekazanych danych. Wymaga uprawnień roli "Manager" lub "Admin".

- **Pobieranie Wszystkich Restauracji:**
  - Endpoint: `GET /api/restaurant`
  - Metoda: `GetAll`
  - Pobiera listę wszystkich restauracji z możliwością filtrowania i sortowania.

- **Tworzenie Potrawy w Restauracji:**
  - Endpoint: `POST /api/restaurant/{restaurantId}/dish`
  - Metoda: `Post`
  - Tworzy nową potrawę w określonej restauracji na podstawie przekazanych danych.

- **Pobieranie Wszystkich Potraw w Restauracji:**
  - Endpoint: `GET /api/restaurant/{restaurantId}/dish`
  - Metoda: `Get`
  - Pobiera listę wszystkich potraw w określonej restauracji.

- **Pobieranie Konkretnej Potrawy w Restauracji:**
  - Endpoint: `GET /api/restaurant/{restaurantId}/dish/{dishId}`
  - Metoda: `Get`
  - Pobiera informacje o konkretnej potrawie w określonej restauracji.

- **Usuwanie Wszystkich Potraw w Restauracji:**
  - Endpoint: `DELETE /api/restaurant/{restaurantId}/dish`
  - Metoda: `Delete`
  - Usuwa wszystkie potrawy w określonej restauracji.

## Wymagania systemowe

- [.NET Core SDK](https://dotnet.microsoft.com/download)
- Dowolne środowisko programistyczne obsługujące ASP.NET (np. Visual Studio, Visual Studio Code)
- [Dodatkowe wymagane oprogramowanie lub narzędzia]

## Uruchamianie

1. Sklonuj repozytorium: `git clone https://github.com/KwasniakJakub/RestaurantAPI.git`
2. Otwórz projekt w wybranym środowisku programistycznym.
3. Skompiluj i uruchom aplikację.

```bash
dotnet build
dotnet run
