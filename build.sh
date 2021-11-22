dotnet pack src/Fabulous -c Release -o bin --version-suffix alpha.2 -p:VersionPrefix=2.0.0 -p:PackageReleaseNotes='First alpha release' -p:PackageDescription='Fabulous v2' -p:PackageLicenseExpression=Apache-2.0
dotnet pack src/Fabulous.XamarinForms -c Release -o bin --version-suffix alpha.2 -p:VersionPrefix=2.0.0 -p:PackageReleaseNotes='First alpha release' -p:PackageDescription='Fabulous for Xamarin.Forms v2' -p:PackageLicenseExpression=Apache-2.0
dotnet pack templates/Fabulous.XamarinForms.Templates.proj -o bin --version-suffix alpha.2 -p:VersionPrefix=2.0.0 -p:PackageReleaseNotes='First alpha release' -p:PackageDescription='Templates for Fabulous for Xamarin.Forms v2' -p:PackageLicenseExpression=Apache-2.0