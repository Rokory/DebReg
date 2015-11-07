DebReg is the web application used by Vienna EUDC 2015 to manage registration for the tournament.

Prerequisites for developer machine
===================================

Visual Studio 2015 (Community Edition should be okay)

Note: You have to configure Visual Studio to retrieve missing Nuget packages during build.

Prequisites for web server
==========================

Internet Information Services
.NET Framework 4.5 or 4.6
SQL Server 2012 or better

Note: We used Microsoft Azure to host the application for EUDC 2015. The free web apps in Azure are fine, but you have to pay for the database.

Setup
=====

Encryption key
--------------

For security purposes passwords for the SMTP configuration are encrypted using AES. To generate the encryption keys, run the CreateKeys project.

On the developer machine and the web server, set the environment variables APPSETTING_AES_IV and APPSETTING_AES_KEY to the generated values. For an Azure web app, you can set the keys using the application settings in the configuration. In Azure use the key names AES_IV and AES_Key.

Initial data
------------

Unfortunately there is no UI for some basic data needed to run Debreg. You can find this data in the project DebReg.Data in the file Configuration.cs in the Migrations folder. The code should be self-explanatory.

More information
================

DebReg Technical Documentation.docx
-----------------------------------

Contains information about user permissions and the APIs.

DebReg FAQ.docx
---------------

A very basic user guide.

DebReg Test Guide.docx
----------------------

A list of all use cases and their dependencies.


