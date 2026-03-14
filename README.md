# FutureBound
**Tagline**: Save for Tomorrow, Bound for the Future
A lightweight, user-friendly .NET MAUI mobile app for simple savings and expense tracking, designed for easy cash flow management with zero professional financial knowledge required.

## Core Features
### 1. User Authentication
- Secure account login/registration with local storage
- Multi-account management & independent data storage
- Account logout for data privacy protection

### 2. Financial Tracking
- Real-time homepage balance overview
- One-click deposit/expense recording with auto-balance update
- Transaction history inquiry with date filtering (year/month/day)
- Specialized bill management (custom named bills with real-time balance summary)

### 3. Auxiliary Functions
- Software settings (amount precision, date format customization)
- Local data backup/restore & cache clearing
- Multi-account switching (personal/family accounts)
- App version & usage instructions

## Tech Stack
- **Framework**: .NET MAUI (C#/XAML)
- **Local Storage**: MAUI Preferences (secure account data storage)
- **UI**: MAUI native UI components (simple, intuitive interface)
- **Version Control**: Git (feature branches, pull requests, conflict resolution)

## How to Run
1. Clone the repository: `git clone https://github.com/Chan-Cool/FutureBound.git`
2. Open the project in **Visual Studio 2022+** (with .NET MAUI workload installed)
3. Build and run on Android/Windows emulator/real device (no internet connection required - fully standalone)

## Key Design Principles
- **Simplicity First**: No complex operations, easy for all user groups
- **Standalone Usage**: Full local data storage, no network dependency
- **Data Accuracy**: Atomic operations for balance calculation, no deviation
- **Smooth Experience**: Lazy loading for transaction records, real-time UI refresh via data binding

## Target Users
Students, new workplace entrants, housewives, and all users seeking a simple, no-threshold financial tracking tool.
