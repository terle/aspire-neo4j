# Aspire Neo4j Example

This example project demonstrates how to integrate Neo4j with an Aspire application using the **NorthernNerds.Aspire.Neo4** package. It includes a simple Razor component, `GraphDataBase.razor`, which interacts with the Neo4j database to showcase real-time data rendering.

## About This Example

The example project is designed to illustrate how you can use the `NorthernNerds.Aspire.Neo4` package in a practical scenario. The main highlight of this project is the `GraphDataBase.razor` component, which:

- **Streams Real-Time Data**: The component uses Neo4j to store and retrieve messages, displaying them in real-time on the webpage.
- **Demonstrates Neo4j Integration**: It shows how to use the `IDriver` provided by the `NorthernNerds.Aspire.Neo4` package to interact with the Neo4j database.
- **Handles Loading States**: The component includes a loading state that is displayed until the data from the Neo4j database is ready.

### `GraphDataBase.razor` Component

This Razor component is accessible at the `/graph` route and serves as a simple demonstration of how to interact with a Neo4j database in a web application. The component creates a new greeting node in the Neo4j database and displays the result on the page.

#### Key Features:
- **Dynamic Content**: The component dynamically creates a new node in the Neo4j graph database and retrieves a message to display.
- **Stream Rendering**: The component uses `StreamRendering` to enhance the user experience by showing loading content until the data is fully ready.
- **Output Caching**: The output is cached for 5 seconds to optimize performance.

### How It Works

When the `/graph` page is accessed:
1. The component injects the `IDriver` instance provided by the `NorthernNerds.Aspire.Neo4` package.
2. It creates a new "Greeting" node in the Neo4j database with a custom message.
3. The result is retrieved and displayed in a list on the page.
4. If the data is still being fetched, a loading message is shown to the user.

### Running the Example

To run the example, simply start the application and navigate to the `/graph` route. The page will load and interact with the Neo4j database to create and display a greeting message.

This example is a great starting point to see how the `NorthernNerds.Aspire.Neo4` package can be used in a real-world application, providing a straightforward way to integrate Neo4j into your Aspire projects.

## About Northern Nerds

Northern Nerds is a one-man freelance software company focused on delivering quality software tools. Follow our work to stay updated on the latest projects.