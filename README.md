# Crudify

This package/.NET tool generates Model, ApiController and Controller for a particular entity.

# Install Package

~~~
$ dotnet tool install -g crudify
~~~

# Run

~~~
$ crud --help
~~~

# Usage

`crud` command expects two arguments, area name and model name. Navigate into the project root directory and execute the command.

### Folder Structure

~~~
Project/
  |--Areas/
    |--Area1/
    |--Area2/
  |...
~~~

### Create CRUD

~~~
\Project> crud -a TestArea -m TestModel
~~~
