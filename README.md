# HTTPBIN

Test you HTTP calls with this API. It captures request and replays the details back to you.

## Bulding and running

You need to have a PostgreSQL database running. Modify the connection string inside appsettings.json to match your database.

```bash
# Install dependencies
dotnet restore

# Run migrations
dotnet ef database update

# Run the application
dotnet run
```

## Usage

TODO
