FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY global.json ./
COPY src/Productos.Api/Productos.Api.csproj src/Productos.Api/
COPY src/Productos.Api/packages.lock.json src/Productos.Api/
RUN dotnet restore src/Productos.Api --locked-mode
COPY src/Productos.Api/ src/Productos.Api/
RUN dotnet publish src/Productos.Api -c Release --no-restore -o /out
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Productos.Api.dll"]
