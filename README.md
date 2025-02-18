# ASP.Net Core MVC Blog App

A blog app built with C# and ASP.NET Core MVC, packed with features like user authentication, an easy-to-use admin panel for content management, and more. You can view the screenshots [here](Screenshots/)

## Features

- **User Authentication**
  - Signup, login, and logout functionality.
  - Custom user model where instead of having to sign in with e-mail, you could use your username.
- **Admin Panel**
  - The panel is accesible to each user with the "Admin" role through the dropdown menu in the navbar.
  - View posts, comments, categories and users.
  - Edit or delete existing users in the database.
  - Quickly make posts from the dashboard.
- **Pagination and Search Bar**
  - All the posts and comments are paginated and sorted by their creation date in a descending order.
  - You can search for each post that you may be looking for.
- **Custom text formatting**
  - Each user can customize the way their posts and comments look thanks to [Froala](https://froala.com)!
- **Profile Management**
  - Users can access their profiles via the dropdown on the navbar. 
  - Each user is able to edit view and edit their own posts.

  

## Project Structure

```bash
.
├── Areas
│   └── Identity                # All the Identity files required for user authentication, etc.
├── bin                         # Stores compiled output files
├── Controllers
│   ├── AdminController.cs      # Controller for the admin panel
│   ├── CategoriesController.cs # Controller for the Category model
│   ├── HomeController.cs       # Controller for the home page (or pages not related to a specific model)
│   └── PostsController.cs      # Controller for the Post model
├── Data
│   ├── BlogDbContext.cs        # Main DbContext file
│   ├── BlogUser.cs             # Custom user model
│   └── SeedData.cs             # Automatically seeds the database if it's empty
├── Migrations                  # Entity Framework Core database migrations
├── Models
│   ├── ViewModels              # All the view models are stored here
│   ├── Category.cs             # The Category model
│   ├── Comment.cs              # The Comment model
│   ├── ErrorViewModel.cs       # View model to display errors
│   └── Post.cs                 # The Post model
├── obj                         # Holds temporary build files and intermediate object files used during compilation
├── Properties                  # Contains configuration files
├── Screenshots                 # Screenshots of the app
├── Views
│   ├── Admin                   # All the views for the admin panel
│   ├── Categories              # All the views for the Category model
│   ├── Home                    # All the views for the home page
│   ├── Posts                   # All the views for the Post model
│   ├── Shared                  # Base Layouts
│   ├── _ViewImports.cshtml     # Used for dependency injection
│   └── _ViewStart.cshtml       # Sets the default layout
├── wwwroot                     # Static files
├── appsettings.json            # Self-explanatory
├── MvcBlog.csproj              # Defines dependencies, target frameworks, and build settings
├── PaginatedList.cs            # Logic for pagination
└── Program.cs                  # The entry point of the app
```

## Setup Instructions

1. Prerequisites:

Before setting up the project, ensure you have the following installed:  

  - [.NET SDK (Latest LTS version)](https://dotnet.microsoft.com/download)  
  - SQLite
  - [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) (optional but recommended)  
  - [Entity Framework Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (install it using `dotnet tool install --global dotnet-ef`)  

2. Clone the Repository:

    ```bash
    git clone https://github.com/TinyPuff/MvcBlog.git  
    cd MvcBlog  
    ```

3. Apply Migrations:

   ```bash
   dotnet ef database update
   ```

4. Build & Run the Application:

   ```bash
   dotnet build
   dotnet run
   ```

5. Access the application at `http://local:5000/` (if you see a different port in the command line/terminal, you should use that instead of 5000).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.