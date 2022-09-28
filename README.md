# Limbo Vimeo [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) [![NuGet](https://img.shields.io/nuget/v/Limbo.Umbraco.Vimeo.svg)](https://www.nuget.org/packages/Limbo.Umbraco.Vimeo) [![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.Vimeo.svg)](https://www.nuget.org/packages/Limbo.Umbraco.Vimeo) [![Our Umbraco](https://img.shields.io/badge/our-umbraco-%233544B1)](https://our.umbraco.com/packages/backoffice-extensions/limbo-vimeo/)

Vimeo video picker for Umbraco 10.

<table>
  <tr>
    <td><strong>License:</strong></td>
    <td><a href="./LICENSE.md"><strong>MIT License</strong></a></td>
  </tr>
  <tr>
    <td><strong>Umbraco:</strong></td>
    <td>
      Umbraco 10
      <sub><sup>(and <a href="https://github.com/limbo-works/Limbo.Umbraco.Vimeo/tree/v1/main">Umbraco 9</a>)</sup></sub>
    </td>
  </tr>
  <tr>
    <td><strong>Target Framework:</strong></td>
    <td>
      .NET 6
      <sub><sup>(and <a href="https://github.com/limbo-works/Limbo.Umbraco.Vimeo/tree/v1/main">.NET 5</a>)</sup></sub>
    </td>
  </tr>
</table>





<br /><br />

## Installation

Install the <a href="https://www.nuget.org/packages/Limbo.Umbraco.Vimeo/2.0.0-alpha001" target="_blank">NuGet package</a> - either via the .NET CLI:

```
dotnet add package Limbo.Umbraco.Vimeo --version 2.0.0-alpha001
```

or the NuGet package manager:

```
Install-Package Limbo.Umbraco.Vimeo -Version 2.0.0-alpha001
```





<br /><br />

## Property Editor

The package features a property editor that allows users to insert a single Vimeo video - either from the URL of the video or an embed code. The property editor will pull information about the inserted video from the Vimeo API, exposing this information for you in the property value.

![image](https://user-images.githubusercontent.com/3634580/159897092-715b3f00-1516-4c62-be4d-923b0275606f.png)

![image](https://user-images.githubusercontent.com/3634580/159897124-2d9d8f00-a275-429d-991a-50778089b19b.png)

When a valid Vimeo has been inserted on a property, the property exposes an instance of `VimeoValue`. Details about the video can be accessed via the `Details` property, and embed information can be accessed through the `Embed` property:

```cshtml
@using Limbo.Umbraco.Vimeo.Models.Videos
@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage

@{

    // Get the media from the media cache
    var media = Umbraco.Media(1234);

    // Get the property value
    var vimeo = media.Value<VimeoValue>("video");

    // Render the video title
    <h1>@vimeo.Details.Title</h1>

    // Render the embed code
    @vimeo.Embed

}
```

<br /><br />

## Configuration

```json
{
  "Limbo": {
    "Vimeo": {
      "Credentials": [
        {
          "Key": "8a7a2756-ddc4-486d-978b-a38f116990c1",
          "Name": "MyApp",
          "Description": "A description about the credentials.",
          "AccessToken": "Your access token here"
        }
      ]
    }
  }
}
```

#### Key
The key should be a randomly generated GUID which will be used as a unique identifier for the credentials.

#### Name + Description
The name and description are currently not used, but are meant to be shown in the UI to identify the credentials to the user.

#### AccessToken
An access token must be specified in order to authenticate your Vimeo user when making requests to th Vimeo API.

You can generate a new access token either by [creating a new Vimeo app](https://developer.vimeo.com/apps/new) or using one of [your existing apps](https://developer.vimeo.com/apps). For the desired app, you can then generate a new personal access token:

![image](https://user-images.githubusercontent.com/3634580/159975720-7541ddce-f59e-4ac9-9a31-fcfa03c97e03.png)

An access token may also be obtained through an OAuth 2.0 authentication flow. This is currently not supported directly by this package, 
but you can see the Skybrud.Social documentation on how to [set up an authentication page](https://social.skybrud.dk/vimeo/authentication/setting-up-an-authentication-page/).
