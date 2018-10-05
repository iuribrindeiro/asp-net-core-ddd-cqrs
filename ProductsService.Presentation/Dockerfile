FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 49865
EXPOSE 44367

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ProductsService.Presentation/ProductsService.Presentation.csproj ProductsService.Presentation/
COPY Bus/Bus.csproj Bus/
RUN dotnet restore ProductsService.Presentation/ProductsService.Presentation.csproj
COPY . .
WORKDIR /src/ProductsService.Presentation
RUN dotnet build ProductsService.Presentation.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ProductsService.Presentation.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ProductsService.Presentation.dll"]
