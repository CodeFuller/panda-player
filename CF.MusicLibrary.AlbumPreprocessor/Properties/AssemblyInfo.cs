using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: AssemblyTitle("CF.MusicLibrary.AlbumPreprocessor")]
[assembly: AssemblyDescription("Tool for preprocessing music albums before adding to the collection")]

[assembly: ComVisible(false)]

[assembly: Guid("41420BD8-3820-404D-BB8A-CF98EDF70873")]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

[assembly: NeutralResourcesLanguage("en")]

[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
									 //(used if a resource is not found in the page, 
									 // or application resource dictionaries)
	ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
											  //(used if a resource is not found in the page, 
											  // app, or any theme specific resource dictionaries)
)]

[assembly: InternalsVisibleTo("CF.MusicLibrary.UnitTests")]
[assembly: InternalsVisibleTo("CF.MusicLibrary.IntegrationTests")]
