﻿//
//          Copyright Seth Hendrick 2016-2021.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Cake.Common;
using Cake.Common.IO;
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps.Tasks
{
    [TaskName( "make_release_notes" )]
    [TaskDescription( "Generates the release notes template in the root directory." )]
    public class ReleaseNoteTask : DefaultTask
    {
        // ---------------- Functions ----------------

        public override void Run( ChaskisContext context )
        {
            DirectoryPath distFolder = context.Directory( context.Argument( "dist", context.Paths.OutputPackages.ToString() ) );

            // Windows Stuff
            DirectoryPath windowsFolder = distFolder.Combine( "windows" );
            DirectoryPath pluginsFolder = distFolder.Combine( "windows/docker/Chaskis/Plugins" );
            FilePath chaskisExe = windowsFolder.CombineWithFilePath( context.File( "docker/chaskis/bin/Chaskis.dll" ) );
            FilePath chaskisCoreDll = windowsFolder.CombineWithFilePath( context.File( "docker/chaskis/bin/Chaskis.Core.dll" ) );
            string chaskisExeVersion = AssemblyName.GetAssemblyName( chaskisExe.ToString() ).Version.ToString( 3 );
            string chaskisCoreVersion = AssemblyName.GetAssemblyName( chaskisCoreDll.ToString() ).Version.ToString( 3 );
            string msiChecksum = System.IO.File.ReadAllText( windowsFolder.CombineWithFilePath( context.File( "ChaskisInstaller.msi.sha256" ) ).ToString() ).Trim();

            // NuGet Stuff
            FilePath nugetFile = distFolder.CombineWithFilePath( context.File( $"nuget/ChaskisCore.{chaskisCoreVersion}.nupkg.sha256" ) );
            string nugetChecksum = System.IO.File.ReadAllText( nugetFile.ToString() ).Trim();

            // Chocolatey Stuff
            FilePath chocoFile = distFolder.CombineWithFilePath( context.File( $"chocolatey/chaskis.{chaskisExeVersion}.nupkg.sha256" ) );
            string chocoChecksum = System.IO.File.ReadAllText( chocoFile.ToString() ).Trim();

            // Debian Stuff
            FilePath debFile = distFolder.CombineWithFilePath( context.File( $"debian/chaskis.deb.sha256" ) );
            string debChecksum = System.IO.File.ReadAllText( debFile.ToString() ).Trim();

            // Arch Stuff
            FilePath pkgFile = distFolder.CombineWithFilePath( context.File( $"arch_linux/chaskis-{chaskisExeVersion}-1-any.pkg.tar.zst.sha256" ) );
            string pkgChecksum = System.IO.File.ReadAllText( pkgFile.ToString() ).Trim();

            Dictionary<string, Version> pluginVersions = new Dictionary<string, Version>();
            foreach( DirectoryPath dir in context.GetDirectories( pluginsFolder.Combine( context.Directory( "*" ) ).ToString() ) )
            {
                string dirName = dir.GetDirectoryName();
                FilePath dll = dir.CombineWithFilePath( context.File( dirName + ".dll" ) );

                pluginVersions[dirName] = AssemblyName.GetAssemblyName( dll.ToString() ).Version;
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
    $@"{chaskisExeVersion} - <Insert Name Here>

<Insert Overall Description Here>

# Release Notes

## Chaskis - v{chaskisExeVersion}

### Feature 1

* #IssueNumber - 

### Feature 2

* #IssueNumber - 

## Chaskis.Core - v{chaskisCoreVersion}

* #IssueNumber - blah

## Plugin Updates:
"
            );

            foreach( var pluginVers in pluginVersions )
            {
                builder.AppendLine( $"### {pluginVers.Key} - v{pluginVers.Value.ToString( 3 )}" );
                builder.AppendLine( string.Empty );
                builder.AppendLine( "* #IssueNumber - blah" );
                builder.AppendLine( string.Empty );
            }

            builder.AppendLine( "# Downloads" );
            builder.AppendLine( string.Empty );

            builder.AppendLine( "## Windows" );
            builder.AppendLine( string.Empty );
            builder.AppendLine( $"* [Msi](https://files.shendrick.net/projects/chaskis/releases/{chaskisExeVersion}/windows/ChaskisInstaller.msi)" );
            builder.AppendLine( $"    * sha256: {msiChecksum}" );
            builder.AppendLine( $"* [Chocolatey](https://chocolatey.org/packages/chaskis)" );
            builder.AppendLine( $"    * [Mirror](https://files.shendrick.net/projects/chaskis/releases/{chaskisExeVersion}/chocolatey/chaskis.{chaskisExeVersion}.nupkg)" );
            builder.AppendLine( $"        * sha256: {chocoChecksum}" );
            builder.AppendLine( string.Empty );
            builder.AppendLine( string.Empty );

            builder.AppendLine( "## Chaskis.Core" );
            builder.AppendLine( string.Empty );
            builder.AppendLine( "* [Nuget](https://www.nuget.org/packages/ChaskisCore/)" );
            builder.AppendLine( $"    * [Mirror](https://files.shendrick.net/projects/chaskis/releases/{chaskisExeVersion}/nuget/ChaskisCore.{chaskisCoreVersion}.nupkg)" );
            builder.AppendLine( $"        * sha256: {nugetChecksum}" );
            builder.AppendLine( string.Empty );
            builder.AppendLine( string.Empty );

            builder.AppendLine( "## Debian" );
            builder.AppendLine( string.Empty );
            builder.AppendLine( $"* [Deb](https://files.shendrick.net/projects/chaskis/releases/{chaskisExeVersion}/debian/chaskis.deb)" );
            builder.AppendLine( $"    * sha256: {debChecksum}" );
            builder.AppendLine( string.Empty );
            builder.AppendLine( string.Empty );

            builder.AppendLine( "## Arch Linux" );
            builder.AppendLine( string.Empty );
            builder.AppendLine( $"* [Aur](https://aur.archlinux.org/packages/chaskis/)" );
            builder.AppendLine( $"    * [Mirror](https://files.shendrick.net/projects/chaskis/releases/{chaskisExeVersion}/arch_linux/)" );
            builder.AppendLine( $"    * sha256: {pkgChecksum}" );
            builder.AppendLine( string.Empty );
            builder.AppendLine( string.Empty );

            builder.AppendLine( "## Docker" );
            builder.AppendLine( "* [Windows Image](https://hub.docker.com/repository/docker/xforever1313/chaskis.windows)" );
            builder.AppendLine( "* [Ubuntu Image](https://hub.docker.com/repository/docker/xforever1313/chaskis.ubuntu)" );
            builder.AppendLine( "* [Raspbian Image](https://hub.docker.com/repository/docker/xforever1313/chaskis.raspbian)" );

            System.IO.File.WriteAllText( "ReleaseNotes.md", builder.ToString() );
        }
    }
}
