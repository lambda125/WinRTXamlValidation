WinRT XAML Validation - Library Usage
=====================================

To use the library in your project, you have two alternatives: 1) Include the sources, 2) Include the binary DLL.


1) Usage by Source
-------------------

First copy the project folder "WinRTXAMLValidation.Library" into your solution folder
and add the project file to your solution. Reference the project in your Windows Store project by using "Add -> Reference...".

Second, copy the file "Styles/ValidationStyles.xaml" to your solution or add this file as link.
Reference ValidationStyles.xaml in your App.xaml, for example with:

<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            ...
            <ResourceDictionary Source="Common/ValidationStyles.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>


If you want to adapt the look & feel of your validation messages, go ahead and edit the ValidationStyles.xaml file.


2) Usage by Assembly
--------------------

If you don’t want to include the library’s sources to your solution, you can just add the compiled assembly as reference. 
To do this, first compile the WinRT XAML Validation library as Debug or Release. Copy the output assembly to your solution 
and reference it in every project where you want to use the validation functionality.

Copy the ValidationStyles.xaml file from the Styles folder of the library to your Windows Store App project 
and reference it as ResourceDictionary in your App.xaml (see above).



For more information, please visit https://winrtxamlvalidation.codeplex.com/Documentation