# Currency Exchange Office System

## Author
Anar Ilgarli

## Course
Network Application Development

## Project Description
A network-based currency exchange office system built with WCF, WPF and SQL Server.

## System Architecture
- **WCF Service** - Business logic, NBP API integration
- **WPF Client** - Desktop user interface
- **SQL Server LocalDB** - Data persistence

## Features
- Real-time exchange rates from NBP API
- User registration and login
- PLN account top-up
- Buy and sell currencies
- Transaction history
- Historical exchange rates

## How to Run
1. Open CurrencyExchangeSystem.sln in Visual Studio
2. Set CurrencyExchangeSystem as Startup Project
3. Press F5 to start the WCF service
4. Right-click WpfClient - Debug - Start new instance

## Database
SQL Server LocalDB with 3 tables:
- Users - user accounts
- Balances - PLN and currency balances
- Transactions - all exchange operations

## NBP API
http://api.nbp.pl/en.html