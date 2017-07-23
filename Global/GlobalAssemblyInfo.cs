using System.Reflection;

//	http://stackoverflow.com/a/62637/5740031

[assembly: AssemblyProduct("MusicLibrary")]

[assembly: AssemblyCompany("CodeFuller")]
[assembly: AssemblyCopyright("Copyright © 2016-2017")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyVersion("1.0.*")]
