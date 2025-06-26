# Dotnet Pet Search

A user-friendly API allows for the search of available pets for adoption within a specific zip code. Users can also input a zip code or a set of coordinates to obtain the city’s information, enabling them to search for available pets in that area.

The Web application was developed using ASP.NET Web API and .NET 8. This application has been rewritten and updated to .NET 8 from .NET 7 for better performance and to utilize Object-Oriented Design principles for easier maintenance.

[Original Repository](https://github.com/AmielCyber/PetSearch)

The Angular application is hosted on Vercel, the React application is hosted on Netlify, and the ASP.NET Web API is hosted on Microsoft Azure.

[Pet Search Angular Application GitHub Repository](https://github.com/AmielCyber/pet-search-angular)

[Pet Search React Application GitHub Repository](https://github.com/AmielCyber/pet-search-react)

## Live Demo

**Note:**
Server response may take around 10 seconds during a
[cold start](https://azure.microsoft.com/en-us/blog/understanding-serverless-cold-start/cold) when the server is
reactivated after 10 minutes of inactivity. I'm considering upgrading the server to 'always on' on Microsoft Azure in
the future.

[ASP.NET API Library Specification with Swagger UI](https://pet-search.azurewebsites.net)

[Angular Application Live Demo](https://pet-search-angular.vercel.app)

[React Application Live Demo](https://pet-search-react.netlify.app)

## Preview

![Desktop Preview](/.github/images/DesktopPreview.gif)

![Mobile Preview](/.github/images/MobilePreview.gif)

## Technology Stack

<div style="display: flex; flex-wrap: wrap; gap: 5px">
    <img alt="C Sharp" width="30px" src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg"/>
    <img alt="Dotnet Core" width="30px" src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg"/>
    <img alt="Azure" width="30px" src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/azure/azure-original.svg"/>
    <img alt="MySQL" width="30px" src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/mysql/mysql-original.svg"/>
</div>

* Developed with C# and [ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet) Web API
* Tested with [Xunit](https://xunit.net/)
* Hosted at [Microsoft Azure Web App Deployment](https://azure.microsoft.com/en-us/products/app-service/web)
* Token storage with [MySQL](https://www.mysql.com/)
* API Documentation with [OpenApi/Swagger](https://www.openapis.org)
* Adoptable pet retrieval with [PetFinder API](https://www.petfinder.com/developers/v2/docs/)
* Location authentication with [MapBox Geolocation API](https://www.mapbox.com)

## Angular Application

[Angular GitHub Source Code](https://github.com/AmielCyber/pet-search-angular)

## React Application

### Note:
I am no longer serving the React files from ASP.NET. I have decided to host the React application at Netlify instead for
a better user experience and separation of concerns. I have also stopped providing updates for the React application.
All client updates will be done through the Angular application.

[React GitHub Source Code](https://github.com/AmielCyber/pet-search-react)

## Local Development Set Up

*Note: This set up assumes you have obtained keys from PetFinder API and MapBox API.*

### Required Dependencies

* [.NET SDK](https://dotnet.microsoft.com/en-us/download)
* [Docker Container for MySQL](https://hub.docker.com/_/mysql)

### Running the Production Build

1. Clone this repository:

```bash
git clone https://github.com/AmielCyber/DotnetPetSearch
```

2. After cloning this repository, go to the repository directory:

```bash
cd DotnetPetSearch
```
#### MySql Setup
You must set a MySQL database and create a username and password. You can do that with a docker container with a MySQL 8.0. To set up the container enter the following in your terminal:
```Bash
docker run -d - e MYSQL_ROOT_PASSWORD=secret -e MYSQL_DATABASE=petsearchdb --name petsearchdb -p 3307:3306 mysql: 8.0
```
You may replace the password and the database name to your own like, but you will also need to update those values in appsettings.Development.json

#### Get API Tokens
You must get access tokens from MapBox and PetFinder API

[PetFinder API](https://www.petfinder.com/developers/v2/docs/)

[MapBox Geolocation API](https://www.mapbox.com)

#### ASP.NET Setup
1. Go to the API application of the ASP.NET project:
```bash
cd ./DotnetPetSearch.API
```
2. Register **your** PetFinder and MapBox keys using [user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows)
```bash
dotnet user-secrets init
dotnet user-secrets set "PetFinderCredentials:ClientId" "your_client_id"
dotnet user-secrets set "PetFinderCredentials:ClientSecret" "your_client_secret"
dotnet user-secrets set "MapBox:AccessToken" "your_access_token"
```
3. Download and install NuGet dependencies:
    ```bash
    dotnet restore
    ```
4. Build the .NET application:
   ```bash
   dotnet build
   ```
5. Create MySQL database:
   ```bash
   dotnet ef database update -p ../DotnetPetSearch.Data
   ```
6. Run tests:
   ```bash
   dotnet test
   ```
7. Run the application:
   ```bash
   dotnet run
   ```
    * In the terminal check what port the application is using and click or copy the link and it should direct you to the Swagger UI Application
   ```
   http://localhost:{port}
   ```