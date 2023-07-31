Details about Refrigerator Application:-
1. This application is built with C# Winforms, Entity Framework with database first approach.
2. It is using SQL Server database to store product data.
3. Since its built with DBFirst approach so it needs database tables to be created first. Follow below steps to do this.
	a. Please run Refrigerator.sql in SQL server management studio to create database.
	b. Please run Refrigerator_CreateTables.sql to create tables.
4. If you are trying to open source code then open RefrigeratorApplication.sln and edit app.config file to change the server name according to your SQLSever server. Alternatively you can go to release folder and edit RefrigeratorApplication.exe.config to update server name. And then run the application.
5. Initially there won't be any data in tables so first add some product types which you want to store e.g Milk, Mango, Lemon, Chillies etc.
6. Once product types are added then it will be visible in Product Type combo box. Now your Refrigerator Application is ready to store things.
7. It keeps track of products addition/consumption as well as expiry and stores details in Product_Log table which can be viewed by clicking Check History.
