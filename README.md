
# Introduction

openHAB Windows application is a native client for openHAB 2 & 3. It uses REST API of openHAB to render
sitemaps of your openHAB.

## Builds

[![CI Build](https://github.com/openhab/openhab-windows/actions/workflows/ci.yml/badge.svg)](https://github.com/openhab/openhab-windows/actions/workflows/ci.yml)
[![Latest App Release Build](https://github.com/openhab/openhab-windows/actions/workflows/app-release.yml/badge.svg)](https://github.com/openhab/openhab-windows/actions/workflows/app-release.yml)

## Code Analysis

The app project is using SonarQube hosted by SonarCloud to analyse the code for issues and code quality.

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/dashboard?id=openhab_openhab-windows)

### Quality Status

| Branch | Quality Gate Status | Bugs |Code Smells
|--------|---------------------|------ |------------|
| beta ||||
| main | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=openhab_openhab-windows&metric=alert_status)](https://sonarcloud.io/dashboard?id=openhab_openhab-windows)| [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=openhab_openhab-windows&metric=bugs)](https://sonarcloud.io/dashboard?id=openhab_openhab-windows)|[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=openhab_openhab-windows&metric=code_smells)](https://sonarcloud.io/dashboard?id=openhab_openhab-windows)|

## Setting up development environment

If you want to contribute to the Windows application we are here to help you to set up
development environment. openHAB Windows app is developed using Visual Studio 2019 and later.

- Download and install [Visual Studio Community Edition](https://www.visualstudio.com/downloads/)
- During install, make sure to select UWP SDK 17763 and SDK 19041
- Check out the latest code from github
- Open the project in Visual Studio (File -> Open, Project/Solution)
- Rebuild the solution to fetch all missing NuGet packages

You are ready to contribute!

Before producing any amount of code please have a look at [contribution guidelines](https://github.com/openhab/openhab.windows/blob/master/CONTRIBUTING.md)

## Trademark Disclaimer

Product names, logos, brands and other trademarks referred to within the openHAB website are the
property of their respective trademark holders. These trademark holders are not affiliated with
openHAB or our website. They do not sponsor or endorse our materials.
