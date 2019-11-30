Please open an issue with what you're proposing before getting too far along in coding any changes in order to ensure that you're working on something that'll be integrated and that you aren't doing something someone has already started.

## Environment Setup
**As of 11/25/2019, I've only verified this manually by myself, if anything is unclear or not working please open an issue and I'll try to help you out.**

TMI is an ASP.NET Core 3.0 web application with a SQL database. In production it's hosted in Azure. To setup your environment locally, clone the repo and open the Visual Studio solution. I'm using Visual Studio 2019, but it should work in 2017 also. 

### Database setup
The database uses Entity Framework migrations. To setup a local database for development:
1. Run "Script-Migration" in a PMC console to get a script to make a localDB.
2. Run the DBInit.sql script from the repo to create a basic database.

### Disable Facebook authentication
Comment out this code block in `Statup.cs` to disable Facebook authentication:
```
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
        facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
    })
```
*Please remember to remove the comment in the commit before you create a PR.*

### Working locally
The default database has two users:
* Email "admin@theminiindex.com" Password "Admin1!"
* Pasword "user@theminiindex.com" Password "User1!"

## What to change
Open to pretty much anything. Keeping track of most requests and bugs in GitHub Issues. Specific areas that could use improvements:
* Testing
* ASP.NET core concepts
* Open to UI changes
* Power user quality of life features/pages
* More providers

## Testing
We don't have any tests yet :(. If you're interested in making sure TMI remains reliable, feel free to contribute some!

## Deployment
Changes are integrated into our PPE environment when code is integrated into master, then manually deployed to production. When we get our first PR, we'll work on adding PR validation builds to a PPE environment.
