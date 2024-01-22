## Foto Storio
Foto Storio is a .NET e-commerce application that combines a Blazor WebAssembly frontend with a Web API backend.

---

![Screenshot](https://i.ibb.co/02zsbbc/fotostorio-screenshot.jpg "Screenshot")

## Features

- Hosted Blazor WebAssembly frontend
- Web API backend using Entity Framework and SQL Server
- Full checkout functionality and payment integration using Stripe Elements
- Authentication and authorisation using .NET Core Identity and JWT bearer tokens
- Implements the repository pattern along with the specification pattern
- API Response Caching for improved performance
- Responsive, mobile-first page layout throughout
- Styling with Tailwind CSS
- Shopping cart/basket functionality using Blazor's CascadingParameters
- Pagination, sorting and filtering of product data
- Fluent Validation

## Setup

To run the application on your local machine, after cloning or downloading it from GitHub, you'll need the following:

- The .NET 8 SDK installed locally
- SQL Server installed locally, or in a Docker container (see below)
- Node (if you want to customise the Tailwind CSS configuration)

If you're using Visual Studio as your code editor, you will need to be running the 2022 version (or higher).

Assuming that you have Docker Desktop installed and running, to create a container using the official **Microsoft SQL Server 2017 Docker image**, either enter the following from a terminal:

`docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=P@ssw0rd1234" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2017-latest`

*or*

you can make use of the **docker-compose.yml** file in the root directory of the application, and enter the following from a terminal window from within the application's root:

`docker-compose up -d`

The official Microsoft SQL Server image is 1.4Gb, so if you don't already have it downloaded in Docker this could take a few minutes, depending on the speed of your connection.

The application is set up to use this Docker container to provide its database, so if you use a local installation of SQL Server instead, you'll need to update the database connection strings in the **appsettings.Development.json** file in the **FotoStorio.Server** folder accordingly.

The application is set up to run as a hosted Blazor Webassembly app. This effectively means everything runs from the Server folder.

So, in a **Visual Studio Code** terminal, you can simply '`cd`' into the **FotoStorio.Server** folder and then type '`dotnet run`'.

In **Visual Studio 2022**, you should set the Server project as the default startup project from the right click menu, then press F5 to run it.

The application can optionally seed sample product, brand and category data into your database at startup. To enable/disable this, update the **"InitialDataSeeding"** value in the **appsettings.Development.json** file as required, then start the application. By default. this is set to **false** and will only run if the application environment is set to **Development**.

The images used throughout the site are publicly available at **imgbb.com** for the sake of convenience.
