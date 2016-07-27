# razor-sharp

Generation library and console application based on Razor template engine.

## Generation file

Generation is based on a Json file that defines the generation. Here it is an example of basic file:

```json
{
    "name": "name of the generation",
    "applicationName": "name_of_the_application",
    "templatesPath": "path_to_templates_directory",
    "outputPath": "path_to_target_location",
    "models": [],
    "projects": [],
    "items": [],
    "transformations": [],
    "paths": [],
    "additionalMetadataReferences": []
}
```

The following table contains an explanation of simple properties:

Name | Description | Default
-----|-------------|--------
name | Name of the generation. Used for logging purposes | `-`
applicationName | Name of the application running the library. This is required for compilation, and must correspond to the executable name (without extension) | `-`
templatesPath | Relative or absolute path to the templates | `-`
outputPath | Path where files will be generated | `.` (current directory)

> All properties without a default value are required  
> It is highly recommended to always specify an output location different than the current one

### Models

The `models` property of the file contains the list of objects that can be used as models for generation.

Every item in the array is the serialization of an object. Example:

```json
{
    "name": "Person_Jon_Doe",
    "value": {
        "$type": "MyNamespace.Person, MyProject",
        "name": "Jon",
        "lastName": "Doe"
    }
}
```

This would be the serialization of the following `C# class`:

```csharp
namespace MyNamespace
{
    using Newtonsoft.Json;

    public class Person
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }
    }
}
```

Please remark that serialized version contains as first item the `$type` property. The full value of this property will be explained later.

### Projects

The `projects` array contains a list of paths. A project item looks like this:

```json
{
    "projects": [
        "path_to_project_1",
        "path_to_project_2",
        "..."
    ]
}
```

Each item is a relative or absolute path pointing to a `project`.

A **project** is intended as a folder containing a `project.json` file and `.cs` files that can be compiled with `.NET core`.

### Items

The `items` property is an array of objects describing a transformation from a template to the output file. The
following example shows how items look like:

```json
{
    "items": [
        {
            "sourcePath": "path_to_a_file_to_copy.txt",
            "outputName": "copiedFile.txt"
        },
        {
            "templateName": "name_of_the_template",
            "outputName": "fileName.txt",
            "$modelRef": "Person_Jon_Doe",
        },
        {
            "templateName": "name_of_another_template",
            "outputName": "myOtherFile.ext",
            "model": {
                "$type": "MyNamespace.Person, MyProject"
                "name": "Jane",
                "lastName": "Doe"
            }
        }
    ]
}
```

### Transformations

The previous example would generate two files in the defined `outputPath` named respectively `fileName.txt` and `myOtherFile.txt`.

The first file uses the template with name `name_of_the_template` with the model `Person_Jon_Doe` referenced from the
**models** section in the document.

The second file uses the template with the name `name_of_another_template` with an inline model.

> It is preferable to use referenced models when the same object is used for twice or more; if a model is used only once,
it is suggested to use the inline model to keep it as close as possible to the usage

`$modelRef` and `model` are mutually exclusive. If both are specified, **$modelRef** will be used and a warning log
message will be written.  
It is also possible to omit both of them, for templates that don't require 

### Paths

The `paths` sections contains subfolders that can be generated within the output path. Example:

```json
{
    "paths": [
        {
            "name": "name_of_the_folder_1",
            "items": [
                {
                    "templateName": "name_of_the_template",
                    "outputName": "README.txt"
                }
            ],
            "paths": []
        },
        {
            "name": "
        }
    ]
}
```