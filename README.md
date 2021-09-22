## Foto Storio
Foto Storio is a .NET e-commerce application that combines a Blazor WebAssembly frontend with a Web API backend.

---

![Screenshot](https://i.ibb.co/02zsbbc/fotostorio-screenshot.jpg "Screenshot")

## Features

- Hosted Blazor WebAssembly frontend
- Web API backend using Entity Framework and SQL Server
- Full checkout functionality and payment integration with Stripe
- Authentication and authorisation using .NET Core Identity and JWT bearer tokens
- Implements the repository pattern along with the specification pattern
- API Response caching for improved performance
- Responsive, mobile-first page layout throughout
- Styling with Tailwind CSS
- Shopping cart/basket functionality using Blazor's CascadingParameters
- Pagination, sorting and filtering of product data
- Fluent Validation

## Setup

To run the application on your local machine, after cloning or downloading it from GitHub, you'll need the following:

- The .NET 5 SDK installed locally
- SQL Server installed locally, or in a Docker container
- Node (if you want to customise the Tailwind CSS configuration)

You'll then need to update the database connections strings in the **appsettings.Development.json** file in the Server folder.

The application is set up to run as a hosted Blazor Webassembly app. This effectively means everything runs from the Server folder.

So, in a **Visual Studio Code** terminal, you can simply '`cd`' into the **FotoStorio.Server** folder and then type '`dotnet run`'.

In **Visual Studio 2019**, you should set the Server project as the default startup project from the right click menu, then press F5 to run it.

The first time the application runs, it will seed sample product, brand and category data into your database.

The images used throughout the site are publicly available at **imgbb.com**, but could be stored anywhere. Each product has its image URL stored in the Products table in the database.
