# Sora

Welcome to the Sora project! Sora is an osu! private server implemented in C# using ASP.NET and EF Core. The project is designed with plugin support in mind, allowing for extensibility and customization.

> [!NOTE]  
> This project is not recommended for beginners. If you're new to C# or programming in general, it's advisable to start with simpler projects to build your skills and understanding. However, you're welcome to explore the codebase and learn from it. You can freely copy and paste any code you find here, as the project is open source under the MIT License.

The only request I have is that you please credit me if you use any code from this project in your own work.

## Project Overview

Sora aims to replicate the functionality of the popular rhythm game osu!, providing a private server environment where players can enjoy the game with added features and flexibility. The project utilizes the following technologies:

* **C#**: The primary programming language used for developing the server.
* **ASP.NET**: Provides the framework for building web applications, used for handling server-side logic and communication.
* **Entity Framework (EF) Core**: A powerful ORM (Object-Relational Mapping) framework for .NET, used for data access and management within the server.
* **Plugins**: Sora is designed to support plugins, allowing for easy customization and addition of new features.

## Getting Started

To get started with the Sora project, follow these steps:

1. **Clone the Repository**: Clone the Sora repository to your local machine using Git:
```bash
git clone --recursive https://github.com/mempler/Sora.git
```
2. **Build and Run**: Open the project in your preferred IDE (Integrated Development Environment) or text editor, build the solution, and run the server.
3. **Configuration**: Customize the server configuration according to your requirements. At startup, Sora will create a new `config.json` file where you have to adjust the database configuration.
   After a successfull configuration, Sora will automatically migrate the outdated / unfinished Database onto the newer version Required by Sora. usually it doesn't matter what version of Sora you use.
   However, you cannot migrate back to the old version, only forward!
4. **Explore Plugins**: Explore the available plugins or develop your own to enhance the functionality of the server. Refer to the `Examples/` and `Plugins/` folder to see how a plugin may be written.
   Changing any functionality inside Sora itself works the same way. So if you don't know how something works, i highly recommend checking out how Sora itself is structured, It is exactly like a *big plugin*

## Contributing

Contributions to the Sora project are welcome! If you'd like to contribute, please follow these guidelines:

* **Fork the Repository**: Fork the Sora repository to your GitHub account.
* **Create a Branch**: Create a new branch for your feature or bug fix.
* **Commit Changes**: Make your changes and commit them with clear and descriptive messages.
* **Push Changes**: Push your changes to your forked repository.
* **Submit a Pull Request**: Submit a pull request from your branch to the main Sora repository for review.

## Support

If you encounter any issues or have questions about the Sora project, feel free to reach out to the project maintainers or open an issue on GitHub.

## ThirdParty Notice

This product includes GeoLite2 data created by MaxMind, available from
<a href="http://www.maxmind.com">http://www.maxmind.com</a>.
