# Currency Exchange Office System

## Course
Network Application Development

## Project Title
Currency Exchange Office System

## Author
Anar Ilgarli

## Student ID
64520

## Project Description
A network-based currency exchange office system built with WCF (Windows Communication Foundation), WPF, and SQL Server LocalDB. The system allows users to register, log in, manage a PLN account, and perform real-time currency exchange operations using live rates fetched from the NBP (National Bank of Poland) API.

### Features
- Real-time exchange rates from NBP API
- User registration and login
- PLN account top-up
- Buy and sell currencies
- Transaction history
- Historical exchange rates

## How to Run

1. Clone or download this repository
2. Open `CurrencyExchangeSystem.sln` in Visual Studio
3. Ensure SQL Server LocalDB is installed (comes with Visual Studio)
4. Set **CurrencyExchangeSystem** as the Startup Project
5. Press **F5** to build and start the WCF service
6. Right-click **WpfClient** → **Debug** → **Start new instance** to launch the client
 

## Database

SQL Server LocalDB with 3 tables:
- **Users** – user accounts and credentials
- **Balances** – PLN and foreign currency balances per user
- **Transactions** – full history of all exchange operations

## External API

NBP (National Bank of Poland) API is used for live and historical exchange rates:
[http://api.nbp.pl/en.html](http://api.nbp.pl/en.html)

