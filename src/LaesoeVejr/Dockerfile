#syntax=docker/dockerfile:1.7-labs

# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS buildenv
RUN apk add --no-cache clang zlib-dev
WORKDIR /src
COPY ["src/LaesoeVejr/LaesoeVejr.csproj", "src/LaesoeVejr/"]
RUN dotnet restore "./src/LaesoeVejr/LaesoeVejr.csproj"
COPY --exclude=*.html --exclude=*.ts . .
WORKDIR "/src/src/LaesoeVejr"

FROM buildenv AS build
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "./LaesoeVejr.csproj" -c $BUILD_CONFIGURATION --no-restore -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM buildenv AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LaesoeVejr.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:DebugType=None /p:DebugSymbols=false

# This stage is used to build the frontend
FROM node:24-alpine AS webpack
ARG WEBPACK_MODE=production
WORKDIR /src
COPY ["src/LaesoeVejr/package*.json", "./"]
RUN npm install --no-audit
WORKDIR /src
COPY ["src/LaesoeVejr/src/", "./src/"]
COPY ["src/LaesoeVejr/typings/", "./typings/"]
COPY ["src/LaesoeVejr/*.ejs", "./"]
COPY ["src/LaesoeVejr/tsconfig*.json", "./"]
COPY ["src/LaesoeVejr/webpack.config*", "./"]
RUN node_modules/.bin/webpack --mode ${WEBPACK_MODE}

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=webpack /src/wwwroot/. wwwroot
ENTRYPOINT ["/app/LaesoeVejr"]
