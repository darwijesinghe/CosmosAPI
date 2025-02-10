# AzureCosmosDbApp

## Project Purpose
This is a basic CRUD application using Azure Cosmos DB NoSQL. It provides standard Create, Read, Update, and Delete operations through a REST API. The project serves as a practice implementation for working with Cosmos DB in a RESTful architecture.

## Contributors
- Darshana Wijesinghe

## App Features
- Supports Create, Read, Update, and Delete operations for managing data.

## Future Enhancements
- Stored Procedures – Implementing Cosmos DB stored procedures for efficient data processing.
- Triggers – Adding triggers to handle business logic within the database.

## Requirements
- .NET 6.0
- Azure Cosmos DB NoSQL
- Azure SDK for Cosmos DB
- Postman or any API client

## Usage

- ### Create a Task
```json5
POST /api/cosmos/createrecord/{containerName}

// Request Body

{
  "TaskName": "The first task for the cosmos db.",
  "Assignee": "Nadeesha",
  "Deadline": "2025-02-10" 
}
```
- ### Get a Task
```json5
GET /api/cosmos/gettask/{id}/{containerName}
```
- ### Get all Tasks
```json5
GET /api/cosmos/gettasks/{containerName}
```
- ### Update a Task
```json5
PUT /api/cosmos/updatetask/{id}/{containerName}

// Request Body

{
  "TaskName": "The first task for the cosmos db.",
  "Assignee": "Harshani",
  "Deadline": "2025-02-10" 
}
```
- ### Delete a Task
```json5
DELETE /api/cosmos/deletetask/{id}/{containerName}
```
## Support

Darshana Wijesinghe  
Email address - [dar.mail.work@gmail.com](mailto:dar.mail.work@gmail.com)  
Linkedin - [darwijesinghe](https://www.linkedin.com/in/darwijesinghe/)  
GitHub - [darwijesinghe](https://github.com/darwijesinghe)

## License

This project is licensed under the terms of the **MIT** license.
